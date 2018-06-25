using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moppingWater : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name.Equals("BroomBottom"))
        {
            //transform.Translate(0, -0.05f, 0);
            //if(transform.localPosition.y < -0.5f)
            //{
                GameObject.Find("LevelLogic").SendMessage("onBroomed");
                Destroy(gameObject);
            //}
        }
    }
}
