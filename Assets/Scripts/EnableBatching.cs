using UnityEngine;
using UnityEngine.Rendering.Universal;

[CreateAssetMenu(fileName = "EnableBatching", menuName = "Tools/Enable Dynamic Batching")]
public class EnableBatching : MonoBehaviour
{
    public UniversalRenderPipelineAsset urpAsset;

    void Start()
    {
        if (urpAsset != null)
        {
            urpAsset.supportsDynamicBatching = true;
            Debug.Log("Dynamic Batching Enabled");
        }
    }
}