using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class TornadoEdgeFade : MonoBehaviour
{
    [Header("基础配置")]
    public Texture2D gradientTexture;
    public bool flipVertical = false;

    [Header("边缘效果")]
    [Range(0, 0.5f)] public float fadeWidth = 0.2f;
    [Range(0, 1)] public float edgeBrightness = 0.5f;

    [Header("自发光")]
    [ColorUsage(true, true)] public Color emissionColor = Color.white;
    [Range(0, 10)] public float emissionIntensity = 1f;

    void Start() => RefreshAll();

#if UNITY_EDITOR
    [ContextMenu("刷新效果")]
#endif
    public void RefreshAll()
    {
        ApplyGradient();
        ApplyEdgeFade();
        ApplyEmission();
    }

    void ApplyGradient()
    {
        if (gradientTexture == null) return;

        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        float segmentHeight = 1f / renderers.Length;

        foreach (var rend in renderers)
        {
            Material mat = rend.sharedMaterial ?? new Material(Shader.Find("Universal Render Pipeline/Lit"));

            // UV渐变计算
            int index = rend.transform.GetSiblingIndex();
            float startV = flipVertical ? 1 - (index + 1) * segmentHeight : index * segmentHeight;

            mat.SetTexture("_BaseMap", gradientTexture);
            mat.SetVector("_BaseMap_ST", new Vector4(1, segmentHeight, 0, startV));
            rend.sharedMaterial = mat;
        }
    }

    void ApplyEdgeFade()
    {
        foreach (var rend in GetComponentsInChildren<Renderer>())
        {
            Material mat = rend.sharedMaterial;
            mat.EnableKeyword("_EDGE_FADE_ON");
            mat.SetFloat("_FadeWidth", fadeWidth);
            mat.SetFloat("_EdgeBrightness", edgeBrightness);
        }
    }

    void ApplyEmission()
    {
        foreach (var rend in GetComponentsInChildren<Renderer>())
        {
            Material mat = rend.sharedMaterial;
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", emissionColor * emissionIntensity);
            mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
        }
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (!EditorApplication.isPlaying)
            RefreshAll();
    }
#endif
}