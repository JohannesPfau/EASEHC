using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuxiliaryFunctions : MonoBehaviour {
    
    public GameObject getPhysicalParent(GameObject go, string goTag)
    {
        if (go.tag.Contains(goTag))
            return go;
        if (go.transform.parent == null)
            return null;
        return getPhysicalParent(go.transform.parent.gameObject, goTag);
    }

    public bool isAttachedToHand(GameObject go)
    {
        if(go.transform.parent == null)
            return false;

        if (go.name.Contains("Hand"))
            return true;

        return isAttachedToHand(go.transform.parent.gameObject);
    }
}
