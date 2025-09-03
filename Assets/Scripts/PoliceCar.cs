using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class PoliceCar : MonoBehaviour
{
    public LensFlareComponentSRP Red;
    public LensFlareComponentSRP Blue;
    public float IntensitySpeed = 2f;
    public AudioSource PoliceSiern;
    private void OnEnable()
    {
        StartCoroutine(LensIntensity());
        PoliceSiern.Play();
    }

    IEnumerator LensIntensity()
    {
        float timer = 0f;

        while (true)
        {
            timer += Time.deltaTime * IntensitySpeed;
            float intensity = Mathf.PingPong(timer, 1f);
            Red.intensity = intensity;
            Blue.intensity = 1f - intensity;

            yield return null;
        }
    }
}
