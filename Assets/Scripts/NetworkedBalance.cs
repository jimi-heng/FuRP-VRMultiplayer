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
    public float time = 1f;

    private NetworkVariable<float> syncedAngle = new NetworkVariable<float>(
        0f,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );
    private float currentAngle = 0f;

    private float leftMass = 0f;
    private float rightMass = 0f;

    public void StartTest()
    {
        if (IsServer)
        {
            leftTray.enter = true;
            rightTray.enter = true;
            Invoke(nameof(RefreshCheck), time);
        }
        else
        {
            changeCheckServerRpc();
        }
    }

    [ServerRpc(RequireOwnership =false)]
    private void changeCheckServerRpc()
    {
        leftTray.enter = true;
        rightTray.enter=true;
        Invoke(nameof(RefreshCheck), time);
    }

    void RefreshCheck()
    {
        leftTray.enter=false;
        rightTray.enter=false;
    }

    public void EndTest(SelectEnterEventArgs args)
    {
        if (IsServer)
        {
            Rigidbody rb = args.interactableObject.transform.gameObject.GetComponent<Rigidbody>();
            leftTray.RemoveRb(rb);
            rightTray.RemoveRb(rb);
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
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out NetworkObject netObj))
        {
            Rigidbody rb = netObj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                leftTray.RemoveRb(rb);
                rightTray.RemoveRb(rb);
            }
        }
    }

        void Update()
        {
                if (IsServer)
                {
                    leftMass =  leftTray.GetTotalMass();
                    rightMass = rightTray.GetTotalMass();

                    float targetAngle = Mathf.Clamp((rightMass - leftMass) * balanceSensitivity, -maxAngle, maxAngle);

                    syncedAngle.Value = targetAngle;
                }

                currentAngle = Mathf.Lerp(currentAngle, syncedAngle.Value, Time.deltaTime * lerpSpeed);
                beam.localRotation = Quaternion.Euler(0, -currentAngle, 0);
            }
        
    }
