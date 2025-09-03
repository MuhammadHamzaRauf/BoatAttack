using UnityEngine;
using UnityEngine.UI;
using GangsterMafia.Core;

/// <summary>
/// Test helper script for the initial cut scene functionality
/// This can be attached to any GameObject in the scene for testing purposes
/// </summary>
public class CutSceneTestHelper : MonoBehaviour
{
    [Header("Test Controls")]
    [SerializeField] private Button resetCutSceneButton;
    [SerializeField] private Button checkCutSceneStateButton;
    [SerializeField] private Text statusText;

    private void Start()
    {
        // Set up test buttons if they exist
        if (resetCutSceneButton != null)
        {
            resetCutSceneButton.onClick.AddListener(ResetCutSceneState);
        }
        
        if (checkCutSceneStateButton != null)
        {
            checkCutSceneStateButton.onClick.AddListener(CheckCutSceneState);
        }
        
        // Update status text
        UpdateStatusText();
    }

    /// <summary>
    /// Reset the initial cut scene state for testing
    /// </summary>
    public void ResetCutSceneState()
    {
        GameManager.Instance.ResetInitialCutsceneState();
        UpdateStatusText();
        Debug.Log("Cut Scene Test: Initial cut scene state has been reset!");
    }

    /// <summary>
    /// Check the current cut scene state
    /// </summary>
    public void CheckCutSceneState()
    {
        bool hasPlayed = GameManager.Instance.HasPlayedInitialCutscene();
        Debug.Log($"Cut Scene Test: Has played initial cut scene: {hasPlayed}");
        UpdateStatusText();
    }

    /// <summary>
    /// Update the status text display
    /// </summary>
    private void UpdateStatusText()
    {
        if (statusText != null)
        {
            bool hasPlayed = GameManager.Instance.HasPlayedInitialCutscene();
            statusText.text = $"Cut Scene State: {(hasPlayed ? "Played" : "Not Played")}";
        }
    }

    /// <summary>
    /// Force mark cut scene as played
    /// </summary>
    public void ForceMarkAsPlayed()
    {
        GameManager.Instance.MarkInitialCutsceneAsPlayed();
        UpdateStatusText();
        Debug.Log("Cut Scene Test: Initial cut scene has been marked as played!");
    }

    /// <summary>
    /// Get current level from GameManager
    /// </summary>
    public void LogCurrentLevel()
    {
        int currentLevel = GameManager.Instance.CurrentLevel;
        Debug.Log($"Cut Scene Test: Current selected level: {currentLevel}");
    }
}
