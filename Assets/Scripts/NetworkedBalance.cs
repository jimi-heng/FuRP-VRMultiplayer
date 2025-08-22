using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

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
    private NetworkVariable<bool> isCheck = new NetworkVariable<bool>(false);

    private float currentAngle = 0f;

    public void StartTest()
    {
        if (IsServer)
        {
            isCheck.Value = true;
            Invoke(nameof(RefreshCheck), 1f);
        }
        else
        {
            changeCheckServerRpc();
        }
    }

    [ServerRpc(RequireOwnership =false)]
    private void changeCheckServerRpc()
    {
        isCheck.Value = true;
        Invoke(nameof(RefreshCheck), 1f);
    }

    void RefreshCheck()
    {
        isCheck.Value = false;
    }

    public void EndTest(SelectEnterEventArgs args)
    {
        if (IsServer)
        {
            isCheck.Value = true;
            Rigidbody rb = args.interactableObject.transform.gameObject.GetComponent<Rigidbody>();
            leftTray.RemoveRb(rb);
            rightTray.RemoveRb(rb);

            Invoke(nameof(RefreshCheck), 1f);
        }
        else
        {
            var netObj = args.interactableObject.transform.gameObject.GetComponent<NetworkObject>();
            changeCheckEndServerRpc(netObj.NetworkObjectId);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void changeCheckEndServerRpc(ulong networkObjectId)
    {
        isCheck.Value = true;
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out NetworkObject netObj))
        {
            Rigidbody rb = netObj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                leftTray.RemoveRb(rb);
                rightTray.RemoveRb(rb);
            }
        }

        Invoke(nameof(RefreshCheck), 1f);
    }

        void Update()
        {
            if (isCheck.Value)
            {
                if (IsServer)
                {
                    float leftMass = leftTray.GetTotalMass();
                    float rightMass = rightTray.GetTotalMass();

                    float targetAngle = Mathf.Clamp((rightMass - leftMass) * balanceSensitivity, -maxAngle, maxAngle);

                    syncedAngle.Value = targetAngle;
                }

                currentAngle = Mathf.Lerp(currentAngle, syncedAngle.Value, Time.deltaTime * lerpSpeed);
                beam.localRotation = Quaternion.Euler(0, -currentAngle, 0);
            }
        }
    }
