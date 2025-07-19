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
                totalBounds.Encapsulate(rend.bounds); // 扩展边界以包含所有子对象
            }
        }

        Debug.Log($"总高度: {totalBounds.size.y} 单位");
        Debug.Log($"底部Y坐标: {totalBounds.min.y}");
        Debug.Log($"顶部Y坐标: {totalBounds.max.y}");
    }
}