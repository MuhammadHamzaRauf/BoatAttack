using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Invector.vCharacterController;
using DG.Tweening;
using UnityEngine.UI;
//using Gley.TrafficSystem;
public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;
    private CapsuleCollider capsuleCollider;
    [Header("Bike")]
    public GameObject currentVehicleExpectCar;
    public GameObject NowPlayerInThisCar;
    bool IsRewardedAdWatched;

    [Header("Genral GameObjects For Refances")]
    public GameObject CurrentCarDoor;
    public Transform CarSitPos;
    public Transform CarEnterPos;
    public Transform CarExitPos;
    public bool firstTime = true;
    public GameObject[] TotalCarPlayer;
    public GameObject CarPlayer;
    public GameObject checkpoint;
    public GameObject drivingPosOFCAr;
    public GameObject Driver;

    public GameObject REWAREDEDPOPFORhealth;

    public Image FadeImage;

    


    IEnumerator ShowRewardePanel()
    {
        REWAREDEDPOPFORhealth.transform.DOScale(1, 0.5f);
        REWAREDEDPOPFORhealth.SetActive(true);

        yield return new WaitForSeconds(3f);

        REWAREDEDPOPFORhealth.transform.DOScale(0, 0.5f);
        REWAREDEDPOPFORhealth.SetActive(false);

    }
    private void OnEnable()
    {
        // Add bounds checking to prevent array index out of range exceptions
        if (TotalCarPlayer != null && TotalCarPlayer.Length > 0)
        {
            int gangsterIndex = GameManager.Instance.CurrentGangster;
            if (gangsterIndex >= 0 && gangsterIndex < TotalCarPlayer.Length)
            {
                CarPlayer = TotalCarPlayer[gangsterIndex];
            }
            else
            {
                Debug.LogWarning($"PlayerManager: Invalid gangster index {gangsterIndex}, array length is {TotalCarPlayer.Length}");
                // Use the first car player as fallback
                if (TotalCarPlayer.Length > 0)
                {
                    CarPlayer = TotalCarPlayer[0];
                }
            }
        }
        else
        {
            Debug.LogError("PlayerManager: TotalCarPlayer array is null or empty!");
        }
    }
    private void Awake()
    {
        if (instance == null) instance = this;
    }

    void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
    }


    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerToCar"))
        {
            UiManager.instance.CarEnterButton.SetActive(true);

            if (other.gameObject.TryGetComponent<CheckPointOfVehicles>(out CheckPointOfVehicles CP))
            {
                NowPlayerInThisCar = CP.Vehicle;
                CurrentCarDoor = CP.Door;
                CarSitPos = CP.CarSitPos;
                CarEnterPos = CP.CarEnterPos;
                CarExitPos = CP.CarExitPos;
                checkpoint = CP.CheckPoint;
                Driver = CP.driver;
            }
        }
        if (other.gameObject.CompareTag("PlayerToBike"))
        {
            UiManager.instance.BikeEnterButton.SetActive(true);
            if (other.gameObject.TryGetComponent<BikeEnterExit>(out BikeEnterExit CP))
            {
                currentVehicleExpectCar = CP.Vehicle;
                CarEnterPos = CP.EnterPos;
                CarExitPos = CP.ExitPos;
                checkpoint = CP.CheckPoint;
            }
        }
        if (other.gameObject.CompareTag("TelePort"))
        {
            FadeImage.gameObject.SetActive(true);
            FadeImage.DOFade(1, 1f).OnComplete(() => {
                GamePlayHandler.instance.Tps_player.transform.position = other.gameObject.GetComponent<TelePortPlace>().TelePortPosition.position;
                GamePlayHandler.instance.Tps_player.transform.rotation = other.gameObject.GetComponent<TelePortPlace>().TelePortPosition.rotation;
                GamePlayHandler.instance.MiniMapRender.GetComponent<MTAssets.EasyMinimapSystem.MinimapRenderer>().RemoveMinimapItemOfHighlight(GamePlayHandler.instance.levels[GameManager.Instance.CurrentLevel].Destination[0].GetComponent<MTAssets.EasyMinimapSystem.MinimapItem>());
                FadeImage.DOFade(0, 1f).OnComplete(() =>
                 {
                     FadeImage.gameObject.SetActive(false);
                 });
            });


        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerToCar"))
        {
            UiManager.instance.CarEnterButton.SetActive(false);
            CurrentCarDoor = null;
            CarSitPos = null;
            NowPlayerInThisCar = null;
        }
        if (other.gameObject.CompareTag("PlayerToBike"))
        {
            UiManager.instance.BikeEnterButton.SetActive(false);
            
                currentVehicleExpectCar = null;
                CarEnterPos = null;
                CarExitPos = null;
                checkpoint =null;
           
        }
    }
    //void FixedUpdate()
    //{
    //    if (capsuleCollider != null)
    //    {
    //        Vector3 rayOrigin = capsuleCollider.transform.position + capsuleCollider.center;

    //        Vector3 rayDirection = capsuleCollider.transform.forward;

    //        Debug.DrawRay(rayOrigin, rayDirection * 3f, Color.red);

    //        RaycastHit hit;
    //        if (Physics.Raycast(rayOrigin, rayDirection, out hit, 3f))
    //        {
    //            if (hit.collider.CompareTag("PlayerToCar"))
    //            {
    //              //  UiManager.instance.CarEnterButton.SetActive(true);
    //              //  parent = hit.collider.transform.parent.gameObject;
    //              //  Grandparent = parent.transform.parent.gameObject;
    //              //  GamePlayHandler.instance.CurrentActiveCar = Grandparent;
    //               // Debug.Log("HIIIIII");
    //            }
    //           else if (hit.collider.CompareTag("Bike"))
    //            {
    //                UiManager.instance.BikeEnterButton.SetActive(true);
    //                currentVehicleExpectCar = hit.collider.gameObject;
    //            }


    //            else if (hit.collider.CompareTag("StollenCar"))
    //                {
    //                //    UiManager.instance.StollenCarEnterButton.SetActive(true);
    //                //    parent = hit.collider.transform.parent.gameObject;
    //                //    Grandparent = parent.transform.parent.gameObject;
    //                //    GamePlayHandler.instance.CurrentActiveCar = Grandparent;
    //                }
    //            else if (hit.collider.CompareTag("StollenCar8"))
    //            {
    //             //   Debug.Log("Hiiiiiii");
    //                //UiManager.instance.StollenCarEnterButton8.SetActive(true);
    //                //parent = hit.collider.transform.parent.gameObject;
    //                //Grandparent = parent.transform.parent.gameObject;
    //                //GamePlayHandler.instance.CurrentActiveCar = Grandparent;
    //            }
    //            else if (hit.collider.CompareTag("RewardedCar"))
    //            {
    //          //      UiManager.instance.rewardedCarButton.SetActive(true);


    //               NowPlayerInThisCar = hit.collider.transform.parent.gameObject;

    //            }
    //       //     else if (hit.collider.GetComponent<VehicleComponent>())
    //          //  {
    //                //UiManager.instance.TrafficCarEnter.SetActive(true);
    //                //parent = hit.collider.transform.gameObject;
    //                //Grandparent = parent;
    //                //GamePlayHandler.instance.CurrentActiveCar = Grandparent;
    //       //     }
    //            else
    //            {
    //        //        UiManager.instance.tankEnter.SetActive(false);
    //       //         UiManager.instance.HeliCopterEnterButton.SetActive(false);
    //            //    UiManager.instance.CarEnterButton.SetActive(false);
    //                UiManager.instance.BikeEnterButton.SetActive(false);
    //          //      UiManager.instance.rewardedCarButton.SetActive(false);
    //          //      UiManager.instance.TrafficCarEnter.SetActive(false);
    //                NowPlayerInThisCar = null;
    //           //     GamePlayHandler.instance.tank = null;
    //            }
    //        }
    //        else
    //        {
    //      //      UiManager.instance.tankEnter.SetActive(false);
    //      //      UiManager.instance.HeliCopterEnterButton.SetActive(false);
    //       //     UiManager.instance.BikeEnterButton.SetActive(false);
    //       //     UiManager.instance.CarEnterButton.SetActive(false);
    //      //      UiManager.instance.rewardedCarButton.SetActive(false);
    //        }
    //    }
    //}

    //bool hasCalledInternetIssue = false;
    //private void Update()
    //{
    //    if (IsRewardedAdWatched)
    //    {
    //        if (AdsManager.Instance.CheckRewardStatus)
    //        {
    //            Debug.Log("asdkj");

    //            if (AdsManager.Instance.RewardedAdFor == "Player_Health_Increase")
    //            {
    //                playerHealthUpdate();
    //                AdsManager.Instance.CheckRewardStatus = false;
    //            }

    //            AdsManager.Instance.RewardedAdFor = "";
    //            IsRewardedAdWatched = false;
    //            hasCalledInternetIssue = false;
    //        }
    //        else if (AdsManager.Instance.IsRewardedAdNotAvailable)
    //        {
    //            hasCalledInternetIssue = false;
    //            IsRewardedAdWatched = false;
    //            if (!hasCalledInternetIssue)
    //            {
    //              GamePlayHandler.instance.InternetIssue();
    //                hasCalledInternetIssue = true;
    //            }
    //            AdsManager.Instance.IsRewardedAdNotAvailable = false;
    //        }
    //    }
    //}


    // Rewarded For Health Update

    public void ShowRewardedAdForHealthMaxiMum()
    {
      //  playerHealthUpdate();
        IsRewardedAdWatched = true;
   //     AdsManager.Instance.RewardedAdFor = "Player_Health_Increase";
  //      AdsManager.Instance.ShowRewardedVideoAd();
    }


    public void playerHealthUpdate()
    {
     //   if (gameObject.TryGetComponent<vThirdPersonController>(out vThirdPersonController tps))
        {
        //    tps.ResetHealth();
            Time.timeScale = 1;

            StartCoroutine(ShowRewardePanel());
      //      FirebaseManager.Instance.RewardedAdWatchedFor("Player_Health_Increase");
        } 
    }



    public void HealthPanel()
    {
      //  if (gameObject.TryGetComponent<vThirdPersonController>(out vThirdPersonController tps))
        {
         //   if (tps.currentHealth <= 10)
            {
                if (firstTime)
                {
                    Time.timeScale = 0;
               //     UiManager.instance.onHealthDownPanel.SetActive(true);
                    firstTime = false;
                }
            }
        }
    }




}
