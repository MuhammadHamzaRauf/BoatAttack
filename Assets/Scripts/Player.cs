using System.Collections;
using System.Collections.Generic;
// using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
  //  public GameObject VehicleControlButton;
 //   public GameObject ThirdPersonControlButton;
 //   public GameObject TPSCamera;
 //   public GameObject VehicleCamera;
 //   public GameObject TPS;
 //   public GameObject PlayerVehicle;


    void Start()
    {
    //    PlayerVehicle.GetComponent<Rigidbody>().isKinematic = false;
    //    PlayerVehicle.GetComponent<RCC_CarControllerV3>().KillEngine();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerToCar")) {
          //  ChangePlayerToCarControl(other.gameObject);
        }
    }

    void ChangePlayerToCarControl(GameObject go) {

        go.SetActive(false);
    //    VehicleControlButton.SetActive(true);
        //ThirdPersonControlButton.SetActive(false);
  //      VehicleCamera.SetActive(true);
   //     TPSCamera.SetActive(false);
    //    PlayerVehicle.GetComponent<Rigidbody>().isKinematic = false;
     //   PlayerVehicle.GetComponent<RCC_CarControllerV3>().StartEngine();
      //  TPS.SetActive(false);
    
    }
}
