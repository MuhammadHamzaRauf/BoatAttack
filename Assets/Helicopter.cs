using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helicopter : MonoBehaviour
{
    public GameObject particleEffect;
    public void BlastHelicopter()
    {
        gameObject.AddComponent<Rigidbody>();
        gameObject.GetComponent<Rigidbody>().mass = 10000;
        particleEffect.SetActive(true);
        GamePlayHandler.instance.LevelEndCutScene();
    }
  
}
