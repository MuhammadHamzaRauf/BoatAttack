using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BikeEnterExit : MonoBehaviour
{
    public Transform EnterPos;
    public Transform ExitPos;
    public GameObject[] GangsterRigs;
    public GameObject BikeRider;
    public GameObject Vehicle;
    public GameObject CheckPoint;

    private void OnEnable()
    {
        Vehicle = transform.parent.gameObject;
        CheckPoint = gameObject;
        
        // Add bounds checking to prevent array index out of range exceptions
        if (GangsterRigs != null && GangsterRigs.Length > 0)
        {
            int gangsterIndex = GameManager.Instance.CurrentGangster;
            if (gangsterIndex >= 0 && gangsterIndex < GangsterRigs.Length)
            {
                BikeRider = GangsterRigs[gangsterIndex];
            }
            else
            {
                Debug.LogWarning($"BikeEnterExit: Invalid gangster index {gangsterIndex}, array length is {GangsterRigs.Length}");
                // Use the first gangster rig as fallback
                if (GangsterRigs.Length > 0)
                {
                    BikeRider = GangsterRigs[0];
                }
            }
        }
        else
        {
            Debug.LogError("BikeEnterExit: GangsterRigs array is null or empty!");
        }
    }
    public void OnBikeEnter()
    {
        // StartCoroutine(EnterBike());
        BikeRider.SetActive(true);
        CheckPoint.SetActive(false);
    }

    //IEnumerator EnterBike()
    //{
    //    PlayerManager.instance.BikePlayer.transform.position = EnterPos.transform.position;
    //    PlayerManager.instance.BikePlayer.transform.rotation = EnterPos.transform.rotation;
    //    PlayerManager.instance.BikePlayer.SetActive(true);
    //    PlayerManager.instance.BikePlayer.GetComponent<Animator>().Play("GettingOnBike");
    //    yield return new WaitForSeconds(1);
    //    BikeRider.SetActive(true);
    //    PlayerManager.instance.BikePlayer.SetActive(false);
    //    CheckPoint.SetActive(false);
    //}

    public void OnBikeExit()
    {
        CheckPoint.SetActive(true);
        StartCoroutine(ExitBike());
    }

    IEnumerator ExitBike()
    {
        BikeRider.SetActive(false);
    //    PlayerManager.instance.BikePlayer.transform.position = ExitPos.transform.position;
    //    PlayerManager.instance.BikePlayer.transform.rotation = ExitPos.transform.rotation;
    //    PlayerManager.instance.BikePlayer.SetActive(true);
   //     PlayerManager.instance.BikePlayer.GetComponent<Animator>().Play("GettingOffBike");
        
        yield return new WaitForSeconds(.01f);
      //  PlayerManager.instance.BikePlayer.SetActive(false);
        GamePlayHandler.instance.Tps_player.transform.position = ExitPos.transform.position;
        GamePlayHandler.instance.Tps_player.transform.rotation = ExitPos.transform.rotation;
        GamePlayHandler.instance.Tps_player.SetActive(true);
        UiManager.instance.thirdPerson_Controls.SetActive(true);
        UiManager.instance.bike_Cam.SetActive(false);
        UiManager.instance.TpsCam.SetActive(true);

    }
}
