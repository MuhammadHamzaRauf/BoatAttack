using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Invector.vCharacterController.AI;
public class CheckGangsterHealthStatus : MonoBehaviour
{
    public GameObject Ai;


    private void OnEnable()
    {
        Ai = gameObject;
    }

    public void CheckDeadSTate()
    {
        if (Ai.TryGetComponent<vControlAIShooter>(out vControlAIShooter ai))
        {
            if (ai.isDead)
            {
                Debug.Log(ai.currentHealth);
                GamePlayHandler.instance.checkStatus();
               
                StartCoroutine(destroy());
              
            }
        }
    }

    IEnumerator destroy()
    {
        yield return new WaitForSeconds(5f);
        Destroy(Ai);
    }

}
