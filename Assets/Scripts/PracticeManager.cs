using UnityEngine;

public class PracticeManager : MonoBehaviour
{
    public static PracticeManager Instance { get; private set; }

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

    // Add methods and properties specific to practice mode
    public void InitializePracticeMode()
    {
        Debug.Log("Practice mode initialized.");
        // Add any initialization logic for practice mode here
    }

    // Example method to disable scoring in practice mode
    public void DisableScoring()
    {
        Debug.Log("Scoring is disabled in practice mode.");
        // Add logic to disable scoring
    }
}
