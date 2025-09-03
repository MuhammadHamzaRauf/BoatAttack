using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Invector.vCharacterController;
public class LevelFail : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<RCC_CarControllerV3>() || other.gameObject.GetComponent<vThirdPersonController>())
        {
            UiManager.instance.LevelFail();
            gameObject.GetComponent<BoxCollider>().enabled = false;
        }

    }
}
