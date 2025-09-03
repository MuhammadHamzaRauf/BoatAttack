using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using GangsterMafia.Core;
using GangsterMafia.Sounds;

public class CharacterSelection : MonoBehaviour
{
    public static CharacterSelection instance;
    public Gangsters[] gangster;
    public Image JeepPrice;

    public Text coins;
    public Text gems;
    public Text jeepPriceText;

    public GameObject buy;
    public GameObject select;

    public int currentGangster;

    public Text playerName;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        UnLockAllCars();        
    }

    public void OpenCArSelection()
    {

        LoadGangsterData();
        UpdateUI();
        coins.text = GameManager.Instance.TotalCoins.ToString();
        gems.text = GameManager.Instance.TotalGems.ToString();
    }


    public void SelectGangsterByIndex(int index)
    {
        if (index < 0 || index >= gangster.Length)
        {
            Debug.LogWarning("Invalid car index selected: " + index);
            return;
        }

        foreach (var car in gangster)
        {
            car.Character.SetActive(false);
        }

        currentGangster = index;
        GameManager.Instance.CurrentGangster = currentGangster;
        gangster[currentGangster].Character.SetActive(true);
        playerName.text = gangster[currentGangster].playername.ToString();
        UpdateUI();
    }

    public void OnClickBuyButton()
    {
        int price = GetWeaponCost();

        if (GameManager.Instance.TotalCoins >= price)
        {
            GameManager.Instance.TotalCoins -= price;
            PlayerPrefs.SetInt("Gangster_" + currentGangster, 1);
            PlayerPrefs.Save();
            gangster[currentGangster].isLook = true;
            PlayerPrefs.SetInt("TotalCoins", GameManager.Instance.TotalCoins);

            if (gangster[currentGangster].LockImage != null)
                gangster[currentGangster].LockImage.gameObject.SetActive(false);


            UpdateUI();
            StartCoroutine(PurchaseSuccessfully());
        }
        else
        {
            StartCoroutine(CantPurches());
        }
    }

    public void OnClickSelectButton()
    {
        //       GameManager.instance.currentGangster = currentGangster;
        //     gangster[GameManager.instance.currentGangster].Character.SetActive(false);
        MainMenuHandler.instance.CharacterSelection.SetActive(false);
        MainMenuHandler.instance.mODESelectionPanel.SetActive(true);
        MainMenuHandler.instance.Canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        SoundManager.Instance.PlayEffect(AudioClipsSource.Instance.PlayBtn);
    }


    public void LoadGangsterData()
    {
        for (int i = 0; i < gangster.Length; i++)
        {
            if (i == 0)
            {
                gangster[i].isLook = true;
                PlayerPrefs.SetInt("Gangster_" + i, 1); 
                if (gangster[i].LockImage != null)
                    gangster[i].LockImage.gameObject.SetActive(false);
                continue;
            }

            if (PlayerPrefs.GetInt("Gangster_" + i, 0) == 1)
            {
                gangster[i].isLook = true;
                if (gangster[i].LockImage != null)
                    gangster[i].LockImage.gameObject.SetActive(false);
            }
            else
            {
                if (gangster[i].LockImage != null)
                    gangster[i].LockImage.gameObject.SetActive(true);
            }
        }

        currentGangster = PlayerPrefs.GetInt("Gangster_", 0);
        gangster[currentGangster].Character.SetActive(true);
        coins.text = GameManager.Instance.TotalCoins.ToString();
        gems.text = GameManager.Instance.TotalGems.ToString();
    }


    public void UpdateUI()
    {
        if (coins != null)
        {
            coins.text = GameManager.Instance.TotalCoins.ToString();
        }
        int weaponCost = GetWeaponCost();
        jeepPriceText.text = weaponCost.ToString();

        if (gangster[currentGangster].isLook)
        {
            buy.SetActive(false);
            select.SetActive(true);
            JeepPrice.gameObject.SetActive(false);
        }
        else
        {
            buy.SetActive(true);
            select.SetActive(false);
            JeepPrice.gameObject.SetActive(true);
        }



        for (int i = 0; i < gangster.Length; i++)
        {
            gangster[i].Character.SetActive(i == currentGangster);
        }

        if (currentGangster == 0)
        {
            JeepPrice.gameObject.SetActive(false);
        }

    }




    private int GetWeaponCost()
    {
        return gangster[currentGangster].G_Price;
    }

    public void OnclickBAckButton()
    {
        MainMenuHandler.instance.garage.SetActive(true);
        CarSelection.instance.cars[CarSelection.instance.currentCar].jeep.SetActive(true);
        MainMenuHandler.instance.CharacterSelection.SetActive(false);
        MainMenuHandler.instance.carSelection.SetActive(true);
        CarSelection.instance.coins.text = GameManager.Instance.TotalCoins.ToString();
        CarSelection.instance.gems.text = GameManager.Instance.TotalGems.ToString();
        CarSelection.instance.OpenCArSelection();
        SoundManager.Instance.PlayEffect(AudioClipsSource.Instance.BackBtn);
        MainMenuHandler.instance.Canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        //  LoadGangsterData();

    }


    public GameObject purchasePanel;
    public GameObject CantpurchasePanel;

    IEnumerator PurchaseSuccessfully()
    {
        MainMenuHandler.instance.Canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        purchasePanel.transform.DOScale(1, .3f);
        purchasePanel.SetActive(true);
        yield return new WaitForSeconds(3f);
        MainMenuHandler.instance.Canvas.renderMode = RenderMode.ScreenSpaceCamera;
        purchasePanel.transform.DOScale(0, .3f);

        purchasePanel.SetActive(false);
    }

    IEnumerator CantPurches()
    {
        MainMenuHandler.instance.Canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CantpurchasePanel.transform.DOScale(1, .3f);
        CantpurchasePanel.SetActive(true);
        yield return new WaitForSeconds(3f);
        MainMenuHandler.instance.Canvas.renderMode = RenderMode.ScreenSpaceCamera;
        CantpurchasePanel.transform.DOScale(0, .3f);
        CantpurchasePanel.SetActive(false);
    }



    public void UnLockAllCars()
    {
        for (int i = 0; i < gangster.Length; i++)
        {
            gangster[i].isLook = true;

            PlayerPrefs.SetInt("Gangster_" + i, 1);
            if (gangster[i].LockImage != null)
                gangster[i].LockImage.gameObject.SetActive(false);
        }

        PlayerPrefs.Save();

        UpdateUI();
    }



    private bool isDragging = false;
    private Vector2 lastPointerPosition;
    private float rotationSpeed = 0.5f;


    public void PointerDownRotationPlay()
    {
        isDragging = true;
        lastPointerPosition = Input.mousePosition;
    }

    public void PointerUpRotationStop()
    {
        isDragging = false;
    }

    private void Update()
    {
        if (isDragging && gangster.Length > 0)
        {
            Vector2 currentPointerPosition = Input.mousePosition;
            float deltaX = currentPointerPosition.x - lastPointerPosition.x;

            if (gangster[currentGangster].dummy != null)
            {
                gangster[currentGangster].dummy.transform.Rotate(0, -deltaX * rotationSpeed, 0, Space.Self);
            }

            lastPointerPosition = currentPointerPosition;
        }
    }



    [System.Serializable]
    public class Gangsters
    {
        public bool isLook;
        public int G_Price;
        public GameObject Character;
        public Image LockImage;
        public GameObject dummy;
        public string playername;
    }

}
