using UnityEngine;
using Unity.Netcode;

public class NetworkTagColorChanger : NetworkBehaviour
{
    public string targetTag = "ColorTarget";  // 目标Tag
    public Color hoverColor = Color.red;
    public Color defaultColor = Color.white;

    // 网络同步颜色变量，初始默认颜色
    private NetworkVariable<Color> syncedColor = new NetworkVariable<Color>(Color.white);

    private void OnEnable()
    {
        syncedColor.OnValueChanged += OnColorChanged;
    }

    private void OnDisable()
    {
        syncedColor.OnValueChanged -= OnColorChanged;
    }

    private void OnColorChanged(Color oldColor, Color newColor)
    {
        ApplyColor(newColor);
    }

    private void ApplyColor(Color color)
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag(targetTag);
        foreach (var obj in targets)
        {
            Renderer rend = obj.GetComponent<Renderer>();
            if (rend != null)
                rend.material.color = color;
        }
    }

    // 服务器接收客户端请求，改颜色
    [ServerRpc(RequireOwnership = false)]
    public void ChangeColorServerRpc()
    {
        syncedColor.Value = hoverColor;
    }

    [ServerRpc(RequireOwnership = false)]
    public void ResetColorServerRpc()
    {
        syncedColor.Value = defaultColor;
    }

    // 这两个方法供XR Interactable事件调用（客户端调用）
    public void ChangeColor()
    {
        if (IsServer)
        {
            syncedColor.Value = hoverColor;
        }
        else
        {
            ChangeColorServerRpc();
        }
    }

    public void ResetColor()
    {
        if (IsServer)
        {
            syncedColor.Value = defaultColor;
        }
        else
        {
            ResetColorServerRpc();
        }
    }
}
