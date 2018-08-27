using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackingLogic : MonoBehaviour {

    List<TrackingEvent> eventList;
    public bool recording;
    public GameObject player;
    public GameObject playerHandL;
    public GameObject playerHandR;

    public GameObject test;

    public float movementRecordDelay = 0.1f;
    float recordDelayed = 0;

    private void Start()
    {
        eventList = new List<TrackingEvent>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadMinus))
            recording = false;
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
            recording = true;
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
            displayList();

        if (recording && player != null && playerHandL != null && playerHandR != null && recordDelayed > movementRecordDelay)
        {
            recordDelayed = 0f;
            // track player movement
            trackPositions();
        }
        else
            recordDelayed += Time.deltaTime;
    }
    
    public void trackEvent(string type, params GameObject[] relatedObjects)
    {
        eventList.Add(new TrackingEvent(type, relatedObjects));
    }

    public void trackPositions()
    {
        eventList.Add(new TrackingEvent(TrackingEvent.TrackingEventType.MOVEMENT, player, playerHandL, playerHandR));
    }

    public void displayList()
    {
        foreach (TrackingEvent evt in eventList)
        {
            for(int i = 0; i < evt.relatedObjects.Length; i++)
            {
                string locationdata = "";
                if (evt.objectPositions.Length >= i)
                {
                    locationdata += " POS: " + evt.objectPositions[i] + " ROT: " + evt.objectRotations[i];
                }

                Debug.Log(evt.timestamp + " (" + evt.eventType.ToString() + ") " + evt.relatedObjects[i].name + locationdata);
            }
        }
    }
}
