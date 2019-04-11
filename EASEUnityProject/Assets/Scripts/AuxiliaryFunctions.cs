using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

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

    static SteamVR_Controller.Device controllerL;
    static SteamVR_Controller.Device controllerR;
    public static bool isGripButtonPressed()
    {
        if (controllerL == null && GameObject.Find("Hand1") && GameObject.Find("Hand1").GetComponent<Hand>())
            controllerL = GameObject.Find("Hand1").GetComponent<Hand>().controller;
        if (controllerR == null && GameObject.Find("Hand2") && GameObject.Find("Hand2").GetComponent<Hand>())
            controllerR = GameObject.Find("Hand2").GetComponent<Hand>().controller;
        if (controllerL != null && controllerL.GetPressDown(Valve.VR.EVRButtonId.k_EButton_Grip) || (controllerR != null && controllerR.GetPressDown(Valve.VR.EVRButtonId.k_EButton_Grip)))
            return true;
        return false;
    }
}
