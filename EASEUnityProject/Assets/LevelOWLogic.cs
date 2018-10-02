using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class LevelOWLogic : LevelLogic {

    // Use this for initialization

    void Start()
    {
        base.Start();

        Destroy(GameObject.Find("WashingMachine_inside").GetComponent<CircularDrive>());
        Destroy(GameObject.Find("WashingMachine_inside").GetComponent<Interactable>());
        GameObject.Find("WashingMachine_inside").GetComponent<Animator>().SetTrigger("Start");
        
    }

    // Update is called once per frame
    void Update()
    {
        //base.Update();
    }

    public void onWashingLoadEnter()
    {

    }

    int plungeProgress = 0;
    public void onPlunge()
    {
        if (progress == 0)
        {
            GameObject.Find("Plunge" + plungeProgress).SetActive(false);
            plungeProgress++;
            if (plungeProgress > 8)
            {
                cmd_addProgress();
            }
        }
    }
}
