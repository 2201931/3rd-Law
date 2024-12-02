using Abertay.Analytics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPlay : MonoBehaviour
{
    public Transform FirePoint;
    public UnityEngine.GameObject BulletPrefab;
    public Rigidbody2D playerRigidbody;
    public Rigidbody2D gunRigidbody;
    public Transform leftCorner; // Empty object for rotation pivot
    public float recoilForceMagnitude = 10f; // Backward force magnitude
    public float recoilTorqueMagnitude = 5f; // Rotational force magnitude
    public float aimSpeed = 100f; // Speed at which the gun aims up and down
    public float maxRotationAngle = 70f; // Maximum rotation angle in degrees
    public float endForceMagnitude = 2f; // Force applied at the end of rotation limits
    public float shootCooldown = 1f; // Cooldown time in seconds

    private float currentRotationAngle = 0f;
    private float initialRotationAngle = 0f;
    private bool hasAppliedEndTorque = false;
    private bool hasAppliedRecoil = false; // New flag to track recoil application
    private bool isRotating = false;
    private bool isMovingClockwise = false;
    private float lastShotTime = 0f; // Time of the last shot

    void Start()
    {
        playerRigidbody.gravityScale = 0;
    }

    void Update()
    {
        HandleAiming();

        if (GameManager.Instance.AreInputsAllowed())
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                DataCollector.Instance.P1_ButtonPressesPerShot++;
            }
            if (Input.GetKeyDown(KeyCode.Space) && Time.time >= lastShotTime + shootCooldown)
            {

                Shoot();
                DataCollector.Instance.P1_ButtonPressesPerShot = DataCollector.Instance.P1_ButtonPressesPerShot / DataCollector.Instance.P1_ShotsFired;
                ApplyRecoil();
                lastShotTime = Time.time; // Update the last shot time
            }

            if (Input.GetKeyUp(KeyCode.Space))
            {
                hasAppliedRecoil = false;
            }
        }

        float aV = playerRigidbody.angularVelocity;
        DataCollector.Instance.P1_TotalSpeed += playerRigidbody.angularVelocity * Time.deltaTime;
        if (aV > DataCollector.Instance.P1_MaxRotationSpeed)
        {
            DataCollector.Instance.P1_MaxRotationSpeed = aV;
        }
        DataCollector.Instance.P1_AverageSpeed = DataCollector.Instance.P2_TotalSpeed / DataCollector.Instance.matchTime;
        //ApplyRandomForces();
    }

    void HandleAiming()
    {
        float aimDirection = 0f;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S))
        {
            initialRotationAngle = currentRotationAngle;
            isRotating = true;
            hasAppliedEndTorque = false;

            // Log aiming event
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "player", "Player1" },
                { "event", "WeaponAimed" },
                { "initialRotationAngle", initialRotationAngle }
            };
            SendCustomEvent("WeaponAimed", parameters);
        }

        if (Input.GetKey(KeyCode.W))
        {
            aimDirection = -1f;
            isMovingClockwise = false;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            aimDirection = 1f;
            isMovingClockwise = true;
        }
        float newRotationAngle = currentRotationAngle + aimDirection * aimSpeed * Time.deltaTime;
        newRotationAngle = Mathf.Clamp(newRotationAngle, -maxRotationAngle, maxRotationAngle);
        float rotationDelta = newRotationAngle - currentRotationAngle;
        transform.RotateAround(leftCorner.position, Vector3.forward, rotationDelta);

        currentRotationAngle = newRotationAngle;
        if (isRotating && !hasAppliedEndTorque && (currentRotationAngle == maxRotationAngle || currentRotationAngle == -maxRotationAngle))
        {
            ApplyEndTorque();
        }
        else if (currentRotationAngle != maxRotationAngle && currentRotationAngle != -maxRotationAngle)
        {
            hasAppliedEndTorque = false;
        }
    }

    void ApplyEndTorque()
    {
        float totalAngleTraversed = Mathf.Abs(currentRotationAngle - initialRotationAngle);
        if (totalAngleTraversed > 180f)
        {
            totalAngleTraversed = 360f - totalAngleTraversed;
        }

        float proportionalForce = endForceMagnitude * (totalAngleTraversed / (2 * maxRotationAngle));
        float torqueDirection = isMovingClockwise ? 1f : -1f;
        if (GameManager.Instance.AreInputsAllowed())
        {
            float finalTorque = proportionalForce * torqueDirection;
            playerRigidbody.AddTorque(finalTorque, ForceMode2D.Impulse);
            DataCollector.Instance.P1_TotalTorqueApplied += Mathf.Abs(finalTorque);
        }
        hasAppliedEndTorque = true;
        isRotating = false;

        // Log end torque application event
        Dictionary<string, object> parameters = new Dictionary<string, object>
        {
            { "player", "Player1" },
            { "event", "EndTorqueApplied" },
            { "totalAngleTraversed", totalAngleTraversed },
            { "proportionalForce", proportionalForce }
        };
        SendCustomEvent("EndTorqueApplied", parameters);
    }

    void Shoot()
    {
        DataCollector.Instance.P1_ShotsFired++;
        Instantiate(BulletPrefab, FirePoint.position, FirePoint.rotation);

        // Create parameters for the custom event
        Dictionary<string, object> parameters = new Dictionary<string, object>
        {
            { "player", "Player1" },
            { "event", "Player1Shot" },
            { "position", FirePoint.position },
            { "rotation", FirePoint.rotation.eulerAngles }
        };

        // Send the custom event
        SendCustomEvent("Player1Shot", parameters);
    }

    void ApplyRecoil()
    {
        if (hasAppliedRecoil) return;

        Vector2 shootingDirection = FirePoint.right;
        Vector2 recoilForce = -shootingDirection * recoilForceMagnitude;
        playerRigidbody.AddForceAtPosition(recoilForce, playerRigidbody.worldCenterOfMass, ForceMode2D.Impulse);

        Vector2 pointOfApplication = FirePoint.position;
        float torque = Vector2.SignedAngle(pointOfApplication - playerRigidbody.worldCenterOfMass, recoilForce);
        playerRigidbody.AddTorque(torque * recoilTorqueMagnitude, ForceMode2D.Impulse);

        hasAppliedRecoil = true;
    }

    void SendCustomEvent(string eventName, Dictionary<string, object> parameters)
    {
        // Implementation for sending custom events
        Debug.Log($"Event: {eventName}, Parameters: {parameters}");
        AnalyticsManager.SendCustomEvent(eventName, parameters);
    }

    /*void ApplyRandomForces()
    {
        Vector2 randomForce = new Vector2(
            Random.Range(-randomForceMagnitude, randomForceMagnitude),
            Random.Range(-randomForceMagnitude, randomForceMagnitude)
        );
        playerRigidbody.AddForce(randomForce, ForceMode2D.Force);
    }*/
}
