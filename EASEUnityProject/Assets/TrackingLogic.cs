using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR.InteractionSystem;

public class TrackingLogic : MonoBehaviour {

    public bool alwaysRecording;

    List<TrackingEvent> eventList;
    public bool recording;
    public GameObject player;
    public GameObject playerHandL;
    public GameObject playerHandR;
    public GameObject playerHead;

    public GameObject test;
    public GameObject DONE_text;

    public List<string> levelGoals;

    public float movementRecordDelay = 0.1f;
    public float actionRecordTreshold = 0.5f;
    float recordDelayed = 0;
    float actionRecordDelayed = 0;
    public SteamVR_Controller.Device controllerL;
    public SteamVR_Controller.Device controllerR;

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
        if (controllerL == null && GameObject.Find("Hand1") && GameObject.Find("Hand1").GetComponent<Hand>())
            controllerL = GameObject.Find("Hand1").GetComponent<Hand>().controller;
        if (controllerR == null && GameObject.Find("Hand2") && GameObject.Find("Hand2").GetComponent<Hand>())
            controllerR = GameObject.Find("Hand2").GetComponent<Hand>().controller;

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

        actionRecordDelayed += Time.deltaTime;

        if(levelGoals.Count == 0 && (controllerL != null && controllerL.GetPressDown(Valve.VR.EVRButtonId.k_EButton_Grip) || (controllerR != null && controllerR.GetPressDown(Valve.VR.EVRButtonId.k_EButton_Grip))))
        {
            // done
            PlayerPrefs.SetInt("progress", PlayerPrefs.GetInt("progress") + 1);
            UnityEngine.SceneManagement.SceneManager.LoadScene("TASK_SCENE");
        }
    }
    
    public void trackEvent(TrackingEvent.TrackingEventType type, params GameObject[] relatedObjects)
    {
        if (!recording)
            return;

        if (actionRecordDelayed < actionRecordTreshold)
            return;
        if(type == TrackingEvent.TrackingEventType.COLLISION)
            actionRecordDelayed = 0;

        TrackingEvent te = new TrackingEvent(type, relatedObjects);
        eventList.Add(te);
        if(GameObject.Find("TaskFramework"))
            GameObject.Find("TaskFramework").GetComponent<TaskFramework>().showTrackedEvent(te);
        checkGoal(te);
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

    void checkGoal(TrackingEvent te)
    {
        string s = "";
        foreach(string goal in levelGoals)
        {
            string type = goal.Split(":".ToCharArray())[0];
            if(type == te.eventType.ToString())
            {
                string[] objs = goal.Split(":".ToCharArray())[1].Split(",".ToCharArray());
                if (te.relatedObjects[0].GetComponentInChildren<InteractableVRObject>().displayedName.ToLower().Contains(objs[0].ToLower()))        // TODO: Contains replaced by ontological objects?
                    if(objs.Length == 1 || te.relatedObjects[1].GetComponentInChildren<InteractableVRObject>().displayedName.ToLower().Contains(objs[1].ToLower()))
                    {
                        Debug.Log("Goal " + goal + " accomplished!");
                        s = goal;
                        break;
                    }
                // other way around:
                if(objs.Length > 1 && te.relatedObjects[0].GetComponentInChildren<InteractableVRObject>().displayedName.ToLower().Contains(objs[1].ToLower()))
                    if(te.relatedObjects[1].GetComponentInChildren<InteractableVRObject>().displayedName.ToLower().Contains(objs[0].ToLower()))
                    {
                        Debug.Log("Goal " + goal + " accomplished!");
                        s = goal;
                        break;
                    }
            }
        }
        if (s != "")
            levelGoals.Remove(s);
        if (levelGoals.Count == 0)
            DONE_text.SetActive(true);
    }
}
