using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using GangsterMafia.Constants;
using GangsterMafia.Core;
using SceneManager = GangsterMafia.Core.SceneManager;

public class UiManager : MonoBehaviour
{
    public static UiManager instance;
    [Header("Controls")]
    public GameObject thirdPerson_Controls;
    public GameObject thirdPerson_CameraMove;
    public GameObject Rcc_Controls;
    public GameObject bike_Controls;
    [Header("Camera")]
    public GameObject rcc_Cam;
    public GameObject TpsCam;
    public GameObject bike_Cam;
    [Header("Buttons")]
    public GameObject CarEnterButton;
    public GameObject BikeEnterButton;
    public GameObject CarExitButton;
    public GameObject BikeExitButton;


    [Header("Level Canvas")]
    public GameObject levelCanvas;
    public GameObject FailPanel;
    public GameObject levelComplete;
    public GameObject levelPause;

    [Header("Level Fail Panel Items")]
    public GameObject SHooterUi;
    public GameObject HealthUi;
  
    public GameObject playerDieCutScene;
    public float cutSceneTime;



    [Header("LevelCompleteTexts")]
    public Text coins;
    public Text remainingTime;
    public Text totalCoins;
    public int remainingTimeint;
    public int totalCoinsInt;
    public bool Is_levelComp = false;

    [Header("LevelFailTexts")]
    public Text coinsF;
    public Text remainingTimeF;
    public Text totalCoinsF;
    public int totalCoinsIntF;


    [Header("Instruction Object")]
    public GameObject instruction;
    public Text InstructionText;


    [Header("Rate US")]
    public string rateus;




    public GameObject pauseButton;

    
    public GameObject nextButton;

    public bool isPLayerInCar = false;
    public bool isPLayerInBike = false;

    public GameObject checkPoint;
    public Sprite Cone;

    public GameObject TrafficHolder;



    private void Awake()
    {
        if (instance == null) instance = this;
        if (GameManager.Instance.CurrentMode == 1)
        {
         
           
            pauseButton.SetActive(true);
        }

    }

    public void SetTrafficHolderParent(GameObject targetCam)
    {
        if (TrafficHolder.transform.parent == TpsCam?.transform ||
            TrafficHolder.transform.parent == rcc_Cam?.transform ||
            TrafficHolder.transform.parent == bike_Cam?.transform)
        {
            TrafficHolder.transform.SetParent(null);
        }
        if (targetCam == TpsCam || targetCam == rcc_Cam || targetCam == bike_Cam)
        {
            TrafficHolder.transform.SetParent(targetCam.transform);
            TrafficHolder.transform.localPosition = Vector3.zero;
            TrafficHolder.transform.localRotation = Quaternion.identity;
        }
        else
        {
            Debug.LogWarning("Invalid camera target provided.");
        }
    }


    #region Intraction with Vehicles
    public void OnClickCarEnterButton()
    {
        isPLayerInCar = true;
        SHooterUi.SetActive(false);
        thirdPerson_CameraMove.GetComponent<Image>().enabled = false;
        rcc_Cam.SetActive(true);
        CarEnterButton.SetActive(false);
        PlayerManager.instance.CurrentCarDoor.GetComponent<DoorController>().RotateDoor();
        CarExitButton.SetActive(true);
        thirdPerson_Controls.SetActive(false);
        PlayerManager.instance.CarPlayer.GetComponent<CarEnterExitScript>().CarEnter();
        PlayerManager.instance.NowPlayerInThisCar.GetComponent<Rigidbody>().isKinematic = false;
        PlayerManager.instance.NowPlayerInThisCar.GetComponent<RCC_CarControllerV3>().enabled = true;
        //  PlayerManager.instance.SitPlayer.transform.rotation = GamePlayHandler.instance.Tps_player.transform.rotation;
        //   PlayerManager.instance.CarSitPos
        PlayerManager.instance.NowPlayerInThisCar.transform.parent = null;
        rcc_Cam.GetComponent<RCC_Camera>().cameraTarget.playerVehicle = PlayerManager.instance.NowPlayerInThisCar.GetComponent<RCC_CarControllerV3>();
        if(PlayerManager.instance.NowPlayerInThisCar.TryGetComponent<TriggerManager>(out TriggerManager Tm))
        {
            if (GameManager.Instance.CurrentLevel == 1)
            {
                PlayerManager.instance.NowPlayerInThisCar.GetComponent<RCC_CarControllerV3>().enabled = true;
                PlayerManager.instance.NowPlayerInThisCar.GetComponent<Rigidbody>().isKinematic = false;
                GamePlayHandler.instance.levels[1].MidItems[0].SetActive(false);
                GamePlayHandler.instance.levels[1].MidItems[0].SetActive(true);
                GamePlayHandler.instance.levels[1].Destination[0].GetComponent<MTAssets.EasyMinimapSystem.MinimapItem>().itemSprite = Cone;
                GamePlayHandler.instance.levels[1].Destination[0].GetComponent<MTAssets.EasyMinimapSystem.MinimapItem>().spriteColor = Color.white;
                GamePlayHandler.instance.levels[1].Destination[1].SetActive(true);
                GamePlayHandler.instance.MiniMapRender.GetComponent<MTAssets.EasyMinimapSystem.MinimapRenderer>().RemoveMinimapItemOfHighlight(GamePlayHandler.instance.levels[1].Destination[0].GetComponent<MTAssets.EasyMinimapSystem.MinimapItem>());
                GamePlayHandler.instance.MiniMapRender.GetComponent<MTAssets.EasyMinimapSystem.MinimapRenderer>().AddMinimapItemToBeHighlighted(GamePlayHandler.instance.levels[1].Destination[1].GetComponent<MTAssets.EasyMinimapSystem.MinimapItem>());
              
                if(Tm.ALert!=null)
                Tm.ALert.Play();
            }
        }

        PlayerManager.instance.NowPlayerInThisCar.GetComponent<MTAssets.EasyMinimapSystem.MinimapCamera>().enabled = true;
        PlayerManager.instance.NowPlayerInThisCar.GetComponent<MTAssets.EasyMinimapSystem.MinimapItem>().enabled = true;
        GamePlayHandler.instance.Tps_player.GetComponent<MTAssets.EasyMinimapSystem.MinimapItem>().enabled = false;
        GamePlayHandler.instance.Tps_player.GetComponent<MTAssets.EasyMinimapSystem.MinimapCamera>().enabled = false;
        GamePlayHandler.instance.MiniMapRender.GetComponent<MTAssets.EasyMinimapSystem.MinimapRenderer>().minimapCameraToShow = PlayerManager.instance.NowPlayerInThisCar.GetComponent<MTAssets.EasyMinimapSystem.MinimapCamera>();
        TpsCam.SetActive(false);
        StartCoroutine(ControllsEnableEnterinCar());
        SetTrafficHolderParent(rcc_Cam);


    }
    IEnumerator ControllsEnableEnterinCar()
    {
        yield return new WaitForSeconds(1.5f);

        Rcc_Controls.SetActive(true);

        PlayerManager.instance.Driver.SetActive(true);
        PlayerManager.instance.CarPlayer.SetActive(false);
    }


    public void OnClickCarExitButton()
    {
        StartCoroutine(ActivePlayerWhenExitCar());
        Rcc_Controls.SetActive(false);
        PlayerManager.instance.CurrentCarDoor.GetComponent<DoorController>().RotateDoor();
        PlayerManager.instance.CarPlayer.GetComponent<CarEnterExitScript>().CarExit();
        PlayerManager.instance.NowPlayerInThisCar.GetComponent<Rigidbody>().isKinematic = true;
        PlayerManager.instance.NowPlayerInThisCar.GetComponent<RCC_CarControllerV3>().enabled = false;
        isPLayerInCar = false;
        CarExitButton.SetActive(false);
        GamePlayHandler.instance.Tps_player.GetComponent<MTAssets.EasyMinimapSystem.MinimapItem>().enabled = true;
        GamePlayHandler.instance.Tps_player.GetComponent<MTAssets.EasyMinimapSystem.MinimapCamera>().enabled = true;
        PlayerManager.instance.NowPlayerInThisCar.GetComponent<MTAssets.EasyMinimapSystem.MinimapCamera>().enabled = false;
        PlayerManager.instance.NowPlayerInThisCar.GetComponent<MTAssets.EasyMinimapSystem.MinimapItem>().enabled = false;
        GamePlayHandler.instance.MiniMapRender.GetComponent<MTAssets.EasyMinimapSystem.MinimapRenderer>().minimapCameraToShow = GamePlayHandler.instance.Tps_player.GetComponent<MTAssets.EasyMinimapSystem.MinimapCamera>();

        SetTrafficHolderParent(TpsCam);

    }

    IEnumerator ActivePlayerWhenExitCar()
    {
        PlayerManager.instance.Driver.SetActive(false);
        PlayerManager.instance.CarPlayer.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        Debug.Log("sadasd");
        GamePlayHandler.instance.Tps_player.transform.position =PlayerManager.instance.CarPlayer.transform.position;
        GamePlayHandler.instance.Tps_player.transform.rotation =PlayerManager.instance.CarPlayer.transform.rotation;
        GamePlayHandler.instance.Tps_player.SetActive(true);
        PlayerManager.instance.CarPlayer.SetActive(false);
        thirdPerson_CameraMove.GetComponent<Image>().enabled = true;
        Rcc_Controls.SetActive(false);
        rcc_Cam.SetActive(false);
        TpsCam.SetActive(true);
        thirdPerson_Controls.SetActive(true);
        SHooterUi.SetActive(true);
    }

    public void onClickBikeEnterButton()
    {
        isPLayerInBike = true;
        bike_Controls.SetActive(true);
        SHooterUi.SetActive(false);
        bike_Cam.SetActive(true);
        TpsCam.SetActive(false);
        thirdPerson_Controls.SetActive(false);
        GamePlayHandler.instance.Tps_player.SetActive(false);
        PlayerManager.instance.checkpoint.GetComponent<BikeEnterExit>().OnBikeEnter();
        PlayerManager.instance.currentVehicleExpectCar.GetComponent<Rigidbody>().isKinematic = false;

        PlayerManager.instance.currentVehicleExpectCar.GetComponent<MTAssets.EasyMinimapSystem.MinimapCamera>().enabled = true;
        PlayerManager.instance.currentVehicleExpectCar.GetComponent<MTAssets.EasyMinimapSystem.MinimapItem>().enabled = true;
        GamePlayHandler.instance.Tps_player.GetComponent<MTAssets.EasyMinimapSystem.MinimapItem>().enabled = false;
        GamePlayHandler.instance.Tps_player.GetComponent<MTAssets.EasyMinimapSystem.MinimapCamera>().enabled = false;
        GamePlayHandler.instance.MiniMapRender.GetComponent<MTAssets.EasyMinimapSystem.MinimapRenderer>().minimapCameraToShow = PlayerManager.instance.currentVehicleExpectCar.GetComponent<MTAssets.EasyMinimapSystem.MinimapCamera>();
        SetTrafficHolderParent(bike_Cam);
    }
    public void onClickBikeExitButton()
    {
        isPLayerInBike = false;
        PlayerManager.instance.checkpoint.SetActive(true);
        bike_Controls.SetActive(false);
        PlayerManager.instance.currentVehicleExpectCar.GetComponent<Rigidbody>().isKinematic = true;
        //  Exit Pos
        PlayerManager.instance.checkpoint.GetComponent<BikeEnterExit>().OnBikeExit();
        PlayerManager.instance.currentVehicleExpectCar.GetComponent<MTAssets.EasyMinimapSystem.MinimapCamera>().enabled = false;
        PlayerManager.instance.currentVehicleExpectCar.GetComponent<MTAssets.EasyMinimapSystem.MinimapItem>().enabled = false;
        GamePlayHandler.instance.Tps_player.GetComponent<MTAssets.EasyMinimapSystem.MinimapItem>().enabled = true;
        GamePlayHandler.instance.Tps_player.GetComponent<MTAssets.EasyMinimapSystem.MinimapCamera>().enabled = true;
        GamePlayHandler.instance.MiniMapRender.GetComponent<MTAssets.EasyMinimapSystem.MinimapRenderer>().minimapCameraToShow = GamePlayHandler.instance.Tps_player.GetComponent<MTAssets.EasyMinimapSystem.MinimapCamera>();
        SHooterUi.SetActive(true);
        SetTrafficHolderParent(TpsCam);
    }

        #endregion



        #region Level Fail Panel
        public void OnPlayerDie()
    {
   //     SoundManager.Instance.StopMusic(AudioClipsSource.Instance.GamePlayClip);
   //     SoundManager.Instance.StopMusic(AudioClipsSource.Instance.MainMenuClip);
   //     SoundManager.Instance.PlayEffect(AudioClipsSource.Instance.death);
        levelCanvas.SetActive(true); 
        GamePlayHandler.instance.MiniMapRender.SetActive(false);
        SHooterUi.SetActive(false);
        thirdPerson_Controls.SetActive(false);
        Debug.Log("QQQQQQQQQQQQQ");
        StartCoroutine(PlayerDieEffect());
    }

    IEnumerator PlayerDieEffect()
    {
        yield return new WaitForSeconds(1);
        playerDieCutScene.SetActive(true);
        yield return new WaitForSeconds(cutSceneTime);

        playerDieCutScene.SetActive(false);
        yield return new WaitForSeconds(3);
        LevelFail();
    }



    public void LevelFail()
    {
       // levelCanvas.SetActive(true);
        GamePlayHandler.instance.MiniMapRender.SetActive(false);
    //    InventryComponent.SetActive(false);
        GamePlayHandler.instance.Tps_player.SetActive(false);
        FailPanel.SetActive(true);
        thirdPerson_Controls.SetActive(false);
        bike_Controls.SetActive(false);
        Rcc_Controls.SetActive(false);
        HealthUi.SetActive(false);
        pauseButton.SetActive(false);
        //    coinsintF = GamePlayHandler.instance.levels[GameManager.instance.currentlevel].LevelCoins;
        //     coinsF.text = coinsintF.ToString();
        //     remainingTimeF.text = "0";
        //      TotalCoinsIntF = coinsintF;
        //      totalCOinsF.text = TotalCoinsIntF.ToString();
        //      Debug.Log(totalCOinsF);
        //      GameManager.instance.totalCoins += TotalCoinsIntF;
        //       PlayerPrefs.SetInt("TotalCoins", GameManager.instance.totalCoins);
        //        SoundManager.Instance.PlayEffect(AudioClipsSource.Instance.LevelLose);
        //        SoundManager.Instance.StopMusic(AudioClipsSource.Instance.GamePlayClip);

        //    GamePlayHandler.instance.trafficHolder.SetActive(false);
        //      GamePlayHandler.instance.MobileTraffic.SetActive(false);

        //      AdsManager.Instance.ShowBanner(1); // LEft Banner
        //       AdsManager.Instance.ShowBanner(2); // Rectangle
        //    AdsManag
        //   StartCoroutine(ShowIntersttial()); // InterStitial
        GameManager.Instance.CurrentKillGangsters = 0;
    }


    public void OnLevelComplete()
    {

        //    SoundManager.Instance.StopMusic(AudioClipsSource.Instance.MainMenuClip);
        //    SoundManager.Instance.StopMusic(AudioClipsSource.Instance.GamePlayClip);
        //    SoundManager.Instance.PlayEffect(AudioClipsSource.Instance.LevelWin);
        levelComplete.SetActive(true);
        int currentLevel = GameManager.Instance.CurrentLevel;
        int nextLevel = currentLevel + 1;
        Debug.Log(nextLevel);
        HealthUi.SetActive(false);
        pauseButton.SetActive(false);

        GamePlayHandler.instance.Tps_player.SetActive(false);
    
   //     coinsint = GamePlayHandler.instance.levels[GameManager.instance.currentlevel].LevelCoins;
      //  coins.text = coinsint.ToString();
    //    remainingTime.text = remainingTimeint.ToString();
   //     TotalCoinsInt = coinsint + remainingTimeint;
   //     totalCOins.text = TotalCoinsInt.ToString();
    //    GameManager.instance.totalCoins += TotalCoinsInt;
        // Update the progress only if the next level is not beyond the available levels
        //if (nextLevel < LevelSelection.instance.lvlButtons.Length)
        //{
        //    PlayerPrefs.SetInt("levelAt", nextLevel);
        //}
      



    //    GamePlayHandler.instance.trafficHolder.SetActive(false);


        GameManager.Instance.CurrentKillGangsters = 0;


        foreach (Levels lvl in GamePlayHandler.instance.levels)
        {
            lvl.levelProps.SetActive(false);
        }

  //      GamePlayHandler.instance.MobileTraffic.SetActive(false);
     
   //     AdsManager.Instance.HideBanner(4); // Center

       
        GamePlayHandler.instance.MiniMapRender.SetActive(false);

        thirdPerson_Controls.SetActive(false);
        bike_Controls.SetActive(false);
        Rcc_Controls.SetActive(false);
        //   StartCoroutine(ShowIntersttial()); // InterStitial
    }

  

    public void levelCompleteFuncationCallAfterTime()
    {
        Is_levelComp = true;
        Invoke("OnLevelComplete", GamePlayHandler.instance.levels[GameManager.Instance.CurrentLevel].levelCompleteInvokeTime);
    }


    IEnumerator ShowIntersttial()
    {
     //   AdsManager.Instance.ShowInterstitial();
    //    yield return new WaitUntil(AdsManager.Instance.MoveOnAfterAd);
        yield break;
    }




    #endregion


    #region Buttons
    public void OnClickNextButton()
    {
        StartCoroutine(ShowIntersttial()); // InterStitial
        if (GameManager.Instance.CurrentMode == 0)
        {
         //   SoundManager.Instance.PlayEffect(AAudioClipsSource.Instance.PlayBtn);
            GameManager.Instance.CurrentLevel += 1;
            GameManager.Instance.CurrentKillGangsters = 0;
           
            SceneManager.Instance.LoadScene(GameConstants.GAMEPLAY_SCENE);
            

        }
        else if (GameManager.Instance.CurrentMode == 1)
        {
     //       SoundManager.Instance.PlayEffect(AudioClipsSource.Instance.PlayBtn);
            levelComplete.SetActive(false);

            GamePlayHandler.instance.Tps_player.SetActive(true);
            GamePlayHandler.instance.onActiveOpenWOrld();

           
          //  GamePlayHandler.instance.MobileTraffic.SetActive(true);
            GamePlayHandler.instance.MiniMapRender.SetActive(true);
            GamePlayHandler.instance.miniMapRoutes.SetActive(false);
          //  GamePlayHandler.instance.miniMapRoutes.GetComponent<MTAssets.EasyMinimapSystem.MinimapRoutes>().StopCalculatingAndHideRotesToDestination();
         
        //    InstantiatePrefabNearPlayer.instance.initialDelay = 300f;
        //    InstantiatePrefabNearPlayer.instance.StartInstantiatingPrefab();

        }
        PlayerPrefs.SetInt("TotalCoins", GameManager.Instance.TotalCoins);
        GameManager.Instance.CurrentKillGangsters = 0;
  //      AdsManager.Instance.HideBanner(1); // LEft Banner
   //     AdsManager.Instance.HideBanner(2); // Rectangle
    //    AdsManager.Instance.ShowBanner(4); // Center
    }


    public void NextbuttonModeSelection()
    {
        StartCoroutine(ShowIntersttial()); // InterStitial
        SceneManager.Instance.LoadScene(GameConstants.MAINMENU_SCENE);
        GameManager.Instance.CheckSceneForMode = 1;
  //      AdsManager.Instance.HideBanner(2); // Rectangle
    }


    public void OnCLickSkillsButton()
    {
        StartCoroutine(ShowIntersttial()); // InterStitial
        SceneManager.Instance.LoadScene(GameConstants.MAINMENU_SCENE);
        GameManager.Instance.CheckSceneForMode = 2;
    //    AdsManager.Instance.HideBanner(4);
    }




    public void OnResatrt()
    {
     //   SoundManager.Instance.PlayEffect(AudioClipsSource.Instance.BtnClick);
        Time.timeScale = 1;
        GameManager.Instance.CurrentKillGangsters = 0;
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    //    AdsManager.Instance.HideBanner(1); // LEft Banner
     //   AdsManager.Instance.HideBanner(2); // Rectangle
  //      AdsManager.Instance.ShowBanner(4); // Center
    }

    public void onClickHomeButton()
    {
        StartCoroutine(ShowIntersttial()); // InterStitial
      //  SoundManager.Instance.PlayEffect(AudioClipsSource.Instance.BackBtn);
        Time.timeScale = 1;
        SceneManager.Instance.LoadScene(GameConstants.MAINMENU_SCENE);
        GameManager.Instance.CurrentKillGangsters = 0;
        PlayerPrefs.SetInt("TotalCoins", GameManager.Instance.TotalCoins);
        //    AdsManager.Instance.HideBanner(1); // LEft Banner
        //    AdsManager.Instance.HideBanner(2); // Rectangle
        //   AdsManager.Instance.ShowBanner(0); // Right Banner
    //    AdsManager.Instance.HideBanner(2); // Rectangle
    }
    public void OnPauseButton()
    {
            StartCoroutine(ShowIntersttial()); // InterStitial
      //      AdsManager.Instance.ShowBanner(1); // LEft Banner
   //     AdsManager.Instance.ShowBanner(2); // Rectangle
                                           //      AdsManager.Instance.HideBanner(4); // Center
        GamePlayHandler.instance.MiniMapRender.SetActive(false);
        Time.timeScale = 0;
        levelPause.SetActive(true);
       // GamePlayHandler.instance.TimeImage.SetActive(false);
        //     SoundManager.Instance.PlayEffect(AudioClipsSource.Instance.BtnClick);
        //    SoundManager.Instance.StopMusic(AudioClipsSource.Instance.GamePlayClip);

        if (!isPLayerInCar && !isPLayerInBike)
        {
            GamePlayHandler.instance.Tps_player.SetActive(false);
        }
        if (isPLayerInCar)
        {
            Rcc_Controls.SetActive(false);
        }
        if (isPLayerInBike)
        {
            bike_Controls.SetActive(false);
        }
      
     //   GamePlayHandler.instance.trafficHolder.SetActive(false);
        if (GameManager.Instance.PoliceCar != null)
        {
            GameManager.Instance.PoliceCar.SetActive(false);
        }
    }
    public void Resume()
    {
     
        //   AdsManager.Instance.HideBanner(1); // LEft Banner
        //    AdsManager.Instance.HideBanner(2); // Rectangle
        //     AdsManager.Instance.ShowBanner(4); // Center
        StartCoroutine(ShowIntersttial()); // InterStitial
        Time.timeScale = 1;
        levelPause.SetActive(false);
        GamePlayHandler.instance.MiniMapRender.SetActive(true);
     //   SoundManager.Instance.PlayEffect(AudioClipsSource.Instance.PlayBtn);
    //    SoundManager.Instance.PlayBackgroundMusic(AudioClipsSource.Instance.GamePlayClip);
       // if (GamePlayHandler.instance.levels[GameManager.instance.currentlevel].levelProps.activeInHierarchy)
     //   {
         //   GamePlayHandler.instance.TimeImage.SetActive(true);
      //  }
        if (!isPLayerInCar && !isPLayerInBike)
        {
            GamePlayHandler.instance.Tps_player.SetActive(true);
        }
        if (isPLayerInCar)
        {
            Rcc_Controls.SetActive(true);
        }
        if (isPLayerInBike)
        {
            bike_Controls.SetActive(true);
        }
    }
    #endregion


    #region InstructionButton

    // Rewarded ad For Instruction
    public void ShowRewardedAdForInstructionButton()
    {
      //  OnclickInstructionButton();
   //     IsRewardedAdWatched = true;
    //    AdsManager.Instance.RewardedAdFor = "Instruction";
    //    AdsManager.Instance.ShowRewardedVideoAd();
    }


    public void OnclickInstructionButton()
    {
        StartCoroutine(disAbleInst());
    }


    IEnumerator disAbleInst()
    {
        instruction.SetActive(true);
        yield return new WaitForSeconds(5);
        instruction.SetActive(false);
    }

    #endregion










    public void OnCLickActivePanels()
    {
        if (isPLayerInCar)
        {
            Rcc_Controls.SetActive(false);
        }
        if (!isPLayerInCar)
        {
            thirdPerson_Controls.SetActive(false);
        }
      
    }

    public void OnCloseActivePanels()
    {
        if (isPLayerInCar)
        {
            Rcc_Controls.SetActive(true);
        }
        if (!isPLayerInCar)
        {
            thirdPerson_Controls.SetActive(true);
        }
    }





    #region RateUS
    public void OnclickRateUS()
    {
        Application.OpenURL(rateus);
    }
    #endregion

    // Rewaeded ad for Double Coins
    public void ShowRewardedAdForDoubleCoins()
    {
      //  OnClickDoubleReward();
       // IsRewardedAdWatched = true;
    //    AdsManager.Instance.RewardedAdFor = "Coins_Double";
     //   AdsManager.Instance.ShowRewardedVideoAd();
    }

    public void OnClickDoubleReward()
    {
        totalCoinsInt = totalCoinsInt * 2;
        totalCoins.text = totalCoinsInt.ToString();
        GameManager.Instance.TotalCoins += totalCoinsInt;
        PlayerPrefs.SetInt("TotalCoins", GameManager.Instance.TotalCoins);
    }

    public void OnClickBigMiniMapOpen()
    {
      //  BigMiniMap.SetActive(true);

        if (!isPLayerInCar)
        {
         //   GamePlayHandler.instance.Tps_player.GetComponent<MTAssets.EasyMinimapSystem.MinimapItem>().particlesHighlightMode = MTAssets.EasyMinimapSystem.MinimapItem.ParticlesHighlightMode.WavesIncrease;
        //    FirebaseManager.Instance.CustomCrashEvent("MiniMap_Open", "Player_waves_increase");
          //  StartCoroutine(MoveCamera());
        }
        else if (isPLayerInCar)
        {
        //    GamePlayHandler.instance.CurrentActiveCar.GetComponent<MTAssets.EasyMinimapSystem.MinimapItem>().particlesHighlightMode = MTAssets.EasyMinimapSystem.MinimapItem.ParticlesHighlightMode.WavesIncrease;
         //   FirebaseManager.Instance.CustomCrashEvent("MiniMap_Open", "Car_waves_increase");
       //     StartCoroutine(MoveCameraC());
        }
      //  FirebaseManager.Instance.CustomCrashEvent("MiniMap_Open", "MiniMap_Open");
        OnCLickActivePanels();
     //   FirebaseManager.Instance.CustomCrashEvent("MiniMap_Open", "Panels_Status_off");


    }

    public void OnClickCrossMiniMap()
    {
      //  BigMiniMap.SetActive(false);
        if (!isPLayerInCar)
        {
          //  FirebaseManager.Instance.CustomCrashEvent("MiniMap_close", "Player_waves_Disable");
        //    GamePlayHandler.instance.Tps_player.GetComponent<MTAssets.EasyMinimapSystem.MinimapItem>().particlesHighlightMode = MTAssets.EasyMinimapSystem.MinimapItem.ParticlesHighlightMode.Disabled;
        }
        else if (isPLayerInCar)
        {
          //  FirebaseManager.Instance.CustomCrashEvent("MiniMap_close", "Car_waves_Disable");
        //    GamePlayHandler.instance.CurrentActiveCar.GetComponent<MTAssets.EasyMinimapSystem.MinimapItem>().particlesHighlightMode = MTAssets.EasyMinimapSystem.MinimapItem.ParticlesHighlightMode.Disabled;
        }
     //   FirebaseManager.Instance.CustomCrashEvent("MiniMap_close", "MiniMap_close");
        OnCloseActivePanels();
      //  FirebaseManager.Instance.CustomCrashEvent("MiniMap_Close", "Panels_Status_on");
    }



    //private IEnumerator MoveCamera()
    //{
    //    float moveTime = 1f;
    //    float currentTime = 0f;

    //    Vector3 startPos = fullMapScreenCam.transform.position;

    //    Vector3 targetPos = GamePlayHandler.instance.Tps_player.transform.position;
    //    while (currentTime < moveTime)
    //    {
    //        currentTime += Time.deltaTime;
    //        fullMapScreenCam.transform.position = Vector3.Lerp(startPos, targetPos, currentTime / moveTime);
    //        yield return null;
    //    }
    //    fullMapScreenCam.transform.position = targetPos;
    //}

    //private IEnumerator MoveCameraC()
    //{
    //    float moveTime = 1f;
    //    float currentTime = 0f;

    //    Vector3 startPos = fullMapScreenCam.transform.position;
    //    Vector3 targetPos = GamePlayHandler.instance.CurrentActiveCar.transform.position;
    //    while (currentTime < moveTime)
    //    {
    //        currentTime += Time.deltaTime;
    //        fullMapScreenCam.transform.position = Vector3.Lerp(startPos, targetPos, currentTime / moveTime);
    //        yield return null;
    //    }

    //    fullMapScreenCam.transform.position = targetPos;
    //}


    private int currentDestinationIndex = 0;

    public void NextDestinationPoint()
    {
        if (currentDestinationIndex < GamePlayHandler.instance.levels[GameManager.Instance.CurrentLevel].Destination.Length - 1)
        {
            currentDestinationIndex++;
      //      GamePlayHandler.instance.miniMapRoutes.GetComponent<MTAssets.EasyMinimapSystem.MinimapRoutes>().destinationPoint =
          //      GamePlayHandler.instance.levels[GameManager.instance.currentlevel].Destination[currentDestinationIndex].transform;

            Debug.Log(currentDestinationIndex);
        }
        else
        {
            Debug.Log("All destinations have been processed.");
        }
    }

    public int cameraN;
    public void onchangeCamera()
    {
        if (cameraN == 0)
        {
            rcc_Cam.GetComponent<RCC_Camera>().cameraMode = RCC_Camera.CameraMode.FPS;
            cameraN = 1;
            PlayerManager.instance.Driver.SetActive(false);
        }
        else if (cameraN == 1)
        {
            rcc_Cam.GetComponent<RCC_Camera>().cameraMode = RCC_Camera.CameraMode.TPS;
            cameraN = 0;
            Debug.Log(cameraN);
            PlayerManager.instance.Driver.SetActive(true);
        }
    }


    //public void triggerforlvlone()
    //{
    //    if (GameManager.instance.currentlevel == 0)
    //    {
    //        StartCoroutine(fadeImageShow());
    //    }
    //}

    // IEnumerator fadeImageShow()
    // {
    //     fadeImage.SetActive(true);
    //     Rcc_Controls.SetActive(false);
    //     float duration = 1f;
    //     float elapsedTime = 0f;
    //     Color startColor = new Color(0, 0, 0, 0);
    //     Color endColor = new Color(0, 0, 0, 1);
    //     while (elapsedTime < duration)
    //     {
    //         float t = elapsedTime / duration;
    //         fadeImage.GetComponent<Image>().color = Color.Lerp(startColor, endColor, t);
    //         elapsedTime += Time.deltaTime;
    //         yield return null;
    //     }

    //     fadeImage.GetComponent<Image>().color = endColor;

    //     yield return new WaitForSeconds(1f);
    ////     GamePlayHandler.instance.miniMapRoutes.GetComponent<MTAssets.EasyMinimapSystem.MinimapRoutes>().destinationPoint = GamePlayHandler.instance.levels[GameManager.instance.currentlevel].Destination[1].transform;

    //     elapsedTime = 0f;
    //     startColor = new Color(0, 0, 0, 1); 
    //     endColor = new Color(0, 0, 0, 0); 
    //     while (elapsedTime < duration)
    //     {
    //         float t = elapsedTime / duration;
    //         fadeImage.GetComponent<Image>().color = Color.Lerp(startColor, endColor, t);
    //         elapsedTime += Time.deltaTime;
    //         yield return null;
    //     }
    //     fadeImage.GetComponent<Image>().color = endColor;

    //     fadeImage.SetActive(false);
    //     Rcc_Controls.SetActive(true);
    // }

}