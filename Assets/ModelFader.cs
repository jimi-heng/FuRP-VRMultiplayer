using UnityEngine;
using System.Collections;

public class ModelFader : MonoBehaviour
{
    public GameObject currentModel;  // ��ǰģ��
    public GameObject newModel;      // ��ģ��
    public float fadeDuration = 2f;  // ����ʱ�䣨�룩

    void Start()
    {
        // ��ʼ״̬����ģ����ȫ͸��
        newModel.SetActive(false);
        SetModelAlpha(newModel, 0f);
    }

    // �ⲿ���ã��簴ť�����
    public void StartFade()
    {
        StartCoroutine(FadeModels());
    }

    IEnumerator FadeModels()
    {
        // ������ģ�ͣ���ʼ͸����
        newModel.SetActive(true);

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / fadeDuration; // 0 �� 1

            // ��ǰģ�ͽ�����Alpha: 1 �� 0��
            SetModelAlpha(currentModel, 1f - progress);

            // ��ģ�ͽ��ԣ�Alpha: 0 �� 1��
            SetModelAlpha(newModel, progress);

            yield return null; // �ȴ���һ֡
        }

        // ������ɺ󣬽��þ�ģ�ͣ���ѡ��
        currentModel.SetActive(false);
    }

    // ����ģ�ͼ��������������͸����
    void SetModelAlpha(GameObject model, float alpha)
    {
        foreach (Renderer renderer in model.GetComponentsInChildren<Renderer>())
        {
            foreach (Material mat in renderer.materials)
            {
                Color color = mat.color;
                color.a = alpha;
                mat.color = color;

                // �ؼ������ò���͸�����ģʽ
                if (alpha < 1f)
                {
                    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    mat.EnableKeyword("_ALPHABLEND_ON");
                    mat.renderQueue = 3000; // Transparent��Ⱦ����
                }
            }
        }
    }
}