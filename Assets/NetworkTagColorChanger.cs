using UnityEngine;
using Unity.Netcode;

public class NetworkTagColorChanger : NetworkBehaviour
{
    public string targetTag = "ColorTarget"; 
    public Color hoverColor = Color.red;
    public Color defaultColor = Color.white;

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
        GameObject[] targets = GameObject.FindGameObjectsWithTag("position");

        foreach (var obj in targets)
        {
            Renderer[] childRenderers = obj.GetComponentsInChildren<Renderer>(true); 

            foreach (Renderer rend in childRenderers)
            {
                if (rend.gameObject.CompareTag(targetTag))
                {
                    rend.material.color = color;
                }
            }
        }
    }

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
