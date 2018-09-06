using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackingLogic : MonoBehaviour {

    public bool alwaysRecording;

    List<TrackingEvent> eventList;
    public bool recording;
    public GameObject player;
    public GameObject playerHandL;
    public GameObject playerHandR;
    public GameObject playerHead;

    public GameObject test;

    public float movementRecordDelay = 0.1f;
    float recordDelayed = 0;

    private void Start()
    {
        eventList = new List<TrackingEvent>();
        if(alwaysRecording)
        {
            Debug.Log("Tracking started");
            recording = true;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            Debug.Log("Tracking stopped");
            recording = false;
        }
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            Debug.Log("Tracking started");
            recording = true;
        }
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
    
    public void trackEvent(TrackingEvent.TrackingEventType type, params GameObject[] relatedObjects)
    {
        if (!recording)
            return;
        eventList.Add(new TrackingEvent(type, relatedObjects));
    }

    public void trackPositions()
    {
        if (!recording)
            return;
        eventList.Add(new TrackingEvent(TrackingEvent.TrackingEventType.MOVEMENT, player, playerHandL, playerHandR, playerHead));
    }

    public void displayList()
    {
        foreach (TrackingEvent evt in eventList)
        {
            if (evt.eventType == TrackingEvent.TrackingEventType.MOVEMENT) // dont display movement bc of visibility
                continue;

            for(int i = 0; i < evt.relatedObjects.Length; i++)
            {
                string locationdata = "";
                if (evt.objectPositions.Length >= i)
                    locationdata += " POS: " + evt.objectPositions[i] + " ROT: " + evt.objectRotations[i];

                Debug.Log(evt.timestamp + " (" + evt.eventType.ToString() + ") " + evt.relatedObjects[i].name + locationdata + "@:\"" + evt.currentTask +"\"");
            }
            Debug.Log(" - - - ");
        }
    }

    private void OnApplicationQuit()
    {
        if (alwaysRecording)
            displayList();
    }
}
