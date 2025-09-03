using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using GangsterMafia.Core;
using GangsterMafia.Sounds;

public class MainMenuHandler : MonoBehaviour
{

    public static MainMenuHandler instance;
    [Header("Panels")]
    public GameObject mainPanel;
    public GameObject setting;
    public GameObject mODESelectionPanel;
    public GameObject levelSelectionPanel;
    public GameObject loadingPanel;
    public GameObject exitPanel;
    public GameObject CharacterSelection;
    public GameObject carSelection;
    public GameObject dailyRewardPanel;
    public GameObject shop;
    public GameObject Profile;


    [Header("Text")]
    public Text coins;
    public Text gems;


    public string rateus;
    public string Privacy;


    public GameObject congratulationPanel;




    public GameObject PurchaseSuceess;

    public GameObject PurchaseFail;

    public GameObject internetIssue;

    [Header("RemoveAds")]
    public GameObject removeAdsButton;
    public GameObject ShopButton; 


    public GameObject congratulationDailyReward;


    public Canvas Canvas;
    public Text Name;


    public GameObject garage;

   public void OnclickShopButton()
    {
        mainPanel.SetActive(false);
        shop.SetActive(true);
        ShopManager.instance.coins.text = GameManager.Instance.TotalCoins.ToString();
        ShopManager.instance.gems.text = GameManager.Instance.TotalGems.ToString();
        SoundManager.Instance.PlayEffect(AudioClipsSource.Instance.BtnClick);
    }

   
    public void OnClickProfile()
    {
        mainPanel.SetActive(false);
        Profile.SetActive(true);
        SoundManager.Instance.PlayEffect(AudioClipsSource.Instance.BtnClick);
    }

    public void playremoveAdsCoroutine()
    {
        StartCoroutine(adremoved());
    }


    IEnumerator adremoved()
    {
        PurchaseSuceess.SetActive(true);
        yield return new WaitForSeconds(3f);
        PurchaseSuceess.SetActive(false);
        PlayerPrefs.SetInt("RemoveAds", 1);
    }


    public void InternetIssue()
    {
        StartCoroutine(internet());
    }

    IEnumerator internet()
    {
        internetIssue.SetActive(true);
        yield return new WaitForSeconds(3);
        internetIssue.SetActive(false);
    }


    public void PurchaseFAil()
    {
        StartCoroutine(IAPCancel());
    }

    IEnumerator IAPCancel()
    {
        PurchaseFail.SetActive(true);
        yield return new WaitForSeconds(3);
        PurchaseFail.SetActive(false);
    }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {

        if (PlayerPrefs.GetFloat("StartMusicVolume") == 0 || PlayerPrefs.GetFloat("StartSoundVolume") == 0)
        {
            PlayerPrefs.SetFloat("MusicVolume", 1);
            PlayerPrefs.SetFloat("StartMusicVolume", 1);
            PlayerPrefs.SetFloat("SoundVolume", 1);
            PlayerPrefs.SetFloat("StartSoundVolume", 1);
        }
      
            OnStartMainMenu();
             SoundManager.Instance._BGAudioSource.volume = PlayerPrefs.GetFloat("MusicVolume");
             SoundManager.Instance._FGAudioSource.volume = PlayerPrefs.GetFloat("SoundVolume");

        SettingHandler.instance.musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        SettingHandler.instance.soundSlider.value = PlayerPrefs.GetFloat("SoundVolume");
        SoundManager.Instance.PlayBackgroundMusic(AudioClipsSource.Instance.MainMenuClip);
        
    }
    public void OnClickNextButton()
    {
        garage.SetActive(true);
        mainPanel.SetActive(false);
        carSelection.SetActive(true);
        CarSelection.instance.coins.text = GameManager.Instance.TotalCoins.ToString();
        CarSelection.instance.gems.text = GameManager.Instance.TotalGems.ToString();
        SoundManager.Instance.PlayEffect(AudioClipsSource.Instance.PlayBtn);
    }
    public void OnClickSettingButton()
    {
        setting.SetActive(true);
        mainPanel.SetActive(false);
        SoundManager.Instance.PlayEffect(AudioClipsSource.Instance.BtnClick);
    }

    public void OnClickExitButton()
    {
        mainPanel.SetActive(false);
        exitPanel.SetActive(true);
        SoundManager.Instance.PlayEffect(AudioClipsSource.Instance.BtnClick);
    }


    public void OnClickExitNoButton()
    {
        exitPanel.SetActive(false);
        mainPanel.SetActive(true);
        SoundManager.Instance.PlayEffect(AudioClipsSource.Instance.BtnClick);
    }


    public void OnClickDailyRewardButton()
    {
        dailyRewardPanel.SetActive(true);
        mainPanel.SetActive(false);
        DailyRewardSystem.instance.coins.text = GameManager.Instance.TotalCoins.ToString();
        DailyRewardSystem.instance.gems.text = GameManager.Instance.TotalGems.ToString();
        SoundManager.Instance.PlayEffect(AudioClipsSource.Instance.BtnClick);
    }

    public void ShowRewardedAdForCoins()
    {
       RewardedCoins();
    }

    public void onclickFreeCoinsButton()
    {
        SoundManager.Instance.PlayEffect(AudioClipsSource.Instance.BtnClick);
    }

    public void RewardedCoins()
    {
      
        GameManager.Instance.TotalCoins += 500;
        coins.text = GameManager.Instance.TotalCoins.ToString();
        GameManager.Instance.SaveTotalCoins();
        StartCoroutine(ShowRewardePanel());
        
        SoundManager.Instance.PlayEffect(AudioClipsSource.Instance.Coins);
    }


    private void OnStartMainMenu()
    {
        UpdateCoinsandCAsh();
            if (PlayerPrefs.GetInt("UnlockEverThing") == 1)
            {
                removeAdsButton.SetActive(false);
                ShopButton.SetActive(false);
            }
            else
            {
               removeAdsButton.SetActive(true);
               ShopButton.SetActive(true);
            }

            if (PlayerPrefs.GetInt("RemoveAds") == 1)
            {
                removeAdsButton.SetActive(false);
            }
            else
            {
                removeAdsButton.SetActive(true);
            }

        if (PlayerPrefs.GetInt("Profile") == 1)
            mainPanel.SetActive(true);
        else Profile.SetActive(true);

        Name.text = ProfileSelection.instance.playerName;
       SoundManager.Instance.PlayBackgroundMusic(AudioClipsSource.Instance.MainMenuClip);
    }

    public void onclickRemoveAdButton()
    {
     //   IAPPurchaserManager.Instance.ConvertToLocal();
      //  removeAdsPanel.SetActive(true);
        SoundManager.Instance.PlayEffect(AudioClipsSource.Instance.BtnClick);
    }

   


    public void OnCLickExitYes()
    {
        Application.Quit();
    }



    public void OpenRateUSLink()
    {
        Application.OpenURL(rateus);
        SoundManager.Instance.PlayEffect(AudioClipsSource.Instance.BtnClick);
    }
    public void PrivacyLink()
    {
        Application.OpenURL(Privacy);
           SoundManager.Instance.PlayEffect(AudioClipsSource.Instance.BtnClick);
    }




    public void UpdateCoinsandCAsh()
    {
        coins.text = GameManager.Instance.TotalCoins.ToString();
        gems.text = GameManager.Instance.TotalGems.ToString();
    }




    IEnumerator ShowRewardePanel()
    {
        congratulationPanel.SetActive(true);
        yield return new WaitForSeconds(3f);
        congratulationPanel.SetActive(false);
        mainPanel.SetActive(true);
    }




   

    public void buyEverThing()
    {

    }

    public void RemoveAdsFunction()
    {
        StartCoroutine(activeUnlockEverthing());
        ShopButton.SetActive(false);
        removeAdsButton.SetActive(false);
        PlayerPrefs.SetInt("RemoveAds", 1);
    }



    IEnumerator activeUnlockEverthing()
    {
        PurchaseSuceess.SetActive(true);
        yield return new WaitForSeconds(3f);
        PurchaseSuceess.SetActive(false);
    }

}
