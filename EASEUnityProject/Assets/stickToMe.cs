using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class stickToMe : AuxiliaryFunctions
{

    public string goTag;
    public List<GameObject> stickedGameObjects;
    public float scaleSize = -1;

    private void Start()
    {
        stickedGameObjects = new List<GameObject>();
    }

    private void OnTriggerEnter(Collider collision)
    {
        //Debug.Log("collision with: " + collision.gameObject.name);
        GameObject physical = getPhysicalParent(collision.gameObject, goTag);
        if (physical != null && physical.transform.parent != transform && !isAttachedToHand(physical))
        {
            Debug.Log("Stick: " + physical.name + " to " + gameObject.name);
            //physical.GetComponent<Rigidbody>().useGravity = false;
            //physical.GetComponent<Rigidbody>().isKinematic = true;
            if(scaleSize != -1)
                physical.transform.localScale = new Vector3(scaleSize, scaleSize, scaleSize);
            physical.transform.parent = transform;
        }
    }
    
}
