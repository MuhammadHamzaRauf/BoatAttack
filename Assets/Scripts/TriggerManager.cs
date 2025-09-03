using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerManager : MonoBehaviour
{
    public int TotalHit = 2;
    public int currentHit;
    public AudioSource ALert;
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.TryGetComponent<RCC_AICarController>(out RCC_AICarController Ai))
        {
            if (GameManager.Instance.CurrentLevel == 1 || GameManager.Instance.CurrentLevel == 2 || GameManager.Instance.CurrentMode == 1)
            {
                currentHit++;
                if (TotalHit == currentHit)
                    UiManager.instance.LevelFail();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("EndPoint"))
        {
            Debug.Log("hjdsa");
            GamePlayHandler.instance.LevelEndCutScene();
        }
    }


}
