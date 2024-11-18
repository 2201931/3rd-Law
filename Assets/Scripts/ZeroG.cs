using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ZeroG : MonoBehaviour
{
    public Rigidbody2D player1Rigidbody;
    public Rigidbody2D player2Rigidbody;
    private Camera mainCamera;
    private Vector2 screenBounds;
    public float speedReductionFactor = 0.9f; // Factor to reduce speed upon collision

    void Start()
    {
        // Disable gravity for both players' Rigidbody2D
        player1Rigidbody.gravityScale = 0;
        player2Rigidbody.gravityScale = 0;

        // Initialize the main camera and screen boundaries
        mainCamera = Camera.main;
        screenBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.transform.position.z));
    }

    void Update()
    {
        // Check if the "G" key is pressed
        if (Input.GetKeyDown(KeyCode.G))
        {
            // Reload the current scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        // Handle boundary collisions for GameObject 1
        HandleBoundaryCollisions(player1Rigidbody);

        // Handle boundary collisions for GameObject 2
        HandleBoundaryCollisions(player2Rigidbody);
    }

    void HandleBoundaryCollisions(Rigidbody2D playerRigidbody)
    {
        // Check if the Rigidbody2D is null
        if (playerRigidbody == null) return;

        // Get the player's current position and velocity
        Vector3 viewPos = playerRigidbody.transform.position;
        Vector2 velocity = playerRigidbody.velocity;

        // Check and handle boundary collisions
        if (viewPos.x > screenBounds.x)
        {
            viewPos.x = screenBounds.x;
            velocity.x = -velocity.x * speedReductionFactor;
        }
        else if (viewPos.x < -screenBounds.x)
        {
            viewPos.x = -screenBounds.x;
            velocity.x = -velocity.x * speedReductionFactor;
        }

        if (viewPos.y > screenBounds.y)
        {
            viewPos.y = screenBounds.y;
            velocity.y = -velocity.y * speedReductionFactor;
        }
        else if (viewPos.y < -screenBounds.y)
        {
            viewPos.y = -screenBounds.y;
            velocity.y = -velocity.y * speedReductionFactor;
        }

        // Apply the updated position and velocity
        playerRigidbody.transform.position = viewPos;
        playerRigidbody.velocity = velocity;
    }
}
