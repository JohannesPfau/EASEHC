using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToPlunge : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Equals("Plunger"))
        {
            GameObject.Find("LevelLogic").SendMessage("onPlunge");
            AudioManager.playAudio("Plunger", gameObject);
        }
    }
}
