using Unity.Netcode;
using UnityEngine;

public class NetworkedBalance : NetworkBehaviour
{
    public TrayDetector leftTray;
    public TrayDetector rightTray;
    public Transform beam;

    public float maxAngle = 30f;
    public float balanceSensitivity = 5f;
    public float lerpSpeed = 2f;

    private NetworkVariable<float> syncedAngle = new NetworkVariable<float>(
        0f,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    private float currentAngle = 0f;

    void Update()
    {
        if (IsServer)
        {
            float leftMass = leftTray.GetTotalMass();
            float rightMass = rightTray.GetTotalMass();

            float targetAngle = Mathf.Clamp((rightMass - leftMass) * balanceSensitivity, -maxAngle, maxAngle);

            syncedAngle.Value = targetAngle; // 服务器写入同步角度
        }

        // 客户端/服务器：根据同步的角度平滑插值旋转
        currentAngle = Mathf.Lerp(currentAngle, syncedAngle.Value, Time.deltaTime * lerpSpeed);
        beam.localRotation = Quaternion.Euler(0,-currentAngle,0);
    }
}
