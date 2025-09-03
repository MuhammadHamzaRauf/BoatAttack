using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using GangsterMafia.Core;
using GangsterMafia.Sounds;

public class InitialCutScene : MonoBehaviour
{
    private void Start()
    {
        SoundManager.Instance.StopMusic(AudioClipsSource.Instance.MainMenuClip);
    }
    
    public void LoadScene()
    {
        // After the cut scene completes, load the level that was selected
        // The level number is stored in GameManager.Instance.CurrentLevel
        
        Debug.Log($"InitialCutScene: Cut scene completed. Loading level {GameManager.Instance.CurrentLevel}");
        
        // Use the custom SceneManager to load the gameplay scene
        GangsterMafia.Core.SceneManager.Instance.LoadScene("GamePlay");
    }
}
