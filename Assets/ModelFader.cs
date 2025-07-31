using UnityEngine;
using System.Collections;

public class ModelFader : MonoBehaviour
{
    public GameObject currentModel;  // 当前模型
    public GameObject newModel;      // 新模型
    public float fadeDuration = 2f;  // 渐变时间（秒）

    void Start()
    {
        // 初始状态：新模型完全透明
        newModel.SetActive(false);
        SetModelAlpha(newModel, 0f);
    }

    // 外部调用（如按钮点击）
    public void StartFade()
    {
        StartCoroutine(FadeModels());
    }

    IEnumerator FadeModels()
    {
        // 激活新模型（初始透明）
        newModel.SetActive(true);

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / fadeDuration; // 0 → 1

            // 当前模型渐隐（Alpha: 1 → 0）
            SetModelAlpha(currentModel, 1f - progress);

            // 新模型渐显（Alpha: 0 → 1）
            SetModelAlpha(newModel, progress);

            yield return null; // 等待下一帧
        }

        // 渐变完成后，禁用旧模型（可选）
        currentModel.SetActive(false);
    }

    // 设置模型及其所有子物体的透明度
    void SetModelAlpha(GameObject model, float alpha)
    {
        foreach (Renderer renderer in model.GetComponentsInChildren<Renderer>())
        {
            foreach (Material mat in renderer.materials)
            {
                Color color = mat.color;
                color.a = alpha;
                mat.color = color;

                // 关键！启用材质透明混合模式
                if (alpha < 1f)
                {
                    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    mat.EnableKeyword("_ALPHABLEND_ON");
                    mat.renderQueue = 3000; // Transparent渲染队列
                }
            }
        }
    }
}