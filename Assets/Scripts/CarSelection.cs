using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using GangsterMafia.Constants;
using GangsterMafia.Core;
using GangsterMafia.Sounds;

public class CarSelection : MonoBehaviour
{
    public static CarSelection instance;
    public Cars[] cars;
    public Image fill_speed;
    public Image fill_acceleration;
    public Image fill_engine;
    public Image fill_break;
    public Image JeepPrice;

    public Text coins;
    public Text gems;
    public Text jeepPriceText;

    public GameObject buy;
    public GameObject select;

    public int currentCar;

    public Text CarName;

    private void Awake()
    {
        instance = this;
    }




    private void Start()
    {
        LoadJeepData();
        UnLockAllCars();
    }
    public void OpenCArSelection()
    {

        LoadJeepData();
        UpdateUI();
        coins.text = GameManager.Instance.TotalCoins.ToString();
        gems.text = GameManager.Instance.TotalGems.ToString();
    }


    public void SelectCarByIndex(int index)
    {
        if (index < 0 || index >= cars.Length)
        {
            Debug.LogWarning("Invalid car index selected: " + index);
            return;
        }

        foreach (var car in cars)
        {
            car.jeep.SetActive(false);
        }

        currentCar = index;
        GameManager.Instance.SetCurrentPlayer(currentCar);
        cars[currentCar].jeep.SetActive(true);
        CarName.text = cars[currentCar].CarName.ToString();
       UpdateUI();
    }

    public void OnClickBuyButton()
    {
        int carcost = GetWeaponCost();

        if (GameManager.Instance.TotalCoins >= carcost)
        {
            GameManager.Instance.AddCoins(-carcost);
            if (DataManager.Instance != null)
            {
                DataManager.Instance.SetInt("Car_" + currentCar, 1);
            }
            cars[currentCar].isLook = true;

            if (cars[currentCar].LockImage != null)
                cars[currentCar].LockImage.gameObject.SetActive(false);

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
        MainMenuHandler.instance.carSelection.SetActive(false);
        MainMenuHandler.instance.CharacterSelection.SetActive(true);

        MainMenuHandler.instance.garage.SetActive(false);
        cars[currentCar].jeep.SetActive(false);

        CharacterSelection.instance.LoadGangsterData();
        SoundManager.Instance.PlayEffect(AudioClipsSource.Instance.PlayBtn);
        MainMenuHandler.instance.Canvas.renderMode = RenderMode.ScreenSpaceCamera;
    }


    private void LoadJeepData()
    {
        for (int i = 0; i < cars.Length; i++)
        {
                    if (i == 0)
        {
            cars[i].isLook = true;
            DataManager.Instance.SetInt("Car_" + i, 1);
            if (cars[i].LockImage != null)
                cars[i].LockImage.gameObject.SetActive(false);
            continue;
        }

        if (DataManager.Instance.GetInt("Car_" + i, 0) == 1)
            {
                cars[i].isLook = true;
                if (cars[i].LockImage != null)
                    cars[i].LockImage.gameObject.SetActive(false);
            }
            else
            {
                if (cars[i].LockImage != null)
                    cars[i].LockImage.gameObject.SetActive(true);
            }
        }

        currentCar = DataManager.Instance.GetInt(PlayerPrefsKeys.SELECTED_CAR_INDEX, 0);
        cars[currentCar].jeep.SetActive(true);
    }



    public void UpdateUI()
    {
        coins.text = GameManager.Instance.TotalCoins.ToString();
        gems.text = GameManager.Instance.TotalGems.ToString();
        int weaponCost = GetWeaponCost();
        jeepPriceText.text = weaponCost.ToString();

        if (cars[currentCar].isLook)
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

      

        for (int i = 0; i < cars.Length; i++)
        {
            cars[i].jeep.SetActive(i == currentCar);
        }

        if (currentCar == 0)
        {
            JeepPrice.gameObject.SetActive(false);
        }
        UpdateImageUi();
        
       
    }


    private float lerpSpeed = 100f;

    public void UpdateImageUi()
    {
        fill_speed.fillAmount = 0;
        fill_speed.fillAmount = Mathf.Lerp(fill_speed.fillAmount, cars[currentCar].speed, Time.deltaTime * lerpSpeed);
        fill_engine.fillAmount = 0;
        fill_engine.fillAmount = Mathf.Lerp(fill_engine.fillAmount, cars[currentCar].engine, Time.deltaTime * lerpSpeed);
        fill_break.fillAmount = 0;
        fill_break.fillAmount = Mathf.Lerp(fill_break.fillAmount, cars[currentCar].brake, Time.deltaTime * lerpSpeed);
        fill_acceleration.fillAmount = 0;
        fill_acceleration.fillAmount = Mathf.Lerp(fill_acceleration.fillAmount, cars[currentCar].acceleration, Time.deltaTime * lerpSpeed);
    }



    private int GetWeaponCost()
    {
        return cars[currentCar].JeepPrice;
    }

    public void OnclickBAckButton()
    {
        foreach (var jeep in cars)
        {
            jeep.jeep.SetActive(false);
        }
  
        MainMenuHandler.instance.carSelection.SetActive(false);
        MainMenuHandler.instance.mainPanel.SetActive(true);
        MainMenuHandler.instance.coins.text = GameManager.Instance.TotalCoins.ToString();
        MainMenuHandler.instance.gems.text = GameManager.Instance.TotalGems.ToString();
        SoundManager.Instance.PlayEffect(AudioClipsSource.Instance.BackBtn);
        LoadJeepData();
       
    }


    public GameObject purchasePanel;
    public GameObject CantpurchasePanel;

    IEnumerator PurchaseSuccessfully()
    {
        purchasePanel.transform.DOScale(1, .3f);
        purchasePanel.SetActive(true);
        yield return new WaitForSeconds(3f);
        purchasePanel.transform.DOScale(0, .3f);

        purchasePanel.SetActive(false);
    }

    IEnumerator CantPurches()
    {
        CantpurchasePanel.transform.DOScale(1, .3f);
        CantpurchasePanel.SetActive(true);
        yield return new WaitForSeconds(3f);
        CantpurchasePanel.transform.DOScale(0, .3f);
        CantpurchasePanel.SetActive(false);
    }



    public void UnLockAllCars()
    {
        for (int i = 0; i < cars.Length; i++)
        {
            cars[i].isLook = true;
            if (DataManager.Instance != null)
            {
                DataManager.Instance.SetInt("Car_" + i, 1);
            }

            if (cars[i].LockImage != null)
                cars[i].LockImage.gameObject.SetActive(false);
        }

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
        if (isDragging && cars.Length > 0)
        {
            Vector2 currentPointerPosition = Input.mousePosition;
            float deltaX = currentPointerPosition.x - lastPointerPosition.x;

            if (cars[currentCar].jeep != null)
            {
                cars[currentCar].jeep.transform.Rotate(Vector3.up, -deltaX * rotationSpeed, Space.World);
            }

            lastPointerPosition = currentPointerPosition;
        }
    }





    [System.Serializable]
    public class Cars
    {
        public bool isLook;
        public int JeepPrice;
        public GameObject jeep;
        public float speed;
        public float acceleration;
        public float engine;
        public float brake;
        public Image LockImage;
        public string CarName;
    }
}
