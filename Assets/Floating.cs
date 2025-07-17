using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class Floating : MonoBehaviour
{
    public float underWaterDrag = 3f;
    public float underWaterAngularDrag = 1f;
    public float airDrag = 0f;
    public float airAngularDrag = 0.05f;
    public float floatingPower = 15f;

    Rigidbody stone;

    public GameObject water;

    bool underwater=false;

    void Start()
    {
        stone=GetComponent<Rigidbody>();
    }

    public void SetFloating()
    {
        float difference = transform.position.y - (water.transform.position.y+water.transform.localScale.y/2);

        if(difference < 0)
        {
            stone.AddForceAtPosition(Vector3.up * floatingPower * Mathf.Abs(difference), transform.position, ForceMode.Force);
            if (!underwater)
            {
                underwater = true;
                SwitchState(true);
            }
        }
        else if (underwater)
        {
            underwater = false;
            SwitchState(false);
        }
    }

    void SwitchState(bool isUnderwater)
    {
        if (isUnderwater)
        {
            stone.linearDamping = underWaterDrag;
            stone.angularDamping = underWaterAngularDrag;
        }
        else 
        {
            stone.linearDamping = airDrag;
            stone.angularDamping = airAngularDrag;
        }
    }

}
