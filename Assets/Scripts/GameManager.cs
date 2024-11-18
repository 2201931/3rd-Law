using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public Text countdownText;
    public Text scoreTextP1;
    public Text scoreTextP2;
    public GameObject menu; // Reference to your menu GameObject
    private int scoreP1;
    private int scoreP2;
    private bool inputsAllowed;
    private bool isShowScoreRunning;
    private bool isCountdownRunning; // Add this variable

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Initialize scores
        scoreP1 = 0;
        scoreP2 = 0;
        UpdateScoreText();

        // Deactivate the menu at the start of the game
        menu.SetActive(false);
        StartCoroutine(StartCountdown());
    }

    IEnumerator StartCountdown()
    {
        if (isCountdownRunning) yield break; // Prevent multiple countdowns
        isCountdownRunning = true; // Set the flag

        inputsAllowed = false; // Disable inputs during countdown

        countdownText.gameObject.SetActive(true); // Ensure countdownText is visible
        RectTransform countdownRect = countdownText.GetComponent<RectTransform>();
        countdownRect.anchoredPosition = new Vector2(0, 0); // Center position
        countdownRect.localScale = Vector3.one; // Reset scale to original size

        countdownText.text = "3";
        yield return new WaitForSeconds(1);
        countdownText.text = "2";
        yield return new WaitForSeconds(1);
        countdownText.text = "1";
        yield return new WaitForSeconds(1);
        countdownText.text = "FIGHT!";
        yield return new WaitForSeconds(1);

        inputsAllowed = true; // Enable inputs after countdown
        StartCoroutine(ShrinkAndMove());

        isCountdownRunning = false; // Reset the flag
    }

    IEnumerator ShrinkAndMove()
    {
        RectTransform countdownRect = countdownText.GetComponent<RectTransform>();
        Vector2 targetPosition = new Vector2(0, Screen.height / 2 + 50);
        float duration = 1.0f;
        Vector2 startPosition = countdownRect.anchoredPosition;
        Vector3 startScale = countdownRect.localScale;
        Vector3 endScale = new Vector3(0.5f, 0.5f, 0.5f);

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            countdownRect.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, t / duration);
            countdownRect.localScale = Vector3.Lerp(startScale, endScale, t / duration);
            yield return null;
        }

        countdownRect.anchoredPosition = targetPosition;
        countdownRect.localScale = endScale;
    }

    public void EndRound()
    {
        // Update the score display at the end of the round
        UpdateScoreText();
        countdownText.gameObject.SetActive(false); // Hide countdownText when the round ends
        StartCoroutine(ShowScore());
    }

    IEnumerator ShowScore()
    {
        if (isShowScoreRunning) yield break;
        isShowScoreRunning = true; // Set the flag

        inputsAllowed = false;

        // Reset score texts to near center to avoid overlap
        RectTransform scoreRectP1 = scoreTextP1.GetComponent<RectTransform>();
        RectTransform scoreRectP2 = scoreTextP2.GetComponent<RectTransform>();
        scoreRectP1.anchoredPosition = new Vector2(-Screen.width / 4, 0); // Slightly left of center
        scoreRectP2.anchoredPosition = new Vector2(Screen.width / 4, 0); // Slightly right of center
        scoreRectP1.localScale = Vector3.one;
        scoreRectP2.localScale = Vector3.one;

        scoreTextP1.gameObject.SetActive(true);
        scoreTextP2.gameObject.SetActive(true);

        yield return new WaitForSeconds(2);

        // Move scores to their respective corners and pulse if they are the winning score
        StartCoroutine(MoveToCornerAndPulse(scoreRectP1, new Vector2(-450, Screen.height / 2 - 50), scoreP1 > scoreP2));
        StartCoroutine(MoveToCornerAndPulse(scoreRectP2, new Vector2(Screen.width / 2 - 100, Screen.height / 2 - 50), scoreP2 > scoreP1));

        yield return new WaitForSeconds(1); // Wait for the move animation to complete

        StartCoroutine(StartCountdown());

        isShowScoreRunning = false; // Reset the flag
    }

    IEnumerator MoveToCornerAndPulse(RectTransform rectTransform, Vector2 targetPosition, bool shouldPulse)
    {
        float duration = 1.0f;
        Vector2 startPosition = rectTransform.anchoredPosition;

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, t / duration);
            yield return null;
        }

        rectTransform.anchoredPosition = targetPosition;

        if (shouldPulse)
        {
            yield return PulseText(rectTransform, 1.0f); // Pulse for 1 second
        }
    }

    IEnumerator PulseText(RectTransform rectTransform, float pulseDuration)
    {
        float duration = 0.5f;
        Vector3 originalScale = rectTransform.localScale;
        Vector3 targetScale = originalScale * 1.5f;

        float elapsedTime = 0f;

        while (elapsedTime < pulseDuration)
        {
            // Scale up
            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                rectTransform.localScale = Vector3.Lerp(originalScale, targetScale, t / duration);
                yield return null;
            }

            // Scale down
            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                rectTransform.localScale = Vector3.Lerp(targetScale, originalScale, t / duration);
                yield return null;
            }

            elapsedTime += duration * 2; // Account for both scale up and scale down
        }

        rectTransform.localScale = originalScale; // Ensure it ends at the original scale
    }

    public void AddScoreP1(int points)
    {
        scoreP1 += points;
        UpdateScoreText();
    }

    public void AddScoreP2(int points)
    {
        scoreP2 += points;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        scoreTextP1.text = "P1: " + scoreP1.ToString();
        scoreTextP2.text = "P2: " + scoreP2.ToString();
    }

    // Method to be called when the Start button is clicked
    public void OnStartButtonClick()
    {
        menu.SetActive(true);
        StartCoroutine(StartCountdown());
    }

    public bool AreInputsAllowed()
    {
        return inputsAllowed;
    }
}
