using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Add this to manage scenes
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{
    private UIDocument _document;
    private Button _button;

    private void Awake()
    {
        // Get the UIDocument component
        _document = GetComponent<UIDocument>();

        // Find the button named "Start" in the UI
        _button = _document.rootVisualElement.Q<Button>("Start");

        // Register the click event callback
        if (_button != null)
        {
            _button.RegisterCallback<ClickEvent>(OnPlayGameClick);
        }
        else
        {
            Debug.LogError("Start button not found!");
        }
    }

    private void OnDisable()
    {
        // Unregister the click event callback
        if (_button != null)
        {
            _button.UnregisterCallback<ClickEvent>(OnPlayGameClick);
        }
    }

    private void OnPlayGameClick(ClickEvent evt)
    {
        // Load the MainScreen scene
        SceneManager.LoadScene("MainScreen");
    }
}
