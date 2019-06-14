using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmitFluidWhenTilted : MonoBehaviour
{
    public float minXemitting;
    public float maxXemitting;
    public GameObject emitter;
    bool done;

    // Update is called once per frame
    void Update()
    {
        if (transform.rotation.eulerAngles.x >= minXemitting && transform.rotation.eulerAngles.x <= maxXemitting)
        {
            emitter.SetActive(true);
            if(!done)
            {
                GameObject.Find("TrackingLogic").GetComponent<TrackingLogic>().levelGoals.Remove("COLLISION:Salatschuessel,Öl"); //Task1
                GameObject.Find("TrackingLogic").GetComponent<TrackingLogic>().levelGoals.Remove("COLLISION:Öl,Pfanne"); //Task2

                switch (PlayerPrefs.GetInt("progress"))
                {
                    case 1:
                        GameObject.Find("TrackingLogic").GetComponent<TrackingLogic>().goalDisplay_level1();
                        break;
                    case 2:
                        GameObject.Find("TrackingLogic").GetComponent<TrackingLogic>().goalDisplay_level2();
                        break;
                }
                GameObject.Find("TrackingLogic").GetComponent<TrackingLogic>().checkDone();
                done = true;
            }
        }
        //else
        //    emitter.SetActive(false);
    }
}
