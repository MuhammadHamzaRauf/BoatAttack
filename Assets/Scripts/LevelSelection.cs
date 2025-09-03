using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GangsterMafia.Constants;

public class LevelSelection : MonoBehaviour
{
    public static LevelSelection instance;
    public Button[] lvlButtons;
    public Text[] levelTextStatus;  
    public GameObject[] GlowStatus;
    public GameObject[] lockImage;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        // UnlockAllLevels();
        int levelAt = PlayerPrefs.GetInt("levelAt", 0); 
        Debug.Log(levelAt);
        UpdateLevelButtons(levelAt);
    }

    public void UpdateLevelButtons(int levelAt)
    {
        int highestUnlockedLevel = levelAt;

        for (int i = 0; i < lvlButtons.Length; i++)
        {
            bool isUnlocked = i <= highestUnlockedLevel;

            lvlButtons[i].enabled = isUnlocked;

            if (isUnlocked)
            {
                GlowStatus[i].SetActive(false);
                lockImage[i].SetActive(false);

                if (i < highestUnlockedLevel)
                {
                    levelTextStatus[i].gameObject.SetActive(true);
                    GlowStatus[i].SetActive(false);
                }
                else if (i == highestUnlockedLevel)
                {
                    levelTextStatus[i].gameObject.SetActive(true);
                    GlowStatus[i].SetActive(true);
                }
            }
            else
            {
                levelTextStatus[i].gameObject.SetActive(false);
                lvlButtons[i].enabled = false;
                GlowStatus[i].SetActive(false);
                lockImage[i].SetActive(true);
            }
        }
    }

    public void SelectLevel(int LevelNo)
    {
        GameManager.Instance.CurrentLevel = LevelNo;
        Debug.Log($"LevelSelection: Level {LevelNo} selected. Current level set to: {GameManager.Instance.CurrentLevel}");
        
        // Check if this is the first time playing any level
        if (ShouldPlayInitialCutscene())
        {
            Debug.Log("LevelSelection: First time playing - will show initial cut scene");
            
            // Mark that cut scene will be played
            GameManager.Instance.MarkInitialCutsceneAsPlayed();
            
            // Load the initial cut scene
            MainMenuHandler.instance.levelSelectionPanel.SetActive(false);
            MainMenuHandler.instance.loadingPanel.SetActive(true);
            
            // Use the custom SceneManager to load the cut scene
            GangsterMafia.Core.SceneManager.Instance.LoadScene(GameConstants.INITIALCUT_SCENE);
        }
        else
        {
            Debug.Log("LevelSelection: Not first time - loading gameplay directly");
            
            // Normal level selection flow
            MainMenuHandler.instance.levelSelectionPanel.SetActive(false);
            MainMenuHandler.instance.loadingPanel.SetActive(true);
            
            // Load the gameplay scene directly
            GangsterMafia.Core.SceneManager.Instance.LoadScene(GameConstants.GAMEPLAY_SCENE);
        }
    }

    private bool ShouldPlayInitialCutscene()
    {
        // Check if the initial cut scene has never been played
        return !GameManager.Instance.HasPlayedInitialCutscene();
    }

    public void Back()
    {
     //   StartCoroutine(ShowIntersttial()); // Interstitial

        MainMenuHandler.instance.levelSelectionPanel.SetActive(false);
        MainMenuHandler.instance.mODESelectionPanel.SetActive(true);
    }

    public void UnlockAllLevels()
    {
        int totalLevels = lvlButtons.Length; 

        PlayerPrefs.SetInt("levelAt", totalLevels - 1);

        UpdateLevelButtons(totalLevels - 1);
    //    unlockLevelsPopUp.SetActive(false);
        PlayerPrefs.SetInt("Buy_Levels", 1);
        StartCoroutine(PopUpSuccess());
    }
   
    IEnumerator PopUpSuccess()
    {
      //  POPUP.SetActive(true);
        yield return new WaitForSeconds(3f);
      //  POPUP.SetActive(false);
    }
}
