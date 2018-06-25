using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachedTo : MonoBehaviour {

    public GameObject gameObjectToAttach;
    	
	// Update is called once per frame
	void Update () {
        if (gameObjectToAttach != null)
            transform.position = gameObjectToAttach.transform.position;

    }
}
