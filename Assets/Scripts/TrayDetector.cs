using System.Collections.Generic;
using UnityEngine;

public class TrayDetector : MonoBehaviour
{
    public List<Rigidbody> objectsOnTray = new List<Rigidbody>();

     private void OnTriggerEnter(Collider other)
    {
        if (!IsServer()) return;

        Rigidbody rb = other.attachedRigidbody;
        if (rb!= null && !objectsOnTray.Contains(rb))
        {
            objectsOnTray.Add(rb);
        }
    }

    public void RemoveRb(Rigidbody rb)
    {
        if (!IsServer()) return;
        if (objectsOnTray.Contains(rb)) 
        { 
            objectsOnTray.Remove(rb);
        }
    }

    float GetMass(Rigidbody rb)
    {
        return rb != null ? rb.mass : 0f;
    }

    public float GetTotalMass()
    {
        float total = 0f;
        foreach (var rb in objectsOnTray)
        {
            if (rb != null)
                total += GetMass(rb);
        }
        return total;
    }
    bool IsServer()
    {
        return Unity.Netcode.NetworkManager.Singleton.IsServer;
    }
}
