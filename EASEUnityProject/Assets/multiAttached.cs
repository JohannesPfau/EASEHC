using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class multiAttached : MonoBehaviour
{
    public void detachMe()
    {
        if (GetComponent<Rigidbody>())
        {
            GetComponent<Rigidbody>().useGravity = true;
            GetComponent<Rigidbody>().isKinematic = false;
        }
        foreach (Collider c in GetComponentsInChildren<Collider>())
            c.enabled = true;
        Destroy(this);
    }

    public void detachMeAfter(float f)
    {
        transform.parent = null;
        Invoke("detachMe", f);
    }
}
