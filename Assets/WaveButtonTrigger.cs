using UnityEngine;
using UnityEngine.UI;

public class WaveButtonTrigger : MonoBehaviour
{
    public Renderer earthquakePlane;
    public Renderer earthquakePlane2;

    public void StartWave()
    {
        float startTime = Time.time;
        earthquakePlane.material.SetFloat("_Trigger", startTime);
        earthquakePlane2.material.SetFloat("_Trigger", startTime);
    }
}
