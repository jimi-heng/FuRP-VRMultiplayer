using UnityEngine;

public class WaterSurface : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        Floating floatingScript = other.GetComponent<Floating>();
        if (floatingScript != null)
        {
            floatingScript.SetFloating();
        }
    }
}
