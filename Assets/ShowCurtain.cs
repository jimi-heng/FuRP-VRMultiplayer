using Unity.Netcode;
using UnityEngine;

public class ShowCurtain : NetworkBehaviour
{
    public Renderer Curtain;
    public void moveCurtain()
    {
        moveServerRpc();
    }

    [ServerRpc(RequireOwnership =false)]
    void moveServerRpc()
    {
        moveClientRpc();
    }

    [ClientRpc]
    void moveClientRpc()
    {
        float trigger=Time.time;
        Curtain.material.SetFloat("_trigger", trigger);
    }
}
