using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointOfVehicles : MonoBehaviour
{
    public GameObject Vehicle;
    public GameObject Door;
    public Transform CarSitPos;
    public Transform CarEnterPos;
    public Transform CarExitPos;
    public GameObject[] Totaldrivers;
    public GameObject driver;
    public GameObject CheckPoint;
    private void OnEnable()
    {
        Vehicle = transform.parent.gameObject;
        
        // Add bounds checking to prevent array index out of range exceptions
        if (Totaldrivers != null && Totaldrivers.Length > 0)
        {
            int gangsterIndex = GameManager.Instance.CurrentGangster;
            if (gangsterIndex >= 0 && gangsterIndex < Totaldrivers.Length)
            {
                driver = Totaldrivers[gangsterIndex];
            }
            else
            {
                Debug.LogWarning($"CheckPointOfVehicles: Invalid gangster index {gangsterIndex}, array length is {Totaldrivers.Length}");
                // Use the first driver as fallback
                if (Totaldrivers.Length > 0)
                {
                    driver = Totaldrivers[0];
                }
            }
        }
        else
        {
            Debug.LogError("CheckPointOfVehicles: Totaldrivers array is null or empty!");
        }

        foreach (Transform child in transform.parent)
        {

            if (child == transform) continue;

            DoorController doorController = child.GetComponent<DoorController>();
            Debug.Log(child.name);
            if (doorController != null)
            {
                Debug.Log(doorController.name);
                Door = doorController.Door.gameObject;
                Debug.Log(Door.name);
            }

            if (child.name == "CarSitPos")
            {
                CarSitPos = child;
            }

            if (child.name == "CarEnterPos")
            {
                CarEnterPos = child;
            }

            if (child.name == "CarExitPos")
            {
                CarExitPos = child;
            }

            CheckPoint = gameObject;
        }
    }
}
