using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveCar : MonoBehaviour
{
    public GameObject[] cars;

    public void activecar()
    {
        // Add bounds checking to prevent array index out of range exceptions
        if (cars != null && cars.Length > 0)
        {
            int playerIndex = GameManager.Instance.CurrentPlayer;
            if (playerIndex >= 0 && playerIndex < cars.Length)
            {
                cars[playerIndex].SetActive(true);
            }
            else
            {
                Debug.LogWarning($"ActiveCar: Invalid player index {playerIndex}, array length is {cars.Length}");
                // Activate the first car as fallback
                if (cars.Length > 0)
                {
                    cars[0].SetActive(true);
                }
            }
        }
        else
        {
            Debug.LogError("ActiveCar: cars array is null or empty!");
        }
    }
}
