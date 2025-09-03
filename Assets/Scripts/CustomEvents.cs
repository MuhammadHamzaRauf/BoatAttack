using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CustomEvents : MonoBehaviour
{
    public UnityEvent onEnableEvent = new UnityEvent();

    public UnityEvent onDisableEvent = new UnityEvent();

    public string tagName = "";
    public UnityEvent onTriggerEnter = new UnityEvent();

    public float event_play_After_Trigger = 2f;
    public UnityEvent EventPlayAfterTrigger = new UnityEvent();


    public float event_play_After_Time = 5f;
    public UnityEvent EventPlayAfterTheTime = new UnityEvent();






    private void OnEnable()
    {
        onEnableEvent.Invoke();
        StartCoroutine(EventPlayAFterSomeTIme());
    }
    private void OnDisable()
    {
        onDisableEvent.Invoke();
    }



    private void OnTriggerEnter(Collider other)
    {
        if (!string.IsNullOrEmpty(tagName))
        {
            if ((tagName != null && !string.IsNullOrEmpty(tagName) && other.gameObject.CompareTag(tagName)))
            {
                onTriggerEnter.Invoke();
                StartCoroutine(EventPlayAfterTheTriggerHAppen());
            }
        }
        else
        {
            Debug.LogWarning("tagName is null or empty");
        }
    }


    IEnumerator EventPlayAfterTheTriggerHAppen()
    {
        yield return new WaitForSeconds(event_play_After_Trigger);
        EventPlayAfterTrigger.Invoke();
    }


    IEnumerator EventPlayAFterSomeTIme()
    {
        yield return new WaitForSeconds(event_play_After_Time);
        EventPlayAfterTheTime.Invoke();
    }

}
