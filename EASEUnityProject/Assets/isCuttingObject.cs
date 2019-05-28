using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class isCuttingObject : MonoBehaviour {

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.GetComponent<isCuttableObject>() != null && !collision.gameObject.GetComponent<isCuttableObject>().isCut)
        {
            //foreach (ContactPoint contact in collision.contacts)
            //{
            //    if (contact.otherCollider == collision.collider)
            //    {
                    //Debug.Log("collide: " + collision.gameObject.name);
                    GetComponentInChildren<VRObjectSlicer>().Slice(collision.gameObject);
                    return;
            //    }
            //}


        }
    }    
}
