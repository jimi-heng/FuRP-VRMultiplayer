using UnityEngine;
using System.Collections;
using Unity.Netcode;

public class MoveUp : NetworkBehaviour
{
    public float moveDistance = 2f;  // �ƶ��ľ���
    public float moveDuration = 1f;  // �ƶ���ʱ�䣨�룩

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
            yield return null; // ����һ֡
        }

        transform.position = targetPos;
        isMoving.Value = false;
    }
}
