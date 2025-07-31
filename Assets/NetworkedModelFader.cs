using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class NetworkedModelFader : NetworkBehaviour
{
    public GameObject currentModel;
    public GameObject newModel;
    public float fadeDuration = 2f;

    private void Start()
    {
        if (IsClient || IsServer)
        {
            // ��ʼ״̬�������пͻ��˺ͷ�����ִ�У�
            newModel.SetActive(false);
            SetModelAlpha(newModel, 0f);
            SetModelAlpha(currentModel, 1f);
        }
    }

    // �ͻ��˵������
    public void RequestFade()
    {
        StartFadeServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void StartFadeServerRpc()
    {
        StartCoroutine(FadeModels());
    }

    private IEnumerator FadeModels()
    {
        // ֪ͨ���пͻ��˼�����ģ��
        SetModelActiveClientRpc(true, true); // ������ģ��
        SetModelAlphaClientRpc(true, 0f);    // ��ʼ͸��

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / fadeDuration;

            // ���������ظ���
            SetModelAlpha(currentModel, 1f - progress);
            SetModelAlpha(newModel, progress);

            // ͬ�������пͻ���
            SetModelAlphaClientRpc(false, 1f - progress); // ��ģ�ͽ���
            SetModelAlphaClientRpc(true, progress);       // ��ģ�ͽ���

            yield return null;
        }

        // ��ɺ���þ�ģ��
        SetModelActiveClientRpc(false, false);
    }

    [ClientRpc]
    private void SetModelAlphaClientRpc(bool isNewModel, float alpha)
    {
        GameObject target = isNewModel ? newModel : currentModel;
        SetModelAlpha(target, alpha);
    }

    [ClientRpc]
    private void SetModelActiveClientRpc(bool isNewModel, bool active)
    {
        GameObject target = isNewModel ? newModel : currentModel;
        target.SetActive(active);
    }

    private void SetModelAlpha(GameObject model, float alpha)
    {
        if (model == null) return;

        foreach (var renderer in model.GetComponentsInChildren<Renderer>())
        {
            foreach (var material in renderer.materials)
            {
                Color color = material.color;
                color.a = alpha;
                material.color = color;

                if (alpha < 1f)
                {
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.EnableKeyword("_ALPHABLEND_ON");
                    material.renderQueue = 3000;
                }
            }
        }
    }
}