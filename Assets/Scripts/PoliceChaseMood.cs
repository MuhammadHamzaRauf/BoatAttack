using System.Collections;
using UnityEngine;
using Gley.TrafficSystem;

public class PoliceChaseMood : MonoBehaviour
{
    public LayerMask roadLayer;

    private bool policeCarActive = false;
    private bool canActivatePolice = false;
    private GameObject currentPoliceCar;

    private void OnEnable()
    {
        if (GameManager.Instance == null)
            return;

        if (GameManager.Instance.CurrentMode == 1)
        {
            StartCoroutine(CheckPlayerInVehicle());
        }
    }

    private IEnumerator CheckPlayerInVehicle()
    {
        while (!UiManager.instance.isPLayerInCar && !UiManager.instance.isPLayerInBike)
        {
            yield return new WaitForSeconds(0.5f);
        }
        StartCoroutine(EnablePoliceAfterDelay(1f));
    }

    private IEnumerator EnablePoliceAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        canActivatePolice = true;

        if (!policeCarActive && GameManager.Instance.CurrentMode == 1)
        {
                        currentPoliceCar = Instantiate(
                            GamePlayHandler.instance.PoliceCar,
                            GamePlayHandler.instance.PoliceInstantiatePos.transform.position,
                            GamePlayHandler.instance.PoliceInstantiatePos.transform.rotation
                        );

                    
          
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (!canActivatePolice || policeCarActive)
            return;

        if (collision.gameObject.TryGetComponent<VehicleComponent>(out VehicleComponent VC))
        {
            if (GameManager.Instance.CurrentMode == 1)
            {
                Vector3 spawnPosition;
                int maxAttempts = 10;
                int attempt = 0;

                while (attempt < maxAttempts)
                {
                    if (FindSpawnPointNear(collision.transform.position, 20f, 50f, out spawnPosition))
                    {
                        Vector3 viewportPoint = Camera.main.WorldToViewportPoint(spawnPosition);

                        bool isInView = viewportPoint.z > 0 &&
                                        viewportPoint.x > 0 && viewportPoint.x < 1 &&
                                        viewportPoint.y > 0 && viewportPoint.y < 1;

                        if (!isInView)
                        {
                            currentPoliceCar = Instantiate(
                                GamePlayHandler.instance.PoliceCar,
                                spawnPosition,
                                Quaternion.identity
                            );

                            policeCarActive = true;
                            StartCoroutine(DisablePoliceCarAfterDelay(60f));
                            break;
                        }
                    }

                    attempt++;
                }
            }
        }
    }


    private IEnumerator DisablePoliceCarAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (currentPoliceCar != null)
        {
            Destroy(currentPoliceCar);
            currentPoliceCar = null;
        }

        policeCarActive = false;
    }

    private bool FindSpawnPointNear(Vector3 origin, float minRadius, float maxRadius, out Vector3 spawnPoint)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomDirection = Random.insideUnitSphere;
            randomDirection.y = 0;

            float randomDistance = Random.Range(minRadius, maxRadius);
            Vector3 testPosition = origin + randomDirection.normalized * randomDistance + Vector3.up * 10f;

            if (Physics.Raycast(testPosition, Vector3.down, out RaycastHit hit, 20f, roadLayer))
            {
                spawnPoint = hit.point;
                return true;
            }
        }

        spawnPoint = Vector3.zero;
        return false;
    }
}
