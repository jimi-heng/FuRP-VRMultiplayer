using UnityEngine;
using System.Collections;
using Unity.Netcode;

public class MoveUp : NetworkBehaviour
{
    public float moveDistance = 2f;  // 移动的距离
    public float moveDuration = 1f;  // 移动的时间（秒）

    private NetworkVariable<bool> isMoving = new NetworkVariable<bool>(false);

    public void StartMove()
    {
        StartMoveServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    void StartMoveServerRpc()
    {
        StartMoveClientRpc();
    }

    [ClientRpc]
    void StartMoveClientRpc()
    {
        if (!isMoving.Value)
        {
            StartCoroutine(MoveUpCoroutine());
        }
    }

    IEnumerator MoveUpCoroutine()
    {
        isMoving.Value = true;

        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + Vector3.up * moveDistance;

        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / moveDuration;
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null; // 等下一帧
        }

        transform.position = targetPos;
        isMoving.Value = false;
    }
}
