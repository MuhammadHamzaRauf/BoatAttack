using System.Collections;
using GangsterMafia.Constants;
using UnityEngine;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{
    public Image fillImage;
    public float loadDuration = 7f;

    void OnEnable()
    {
        StartCoroutine(FillLoadingBar());
    }

    IEnumerator FillLoadingBar()
    {
        float elapsed = 0f;
        fillImage.fillAmount = 0f;

        while (elapsed < loadDuration)
        {
            elapsed += Time.deltaTime;
            fillImage.fillAmount = Mathf.Clamp01(elapsed / loadDuration);
            yield return null;
        }
        fillImage.fillAmount = 1f;
        // GangsterMafia.Core.SceneManager.Instance.LoadScene(GameConstants.GAMEPLAY_SCENE);
    }
}
