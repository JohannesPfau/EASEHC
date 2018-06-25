using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class onWashingLoadEnter : MonoBehaviour {

    public GameObject clothSpawn;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag.Contains("Cloth"))
        {
            GameObject.Find("LevelLogic").SendMessage("onWashingLoadEnter");
            Destroy(collision.gameObject.GetComponent<InteractableVRObject>());
            Destroy(collision.gameObject.GetComponent<Interactable>());
            collision.gameObject.transform.position = clothSpawn.transform.position;
            collision.gameObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            collision.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            collision.gameObject.tag = "Physical";
            //collision.gameObject.GetComponent<Rigidbody>().useGravity = false;
            //collision.gameObject.GetComponent<Rigidbody>().isKinematic = true;
        }        
    }
}
