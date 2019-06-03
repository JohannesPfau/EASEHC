using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class transformableSteak : MonoBehaviour
{
    public float roastTimeUntilNextStage = 10f;
    public LevelOWLogic levellogic;
    public GameObject nextStageObject;
    bool onPlate;
    public GameObject steamFX;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(onPlate && levellogic.hotplate_on && nextStageObject)
        {
            roastTimeUntilNextStage -= Time.deltaTime;
            if (roastTimeUntilNextStage <= 0)
            {
                nextStageObject.SetActive(true);
                nextStageObject.transform.parent = null;
                nextStageObject.GetComponent<transformableSteak>().steamFX.GetComponent<ParticleSystem>().Play();
                gameObject.SetActive(false);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.gameObject.GetComponent<InteractableVRObject>().displayedName.Contains("Pfanne"))
        {
            onPlate = true;
            steamFX.GetComponent<ParticleSystem>().Play();
            GameObject.Find("TrackingLogic").GetComponent<TrackingLogic>().levelGoals.Remove("COLLISION:Steak,Pfanne"); //Task2
            GameObject.Find("TrackingLogic").GetComponent<TrackingLogic>().goalDisplay_level2();
        }

        if (collision.transform.gameObject.GetComponent<InteractableVRObject>().displayedName.Contains("Teller"))
        {
            onPlate = false;
            steamFX.GetComponent<ParticleSystem>().Stop();
        }
    }
    
}
