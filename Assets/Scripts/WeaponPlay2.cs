using Abertay.Analytics;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class PlayerTwoController : MonoBehaviour
{
    public Rigidbody2D playerRigidbody;
    public Rigidbody2D gunRigidbody;
    public Transform firePoint;
    public UnityEngine.GameObject bulletPrefab;
    public float aimSpeed = 100f;
    public float maxRotationAngle = 45f;
    public float recoilForceMagnitude = 10f;
    public float recoilTorqueMagnitude = 5f;
    public float endForceMagnitude = 20f;
    public float randomForceMagnitude = 2f;
    public Transform rightCorner; // Empty object for rotation pivot
    public float shootCooldown = 1f; // Cooldown time in seconds

    private float currentRotationAngle = 0f;
    private float initialRotationAngle = 0f;
    private bool isRotating = false;
    private bool hasAppliedEndTorque = false;
    private bool hasAppliedRecoil = false;
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
            if (Input.GetKeyDown(KeyCode.Return) && Time.time >= lastShotTime + shootCooldown)
            {
                Shoot();
                ApplyRecoil();
                lastShotTime = Time.time; // Update the last shot time
            }

            if (Input.GetKeyUp(KeyCode.Return))
            {
                hasAppliedRecoil = false;
            }
        }

        //ApplyRandomForces();
    }

    void HandleAiming()
    {
        float aimDirection = 0f;

        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            initialRotationAngle = currentRotationAngle;
            isRotating = true;
            hasAppliedEndTorque = false;

            // Log aiming event
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "player", "Player2" },
                { "event", "WeaponAimed" },
                { "initialRotationAngle", initialRotationAngle }
            };
            SendCustomEvent("WeaponAimed", parameters);
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            aimDirection = 1f;
            isMovingClockwise = false;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            aimDirection = -1f;
            isMovingClockwise = true;
        }

        float newRotationAngle = currentRotationAngle + aimDirection * aimSpeed * Time.deltaTime;
        newRotationAngle = Mathf.Clamp(newRotationAngle, -maxRotationAngle, maxRotationAngle);
        float rotationDelta = newRotationAngle - currentRotationAngle;
        transform.RotateAround(rightCorner.position, Vector3.forward, rotationDelta);

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
        float torqueDirection = isMovingClockwise ? -1f : 1f;
        if (GameManager.Instance.AreInputsAllowed())
        {
            playerRigidbody.AddTorque(proportionalForce * torqueDirection, ForceMode2D.Impulse);
        }
        hasAppliedEndTorque = true;
        isRotating = false;

        // Log end torque application event
        Dictionary<string, object> parameters = new Dictionary<string, object>
        {
            { "player", "Player2" },
            { "event", "EndTorqueApplied" },
            { "totalAngleTraversed", totalAngleTraversed },
            { "proportionalForce", proportionalForce }
        };
        SendCustomEvent("EndTorqueApplied", parameters);
    }

    void Shoot()
    {
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // Create parameters for the custom event
        Dictionary<string, object> parameters = new Dictionary<string, object>
        {
            { "player", "Player2" },
            { "event", "Player2Shot" },
            { "position", firePoint.position },
            { "rotation", firePoint.rotation.eulerAngles }
        };

        // Send the custom event
        SendCustomEvent("Player2Shot", parameters);
    }

    void ApplyRecoil()
    {
        if (hasAppliedRecoil) return;

        Vector2 shootingDirection = firePoint.right;
        Vector2 recoilForce = -shootingDirection * recoilForceMagnitude;
        playerRigidbody.AddForceAtPosition(recoilForce, playerRigidbody.worldCenterOfMass, ForceMode2D.Impulse);

        Vector2 pointOfApplication = firePoint.position;
        float torque = Vector2.SignedAngle(pointOfApplication - playerRigidbody.worldCenterOfMass, recoilForce);
        playerRigidbody.AddTorque(torque * recoilTorqueMagnitude, ForceMode2D.Impulse);

        hasAppliedRecoil = true;
    }

    /*void ApplyRandomForces()
    {
        Vector2 randomForce = new Vector2
           (Random.Range(-randomForceMagnitude, randomForceMagnitude),
            Random.Range(-randomForceMagnitude, randomForceMagnitude));
        playerRigidbody.AddForce(randomForce, ForceMode2D.Force);
    }*/

    void SendCustomEvent(string eventName, Dictionary<string, object> parameters)
    {
        // Implementation for sending custom events
        Debug.Log($"Event: {eventName}, Parameters: {parameters}");
        AnalyticsManager.SendCustomEvent(eventName, parameters);
    }
}
