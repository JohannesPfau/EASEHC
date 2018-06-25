using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class emptyFromMe : AuxiliaryFunctions
{

    public GameObject stickObject;
    public float maxDist;
    public float scaleSize = -1;

    //private void OnTriggerEnter(Collider collision)
    //{
    //    GameObject physical = getPhysicalParent(collision.gameObject);
    //    if (physical != null && physical.transform.parent == stickObject.transform)
    //    {
    //        Debug.Log("Empty: " + physical.name + " from " + stickObject.name);
    //        //physical.GetComponent<Rigidbody>().useGravity = false;
    //        //physical.GetComponent<Rigidbody>().isKinematic = true;
    //        physical.transform.localScale = physical.transform.localScale * 3;
    //        physical.transform.parent = null;
    //    }
    //}

    public void Update()
    {
        Rigidbody[] RBs = stickObject.GetComponentsInChildren<Rigidbody>();
        foreach(Rigidbody rb in RBs)
        {
            emptyMe(rb.gameObject);
        }
        List<GameObject> gos = stickObject.GetComponent<stickToMe>().stickedGameObjects;
        foreach (GameObject go in gos)
        {
            emptyMe(go.gameObject);
        }
    }

    public void emptyMe(GameObject go)
    {
        float dist = Vector3.Distance(go.transform.position, stickObject.transform.position);
        if (dist >= maxDist)
        {
            Debug.Log("Empty: " + go.name + " from " + stickObject.name);
            if(scaleSize != -1)
                if (go.tag.Contains("Cloth"))
                    go.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            go.transform.parent = null;
            if (stickObject.GetComponent<stickToMe>().stickedGameObjects.Contains(go))
                stickObject.GetComponent<stickToMe>().stickedGameObjects.Remove(go);
        }
    }
}
