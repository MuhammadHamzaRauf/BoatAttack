using System.Collections;
using System.Collections.Generic;
using GangsterMafia.Core;
using UnityEngine;
using UnityEngine.UI;
using GangsterMafia.Sounds;

public class ShopManager : MonoBehaviour
{
    public static ShopManager instance;

    private void Awake()
    {
        instance = this;
    }

    [Header("Coins")]
    public Text coins;
    public Text gems;


    private void Start()
    {
        coins.text = GameManager.Instance.TotalCoins.ToString();
        gems.text = GameManager.Instance.TotalGems.ToString();
    }

    [Header("MiniPanels")]
    public GameObject CoinsPanel;
    public GameObject GemsPanel;
    public GameObject UnlockAllPanel;
    public GameObject UnlockCarsPanel;
 //   public GameObject CoinsOnAd;

    [Header("Buttons")]
    public GameObject CoinsButton;
    public GameObject GemsButton;
    public GameObject UnlockAllButton;
    public GameObject UnlockCarsButton;
    public GameObject CoinsOnButton;


    public void OnShopBack()
    {
        MainMenuHandler.instance.shop.SetActive(false);
        MainMenuHandler.instance.mainPanel.SetActive(true);
        MainMenuHandler.instance.coins.text = GameManager.Instance.TotalCoins.ToString();
        MainMenuHandler.instance.gems.text = GameManager.Instance.TotalGems.ToString();
        SoundManager.Instance.PlayEffect(AudioClipsSource.Instance.BackBtn);
    }

    #region Button Selection
    public void OnClickCoinsButton() => SetPanelAndButton(CoinsPanel, CoinsButton);
    public void OnClickGemsButton() => SetPanelAndButton(GemsPanel, GemsButton);
    public void OnClickUnlockAllButton() => SetPanelAndButton(UnlockAllPanel, UnlockAllButton);
    public void OnClickUnlockCarsButton() => SetPanelAndButton(UnlockCarsPanel, UnlockCarsButton);
    public void OnClickCoinsOnAdButton()
    {
        WatchAdCoins();
    }

    private void SetPanelAndButton(GameObject panel, GameObject button)
    {
        SetActivePanel(panel);
        SetSelectedButton(button);
    }

    private void SetActivePanel(GameObject activePanel)
    {
        CoinsPanel.SetActive(false);
        GemsPanel.SetActive(false);
        UnlockAllPanel.SetActive(false);
        UnlockCarsPanel.SetActive(false);
     //   CoinsOnAd.SetActive(false);

        if (activePanel != null)
            activePanel.SetActive(true);
    }

    private void SetSelectedButton(GameObject selectedButton)
    {
        CoinsButton.GetComponent<Image>().color = Color.white;
        GemsButton.GetComponent<Image>().color = Color.white;
        UnlockAllButton.GetComponent<Image>().color = Color.white;
        UnlockCarsButton.GetComponent<Image>().color = Color.white;
        CoinsOnButton.GetComponent<Image>().color = Color.white;

        if (selectedButton != null)
            selectedButton.GetComponent<Image>().color = Color.yellow;
    }
    #endregion

    #region Currency Add Functions

    public void AddCoinsByIndex(int index)
    {
        int amount = (index + 1) * 1000;
        GameManager.Instance.TotalCoins += amount;
        UpdateCurrencyUI();
        InAppPurchase();
    }

    public void AddGemsByIndex(int index)
    {
        int[] gemValues = { 50, 60, 100, 150, 200, 230 };
        if (index >= 0 && index < gemValues.Length)
        {
            GameManager.Instance.TotalGems += gemValues[index];
            UpdateCurrencyUI();
            InAppPurchase();
        }
    }

    public void BuyAll()
    {
        PlayerPrefs.SetInt("UnlockEverThing", 1);
        PlayerPrefs.SetInt("RemoveAds", 1);
        CarSelection.instance.UnLockAllCars();
        MainMenuHandler.instance.ShopButton.SetActive(false);
        MainMenuHandler.instance.removeAdsButton.SetActive(false);
        UpdateCurrencyUI();
        InAppPurchase();
    }

    public void BuyCars()
    {
        CarSelection.instance.UnLockAllCars();
        UpdateCurrencyUI();
        InAppPurchase();
    }

    public void WatchAdCoins()
    {
        GameManager.Instance.TotalCoins += 500;
        coins.text = GameManager.Instance.TotalCoins.ToString();
        GameManager.Instance.SaveTotalCoins();
        StartCoroutine(ShowRewardePanel());

        SoundManager.Instance.PlayEffect(AudioClipsSource.Instance.Coins);
        UpdateCurrencyUI();
        InAppPurchase();
    }

    private void UpdateCurrencyUI()
    {
        coins.text = GameManager.Instance.TotalCoins.ToString();
        gems.text = GameManager.Instance.TotalGems.ToString();
        GameManager.Instance.SaveTotalCoins();
    }

    #endregion

    #region Purchase Popup

    public void InAppPurchase()
    {
        StartCoroutine(PopUpSuccess());
    }

    IEnumerator PopUpSuccess()
    {
        MainMenuHandler.instance.PurchaseSuceess.SetActive(true);
        yield return new WaitForSeconds(3f);
        MainMenuHandler.instance.PurchaseSuceess.SetActive(false);
        MainMenuHandler.instance.shop.SetActive(false);
        MainMenuHandler.instance.mainPanel.SetActive(true);
        MainMenuHandler.instance.coins.text = GameManager.Instance.TotalCoins.ToString();
        MainMenuHandler.instance.gems.text = GameManager.Instance.TotalGems.ToString();
        UpdateCurrencyUI();
    }

    IEnumerator ShowRewardePanel()
    {
       MainMenuHandler.instance.congratulationPanel.SetActive(true);
        yield return new WaitForSeconds(3f);
       MainMenuHandler.instance.congratulationPanel.SetActive(false);
       MainMenuHandler.instance.mainPanel.SetActive(true);
        MainMenuHandler.instance.shop.SetActive(false);
        UpdateCurrencyUI();
    }

    #endregion
}
