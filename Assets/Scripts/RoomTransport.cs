using UnityEngine;

public class RoomTransport : MonoBehaviour
{
    public Transform head;
    public Transform origin;
    public Transform room1;
    public Transform room2;
    public Transform room3;

    public void Transform1()
    {
        Vector3 offset = head.position - origin.position;
        origin.position = room1.position-offset;
    }
}
