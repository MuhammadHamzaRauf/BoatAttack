using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Invector.vCharacterController;

public class PlayerActivation : MonoBehaviour
{
    public GameObject[] Charcters;
    public Avatar[] Avatars;


    private void OnEnable()
    {
        // Add bounds checking to prevent array index out of range exceptions
        if (Charcters != null && Charcters.Length > 0)
        {
            int gangsterIndex = GameManager.Instance.CurrentGangster;
            if (gangsterIndex >= 0 && gangsterIndex < Charcters.Length)
            {
                Charcters[gangsterIndex].SetActive(true);
            }
            else
            {
                Debug.LogWarning($"PlayerActivation: Invalid gangster index {gangsterIndex}, array length is {Charcters.Length}");
                // Activate the first character as fallback
                if (Charcters.Length > 0)
                {
                    Charcters[0].SetActive(true);
                }
            }
        }
        else
        {
            Debug.LogError("PlayerActivation: Charcters array is null or empty!");
        }
        
        if (Avatars != null && Avatars.Length > 0)
        {
            if (gameObject.TryGetComponent<vThirdPersonController>(out vThirdPersonController TPC))
            {
                int gangsterIndex = GameManager.Instance.CurrentGangster;
                if (gangsterIndex >= 0 && gangsterIndex < Avatars.Length)
                {
                    gameObject.GetComponent<Animator>().avatar = Avatars[gangsterIndex];
                }
                else
                {
                    Debug.LogWarning($"PlayerActivation: Invalid gangster index {gangsterIndex} for Avatars, array length is {Avatars.Length}");
                    // Use the first avatar as fallback
                    if (Avatars.Length > 0)
                    {
                        gameObject.GetComponent<Animator>().avatar = Avatars[0];
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning("PlayerActivation: Avatars array is null or empty!");
        }
    }
}
