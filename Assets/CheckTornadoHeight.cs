using UnityEngine;

public class CheckTornadoHeight : MonoBehaviour
{
    void Start()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        Bounds totalBounds = new Bounds();
        bool first = true;

        foreach (Renderer rend in renderers)
        {
            if (first)
            {
                totalBounds = rend.bounds;
                first = false;
            }
            else
            {
                totalBounds.Encapsulate(rend.bounds); // ��չ�߽��԰��������Ӷ���
            }
        }

        Debug.Log($"�ܸ߶�: {totalBounds.size.y} ��λ");
        Debug.Log($"�ײ�Y����: {totalBounds.min.y}");
        Debug.Log($"����Y����: {totalBounds.max.y}");
    }
}