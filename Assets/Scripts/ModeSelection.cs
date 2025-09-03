using GangsterMafia.Constants;
using GangsterMafia.Core;
using UnityEngine;
using GangsterMafia.Sounds;

public class ModeSelection : MonoBehaviour
{
   
    public void OpenWorld()
    {
        GameManager.Instance.CurrentMode = 0;
        MainMenuHandler.instance.mODESelectionPanel.SetActive(false);
        MainMenuHandler.instance.levelSelectionPanel.SetActive(true);
        SoundManager.Instance.PlayEffect(AudioClipsSource.Instance.BtnClick);
    }

    public void ChaseMode()
    {
        GameManager.Instance.CurrentMode = 1;
        MainMenuHandler.instance.mODESelectionPanel.SetActive(false);
        MainMenuHandler.instance.loadingPanel.SetActive(true);
        SoundManager.Instance.PlayEffect(AudioClipsSource.Instance.BtnClick);
        SceneManager.Instance.LoadScene(GameConstants.GAMEPLAY_SCENE);
    }


    public void onLcickBackButton()
    {
        MainMenuHandler.instance.mODESelectionPanel.SetActive(false);
        MainMenuHandler.instance.CharacterSelection.SetActive(true);
        SoundManager.Instance.PlayEffect(AudioClipsSource.Instance.BtnClick);
        MainMenuHandler.instance.Canvas.renderMode = RenderMode.ScreenSpaceCamera;
    }
}
