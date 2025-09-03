using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Invector.vItemManager;
using DG.Tweening;
using GangsterMafia.Core;
using Invector;
using Invector.vShooter;
using GangsterMafia.Sounds;

public class GamePlayHandler : MonoBehaviour
{
    public static GamePlayHandler instance;
    public GameObject Env;
    public Levels[] levels;
    [Header("Players")]
    public GameObject[] cars;
    public GameObject Tps_player;
    public Transform CarsActiveChaseMood;

    [Header("Time Items")]
    public GameObject TimeImage;
    public Text leveltimeText;


    [Header("Avatar")]
    public GameObject Avatar;


    [Header("Weapons")]
    public GameObject[] weapons;
    public GameObject currentWeapon;



    public GameObject skipButton;
    public Coroutine CutScene;

    bool IsRewardedAdWatched;


    public GameObject miniMapRoutes;

    public GameObject MiniMapRender;

    public GameObject[] CarSitPlayer;
    public GameObject Bike;

    public GameObject currentCar;
    public GameObject RocketLunchar;
    public GameObject PoliceCar;

    public GameObject UiTPS;

    public Transform PoliceInstantiatePos;

    private void Awake()
    {
        if (instance == null) instance = this;
        Env.SetActive(true);
    }



    public void ActiveCar()
    {
        if (levels[GameManager.Instance.CurrentLevel].CarPos != null)
        {
            // Add bounds checking to prevent array index out of range exceptions
            if (cars != null && cars.Length > 0)
            {
                int playerIndex = GameManager.Instance.CurrentPlayer;
                if (playerIndex >= 0 && playerIndex < cars.Length)
                {
                    cars[playerIndex].transform.position = levels[GameManager.Instance.CurrentLevel].CarPos.position;
                    cars[playerIndex].transform.rotation = levels[GameManager.Instance.CurrentLevel].CarPos.rotation;
                    cars[playerIndex].SetActive(true);
                    StartCoroutine(CarKinamticDelay());
                }
                else
                {
                    Debug.LogWarning($"GamePlayHandler.ActiveCar: Invalid player index {playerIndex}, array length is {cars.Length}");
                }
            }
            else
            {
                Debug.LogError("GamePlayHandler.ActiveCar: cars array is null or empty!");
            }
        }
    }

    private void Start()
    {
        if (GameManager.Instance.CurrentGameMode == 0)
        {
            levels[GameManager.Instance.CurrentLevel].levelProps.SetActive(true);
            ActiveCutScene();
        }
        else
        {
            Tps_player.SetActive(true);
            onActiveOpenWOrld();
            
            // Add bounds checking to prevent array index out of range exceptions
            if (cars != null && cars.Length > 0)
            {
                int playerIndex = GameManager.Instance.CurrentPlayer;
                if (playerIndex >= 0 && playerIndex < cars.Length)
                {
                    cars[playerIndex].transform.position = CarsActiveChaseMood.position;
                    cars[playerIndex].transform.rotation = CarsActiveChaseMood.rotation;
                    cars[playerIndex].SetActive(true);
                    StartCoroutine(CarKinamticDelay());
                }
                else
                {
                    Debug.LogWarning($"GamePlayHandler.Start: Invalid player index {playerIndex}, array length is {cars.Length}");
                }
            }
            else
            {
                Debug.LogError("GamePlayHandler.Start: cars array is null or empty!");
            }
            
            MiniMapRender.SetActive(true);
        }
        SoundManager.Instance.StopMusic(AudioClipsSource.Instance.MainMenuClip);
        SoundManager.Instance.PlayBackgroundMusic(AudioClipsSource.Instance.GamePlayClip);
    }


    IEnumerator CarKinamticDelay()
    {
        yield return new WaitForSeconds(2);
        
        // Add bounds checking to prevent array index out of range exceptions
        if (cars != null && cars.Length > 0)
        {
            int playerIndex = GameManager.Instance.CurrentPlayer;
            if (playerIndex >= 0 && playerIndex < cars.Length)
            {
                cars[playerIndex].GetComponent<Rigidbody>().isKinematic = true;
            }
            else
            {
                Debug.LogWarning($"GamePlayHandler.CarKinamticDelay: Invalid player index {playerIndex}, array length is {cars.Length}");
            }
        }
        else
        {
            Debug.LogError("GamePlayHandler.CarKinamticDelay: cars array is null or empty!");
        }
    }

    public GameObject internetIssue;

    public void InternetIssue()
    {
        StartCoroutine(internet());
    }

    IEnumerator internet()
    {
        internetIssue.SetActive(true);
        yield return new WaitForSeconds(2);
        internetIssue.SetActive(false);
    }



    public void ActiveCutScene()
    {
        Debug.Log("hjgskdj");
     CutScene = StartCoroutine(ActiveLevelSTartCinmatic());
    }



    #region WeaponInstantiate

    public void SpawnAllWeaponsExceptCurrent()
    {
        foreach (GameObject weaponPrefab in weapons)
        {
            GameObject spawnedWeapon = Instantiate(weaponPrefab, Tps_player.transform.position, Tps_player.transform.rotation);
            spawnedWeapon.SetActive(true);
        }
    }


    #endregion
    #region StartOnGamePlay
    public void OnActiveGamePlay()
    {
        if (GameManager.Instance.CurrentGameMode == 0)
        {
                Tps_player.transform.position = levels[GameManager.Instance.CurrentLevel].playerPos.transform.position;
                Tps_player.transform.rotation = levels[GameManager.Instance.CurrentLevel].playerPos.transform.rotation;
                Tps_player.SetActive(true);
                UiTPS.SetActive(false);
                UiManager.instance.SetTrafficHolderParent(UiManager.instance.TpsCam);
                UiManager.instance.thirdPerson_CameraMove.SetActive(false);
                ActiveCar();    

                levels[GameManager.Instance.CurrentLevel].Destination[0].SetActive(true);
                MiniMapRender.GetComponent<MTAssets.EasyMinimapSystem.MinimapRenderer>().AddMinimapItemToBeHighlighted(levels[GameManager.Instance.CurrentLevel].Destination[0].GetComponent<MTAssets.EasyMinimapSystem.MinimapItem>());

                if (levels[GameManager.Instance.CurrentLevel].BikePos != null)
                {
                    Bike.transform.position = levels[GameManager.Instance.CurrentLevel].BikePos.transform.position;
                    Bike.transform.rotation = levels[GameManager.Instance.CurrentLevel].BikePos.transform.rotation;
                    Bike.SetActive(true);
                    Debug.Log("HI");
            }
        }
    }

    public GameObject WeaponButton;
    public GameObject Inventory;
    public void OnCLickEquipWeapon()
    {
        SpawnAllWeaponsExceptCurrent();
        WeaponButton.SetActive(false);
        Inventory.SetActive(true);
    }

    #endregion



    #region Cinmatics
       //     int curntModeLvl; 
    IEnumerator ActiveLevelSTartCinmatic()
    {
        UiManager.instance.pauseButton.SetActive(false);
        MiniMapRender.SetActive(false);
        levels[GameManager.Instance.CurrentLevel].StartSinmatic.SetActive(true);
        UiManager.instance.pauseButton.SetActive(false);
        Debug.Log(GameManager.Instance.CurrentLevel);
        
       
        ObjectsOnCutScene();
        yield return new WaitForSeconds(levels[GameManager.Instance.CurrentLevel].StartCInmaticTime);

        ObjectsActiveAfterCutSceneEnd();
       

    }

     void ActiveInstructon()
     {
        UiManager.instance.instruction.SetActive(true);
        UiManager.instance.InstructionText.text = levels[GameManager.Instance.CurrentLevel].LVLiNSTRUCTION;
     }

    public void OnCloseInstructionPanel()
    {
        UiManager.instance.instruction.SetActive(false);
        UiTPS.SetActive(true);
        UiManager.instance.thirdPerson_CameraMove.SetActive(true);
    }

    void ObjectsActiveAfterCutSceneEnd()
    {
        MiniMapRender.SetActive(true);
        UiManager.instance.pauseButton.SetActive(true);
        OnActiveGamePlay();
      
     
        levels[GameManager.Instance.CurrentLevel].StartSinmatic.SetActive(false);
        ObjectsOffAfterCutScene();
        ObjectsAfterCutScene();
        ActiveInstructon();
    }

    #endregion


    #region SkipCinMatic
    public void RewardedAdOnSKip()
    {
    }


    public void OnSKipCutScene()
    {
        ObjectsActiveAfterCutSceneEnd();
        StopCoroutine(CutScene);
    }
    #endregion


    #region LevelTIme
    public float remainingTime;






    private void UpdateTimeUI()
    {
        float minutes = Mathf.FloorToInt(remainingTime / 60);
        float seconds = Mathf.FloorToInt(remainingTime % 60);

        leveltimeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        UiManager.instance.remainingTimeint = (int)remainingTime;
    }

    public bool isTimerRunning = false;


    #endregion

    #region PoliceCatchGangster
    #endregion

    #region ObjectsBefore&AfterCutscene
    public void ObjectsOnCutScene()
    {
        if (levels[GameManager.Instance.CurrentLevel].ObjectsInCutScene != null)
        {
            foreach (GameObject gangster in levels[GameManager.Instance.CurrentLevel].ObjectsInCutScene)
            {
                gangster.SetActive(true);
            }
        }
    }

    public void ObjectsOffAfterCutScene()
    {
        if (levels[GameManager.Instance.CurrentLevel].ObjectsInCutScene != null)
        {
            foreach (GameObject gangster in levels[GameManager.Instance.CurrentLevel].ObjectsInCutScene)
            {
                gangster.SetActive(false);
            }
        }
    }

    public void ObjectsAfterCutScene()
    {
        if (levels[GameManager.Instance.CurrentLevel].ObjectsAftersCutScene != null)
        {
            Debug.Log("HI");
            foreach (GameObject gangster in levels[GameManager.Instance.CurrentLevel].ObjectsAftersCutScene)
            {
            Debug.Log("HIsadddddddd");
                gangster.SetActive(true);
            }
        }
    }
    #endregion




    #region CheckGangsterStatus

    public void checkStatus()
    {
        GameManager.Instance.CurrentKillGangsters += 1;


        if (GameManager.Instance.CurrentKillGangsters == levels[GameManager.Instance.CurrentLevel].levelGangsters)
        {
            if (GameManager.Instance.CurrentLevel == 0  || GameManager.Instance.CurrentLevel ==3 || GameManager.Instance.CurrentLevel == 4 || GameManager.Instance.CurrentLevel == 5)
            {
                Invoke(nameof(LevelEndCutScene), 2);
            }
            if (GameManager.Instance.CurrentLevel == 1)
            {
                MidLevelObjects();
            }
            if (GameManager.Instance.CurrentLevel == 2)
            {
                Invoke(nameof(LevelMidCutScene), 2);
            }

            if (GameManager.Instance.CurrentLevel == 6)
            {
                MidLevelObjects();
                levels[6].Destination[1].SetActive(true);
                MiniMapRender.GetComponent<MTAssets.EasyMinimapSystem.MinimapRenderer>().RemoveMinimapItemOfHighlight(levels[6].Destination[0].GetComponent<MTAssets.EasyMinimapSystem.MinimapItem>());
                MiniMapRender.GetComponent<MTAssets.EasyMinimapSystem.MinimapRenderer>().AddMinimapItemToBeHighlighted(levels[6].Destination[1].GetComponent<MTAssets.EasyMinimapSystem.MinimapItem>());

            }

            if (GameManager.Instance.CurrentLevel == 7)
            {
                levels[7].Car.transform.GetChild(0).gameObject.GetComponent<vHealthController>().enabled = true;            
                levels[7].Car.GetComponent<DOTweenAnimation>().DOPlay();                
                        GameObject spawnedWeapon = Instantiate(RocketLunchar, Tps_player.transform.position, Tps_player.transform.rotation);
                        spawnedWeapon.SetActive(true);
                Tps_player.GetComponent<vShooterManager>().alwaysAiming = true;
            }
        }        
    }

    #endregion




    public void OnHealthSKip()
    {
        Time.timeScale = 1;
    //    UiManager.instance.onHealthDownPanel.SetActive(false);
    }




    #region Level_End_CutScene
    public void LevelEndCutScene()
    {
        StartCoroutine(EndCUtScene());
    }

    IEnumerator EndCUtScene()
    {
        UiManager.instance.TrafficHolder.transform.SetParent(null);
        MiniMapRender.SetActive(false);
        UiManager.instance.HealthUi.SetActive(false);
        UiManager.instance.pauseButton.SetActive(false);
        Tps_player.SetActive(false);
        UiManager.instance.TpsCam.SetActive(false);
        levels[GameManager.Instance.CurrentLevel].levelEndCutScene.SetActive(true);
        UiManager.instance.rcc_Cam.SetActive(false);
        if (PlayerManager.instance.NowPlayerInThisCar != null) 
        PlayerManager.instance.NowPlayerInThisCar.SetActive(false);
        UiManager.instance.Rcc_Controls.SetActive(false);
        yield return new WaitForSeconds(levels[GameManager.Instance.CurrentLevel].levelEndCutSceneTime);
        levels[GameManager.Instance.CurrentLevel].levelEndCutScene.SetActive(false);
        UiManager.instance.OnLevelComplete();
    }
    #endregion


   public void MidLevelObjects()
    {
        foreach (GameObject MidItems in levels[GameManager.Instance.CurrentLevel].MidItems)
        {
            MidItems.SetActive(true);
        }
    }

    #region OpenWOrld

    public void onActiveOpenWOrld()
    {
      
    }





    #endregion



    #region MidCutSCene

    public void LevelMidCutScene()
    {
        StartCoroutine(MidCUtScene());
    }

    IEnumerator MidCUtScene()
    {
        UiManager.instance.TrafficHolder.transform.SetParent(null);
        UiManager.instance.SetTrafficHolderParent(levels[GameManager.Instance.CurrentLevel].MidCutScene);
        UiManager.instance.HealthUi.SetActive(false);
        UiManager.instance.pauseButton.SetActive(false);
        UiManager.instance.SHooterUi.SetActive(false);
        Tps_player.SetActive(false);
        if(levels[GameManager.Instance.CurrentLevel].Car!=null)
        levels[GameManager.Instance.CurrentLevel].Car.SetActive(false);
        UiManager.instance.thirdPerson_Controls.SetActive(false);
        MiniMapRender.SetActive(false);
        UiManager.instance.TpsCam.SetActive(false);
        levels[GameManager.Instance.CurrentLevel].MidCutScene.SetActive(true);
        if (GameManager.Instance.CurrentLevel == 2)
        {
            cars[GameManager.Instance.CurrentPlayer].transform.position = levels[2].MidLevelCarsPos.transform.position;
            cars[GameManager.Instance.CurrentPlayer].transform.rotation = levels[2].MidLevelCarsPos.transform.rotation;
            cars[GameManager.Instance.CurrentPlayer].SetActive(true);
            cars[GameManager.Instance.CurrentPlayer].tag = "Limosine";
        }
        yield return new WaitForSeconds(levels[GameManager.Instance.CurrentLevel].MidCutSCeneTime);
        levels[GameManager.Instance.CurrentLevel].MidCutScene.SetActive(false);
        UiManager.instance.HealthUi.SetActive(true);
        UiManager.instance.pauseButton.SetActive(true);
        UiManager.instance.SHooterUi.SetActive(true);
        if (GameManager.Instance.CurrentLevel == 2)
        {
            foreach (GameObject MidItems in levels[2].MidItems)
            {
                MidItems.SetActive(true);
            }
            levels[2].Destination[1].SetActive(true);
            MiniMapRender.GetComponent<MTAssets.EasyMinimapSystem.MinimapRenderer>().RemoveMinimapItemOfHighlight(levels[2].Destination[0].GetComponent<MTAssets.EasyMinimapSystem.MinimapItem>());
            MiniMapRender.GetComponent<MTAssets.EasyMinimapSystem.MinimapRenderer>().AddMinimapItemToBeHighlighted(levels[2].Destination[1].GetComponent<MTAssets.EasyMinimapSystem.MinimapItem>());

            cars[GameManager.Instance.CurrentPlayer].GetComponent<Rigidbody>().isKinematic = false;


            Tps_player.transform.position = levels[2].MidLevelPlayerPos.transform.position;
            Tps_player.transform.rotation = levels[2].MidLevelPlayerPos.transform.rotation;
            Tps_player.SetActive(true);
            UiManager.instance.TpsCam.SetActive(true);
            UiManager.instance.SetTrafficHolderParent(UiManager.instance.TpsCam);
            UiManager.instance.thirdPerson_Controls.SetActive(true);
            MiniMapRender.SetActive(true);

            foreach (GameObject MidItems in levels[GameManager.Instance.CurrentLevel].MidItems)
            {
                MidItems.SetActive(false);
            }


            foreach (GameObject MidItems in levels[GameManager.Instance.CurrentLevel].MidItems)
            {
                MidItems.SetActive(true);
            }

        }
    }
    #endregion


    public void AiTargetOnPlayer()
    {
        foreach (GameObject target in levels[GameManager.Instance.CurrentLevel].AiGangster)
        {
          //  target.GetComponent<vsh>
        }
    }

}
[System.Serializable]
public class Levels
{
    public GameObject levelProps;
    public GameObject StartSinmatic;
    public float StartCInmaticTime;
    public Transform playerPos;
 //   public float lvlTimer;
    public float levelCompleteInvokeTime;
    public int LevelCoins;
    public GameObject[] ObjectsInCutScene;
    public GameObject[] ObjectsAftersCutScene;

    public GameObject[] MidItems;
    public GameObject MidCutScene;
    public float MidCutSCeneTime;
    public int levelGangsters;

    public GameObject levelEndCutScene;
    public float levelEndCutSceneTime;

    public GameObject[] Destination;


    public Transform MidLevelCarsPos;
    public Transform MidLevelPlayerPos;

    public GameObject Car;
    public Transform CarPos;

    public GameObject[] AiGangster;
    public Transform BikePos;

    public string LVLiNSTRUCTION;
}