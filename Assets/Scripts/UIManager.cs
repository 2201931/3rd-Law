using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text instructionText; // Reference to the UI Text element

    public void SetInstructionText(string message)
    {
        if (instructionText != null)
        {
            instructionText.text = message;
        }
    }
}
