using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameRules : MonoBehaviour
{
    public GameObject bulletPrefab;
    public GameObject P1;
    public GameObject P2;
    public float forceAmount = 100f;
    private Vector3 P1Spawn;
    private Vector3 P2Spawn;
    public GameManager gameManager; // Reference to the GameManager
    public Text victoryText; // Reference to the UI Text element

    private Dictionary<GameObject, int> playerKills = new Dictionary<GameObject, int>();
    private float roundStartTime; // Track the start time of each round

    private void Start()
    {
        // Starting spawn position for later when I respawn
        P1Spawn = P1.transform.position;
        P2Spawn = P2.transform.position;
        victoryText.gameObject.SetActive(false); // Hide the victory text at the start

        // Initialise the Analytics Manager
        Abertay.Analytics.AnalyticsManager.Initialise("GameEnvironment");

        // Start the first round
        StartNewRound();
    }

    private void Update()
    {
        // Add additional game logic maybe?? Anyway, it goes here
    }

    public void BulletHitPlayer(GameObject bullet, GameObject player)
    {
        // Destroy the bullet after it hits a player
        Destroy(bullet);

        // Add force to the player if it has the tag "Player"
        if (player.CompareTag("Player"))
        {
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.AddForce(Vector2.up * forceAmount, ForceMode2D.Impulse);
            }

            // Start the death delay coroutine
            StartCoroutine(DelayDeath(player));
        }
    }

    public void PlayerDied(GameObject player)
    {
        if (!player.CompareTag("Player"))
        {
            Debug.Log("Ignored death for non-player object: " + player.name);
            return;
        }

        Debug.Log("Player died: " + player.name);

        // Increment score for the opposing player
        if (player == P1)
        {
            gameManager.AddScoreP2(1);
            UpdatePlayerKills(P2); // Track kills for P2
        }
        else if (player == P2)
        {
            gameManager.AddScoreP1(1);
            UpdatePlayerKills(P1); // Track kills for P1
        }

        // Log the round duration
        LogRoundDuration();

        // Respawn both players
        Respawn(P1, P1Spawn);
        Respawn(P2, P2Spawn);

        // Notify the GameManager to end the round
        gameManager.EndRound();

        // Start a new round
        StartNewRound();
    }

    private void UpdatePlayerKills(GameObject player)
    {
        // Check if the player is already in the dictionary
        if (!playerKills.ContainsKey(player))
        {
            playerKills[player] = 0; // Initialize kills to 0 if not present
        }

        playerKills[player]++; // Increment the player's kill count
        Debug.Log(player.name + " has " + playerKills[player] + " kills.");

        // Check if the player has reached 5 kills
        if (playerKills[player] >= 5)
        {
            StartCoroutine(EndGame(player)); // Start the EndGame coroutine
        }
    }

    private IEnumerator EndGame(GameObject winner)
    {
        Debug.Log(winner.name + " wins the game!");
        // Display the victory message
        victoryText.text = winner.name + " wins the game!";
        victoryText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2); // Wait for 2 seconds before restarting the scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Restart the scene
    }

    public void Respawn(GameObject player, Vector2 spawnPosition)
    {
        Debug.Log("Respawn");
        player.SetActive(true);
        player.transform.position = spawnPosition;
        player.transform.rotation = Quaternion.identity;
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0;
    }

    private void LogRoundDuration()
    {
        float roundDuration = Time.time - roundStartTime; // Calculate the round duration
        Dictionary<string, object> parameters = new Dictionary<string, object>
        {
            { "RoundDuration", roundDuration }
        };
        Abertay.Analytics.AnalyticsManager.SendCustomEvent("RoundEnded", parameters); // Log the round duration
    }

    private void StartNewRound()
    {
        roundStartTime = Time.time; // Record the start time of the round
    }

    private IEnumerator DelayDeath(GameObject player)
    {
        yield return new WaitForSeconds(2.0f); // Wait for 2 seconds before marking the player as dead
        player.SetActive(false);
        PlayerDied(player); // Player death
    }
}
