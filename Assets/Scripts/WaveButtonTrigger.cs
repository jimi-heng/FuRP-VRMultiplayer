using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class WaveButtonTrigger : NetworkBehaviour
{
    public Renderer earthquakePlane;
    public Renderer earthquakePlane2;

    public void StartWave()
    {
        StartServerRpc();
    }

    [ServerRpc(RequireOwnership=false)]
    void StartServerRpc()
    {
        StartClientRpc();
    }

    [ClientRpc]
    void StartClientRpc()
    {
        float startTime = Time.time;
        earthquakePlane.material.SetFloat("_Trigger", startTime);
        earthquakePlane2.material.SetFloat("_Trigger", startTime);
    }
}
