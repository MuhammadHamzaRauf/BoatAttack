using UnityEngine;
using System.Collections;

public class DoorController : MonoBehaviour
{
    public GameObject Door;               
    public float rotationDuration = .5f;    
    public float rotationAngle = 90f;     
    public float waitBeforeClose = 1f;    

    private bool isRotating = false;

    public void RotateDoor()
    {
        if (!isRotating)
        {
            StartCoroutine(RotateAndReset());
        }
    }

    private IEnumerator RotateAndReset()
    {
        isRotating = true;

        Quaternion originalRotation = Door.transform.rotation;

        Quaternion targetRotation = originalRotation * Quaternion.Euler(0f, rotationAngle, 0f);

        yield return StartCoroutine(RotateOverTime(originalRotation, targetRotation, rotationDuration));

        yield return new WaitForSeconds(waitBeforeClose);

        yield return StartCoroutine(RotateOverTime(Door.transform.rotation, originalRotation, rotationDuration));

        isRotating = false;
    }

    private IEnumerator RotateOverTime(Quaternion from, Quaternion to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            Door.transform.rotation = Quaternion.Slerp(from, to, t);
            yield return null;
        }
        Door.transform.rotation = to;
    }
}
