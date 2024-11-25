using UnityEngine;

public class Onboarding : MonoBehaviour
{
    public Transform[] waypoints; // Array of waypoints (empty GameObjects)
    public Collider2D p1Collider; // P2's collider
    public Collider2D bulletCollider; // Bullet's collider
    private int hitCount = 0; // Counter for the number of hits
    private int currentWaypointIndex = 0; // Index of the current waypoint
    public UIManager uiManager; // Reference to the UIManager
    private bool bulletMode = false; // Flag to indicate bullet mode

    void Start()
    {
        // Initialize PracticeManager
        PracticeManager.Instance.InitializePracticeMode();

        // Set initial position to the first waypoint
        if (waypoints != null && waypoints.Length > 0 && waypoints[0] != null)
        {
            transform.position = waypoints[0].position;
            currentWaypointIndex = 1; // Ensure we start at the first waypoint and move to the next one
        }
        else
        {
            Debug.LogError("Waypoints array is null, empty, or the first waypoint is null.");
        }

        // Set initial UI message
        if (uiManager != null)
        {
            uiManager.SetInstructionText("Knock into the Hexagon");
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collision detected with: " + collision.gameObject.name);
        if (collision.collider == p1Collider && hitCount < 5)
        {
            Debug.Log("Collided with player");
            hitCount++;
            MoveToNextWaypoint();

            if (hitCount == 5)
            {
                bulletMode = true; // Enable bullet mode
                Debug.Log("Bullet mode activated");
                if (uiManager != null)
                {
                    uiManager.SetInstructionText("Now shoot the Hexagon");
                }
            }
        }
        else if (collision.collider == bulletCollider && bulletMode)
        {
            Debug.Log("Collided with bullet in bullet mode");
            hitCount++;
            MoveToNextWaypoint();
        }
        else if (collision.collider == bulletCollider && !bulletMode)
        {
            Debug.Log("Bullet collision ignored because bullet mode is not active.");
        }
    }

    void MoveToNextWaypoint()
    {
        Debug.Log("In waypoint function");

        if (waypoints != null && currentWaypointIndex < waypoints.Length)
        {
            if (waypoints[currentWaypointIndex] != null)
            {
                transform.position = waypoints[currentWaypointIndex].position;
                currentWaypointIndex++;
                Debug.Log("Moved to waypoint index: " + currentWaypointIndex);
            }
            else
            {
                Debug.LogError("Waypoint at index " + currentWaypointIndex + " is null.");
            }
        }
        else
        {
            Debug.LogError("Waypoints array is null or index out of range.");
        }
    }
}
