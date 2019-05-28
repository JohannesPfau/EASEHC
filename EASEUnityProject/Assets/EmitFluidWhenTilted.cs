using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmitFluidWhenTilted : MonoBehaviour
{
    public float minXemitting;
    public float maxXemitting;
    public GameObject emitter;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.rotation.eulerAngles.x >= minXemitting && transform.rotation.eulerAngles.x <= maxXemitting)
        {
            emitter.SetActive(true);
            GameObject.Find("TrackingLogic").GetComponent<TrackingLogic>().levelGoals.Remove("COLLISION:Salatschuessel,Öl");
        }
        //else
        //    emitter.SetActive(false);
    }
}
