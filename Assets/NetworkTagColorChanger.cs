using UnityEngine;
using Unity.Netcode;

public class NetworkTagColorChanger : NetworkBehaviour
{
    public string targetTag = "ColorTarget";  // Ŀ��Tag
    public Color hoverColor = Color.red;
    public Color defaultColor = Color.white;

    // ����ͬ����ɫ��������ʼĬ����ɫ
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

    // ���������տͻ������󣬸���ɫ
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

    // ������������XR Interactable�¼����ã��ͻ��˵��ã�
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
