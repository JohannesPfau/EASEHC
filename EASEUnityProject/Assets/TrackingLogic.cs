using RockVR.Video;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR.InteractionSystem;

public class TrackingLogic : MonoBehaviour {
    public int levelNr = 0;

    public bool alwaysRecording;

    List<TrackingEvent> eventList;
    public bool recording;
    public GameObject player;
    public GameObject playerHandL;
    public GameObject playerHandR;
    public GameObject playerHead;

    public GameObject test;
    public GameObject DONE_text;
    bool done;
    public GameObject GoalReachedStar_prefab;

    public List<string> levelGoals;

    public float movementRecordDelay = 0.1f;
    public float actionRecordTreshold = 0.5f;
    float recordDelayed = 0;
    float actionRecordDelayed = 0;
    public SteamVR_Controller.Device controllerL;
    public SteamVR_Controller.Device controllerR;

    public Camera dedicatedCaptureCamera;

    private void Start()
    {
        eventList = new List<TrackingEvent>();
        if(alwaysRecording)
        {
            Debug.Log("Tracking started");
            recording = true;
            VideoCaptureCtrl.instance.StartCapture();
            dedicatedCaptureCamera.fieldOfView = 43;
            dedicatedCaptureCamera.transform.localPosition = Vector3.zero;
            dedicatedCaptureCamera.transform.localRotation = Quaternion.identity;
        }
        PlayerPrefs.SetInt("progress", levelNr);
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
            UnityEngine.SceneManagement.SceneManager.LoadScene("RatingEvaluation_Rating1Scene");
            VideoCaptureCtrl.instance.StopCapture();
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

        if (relatedObjects.Length > 1) // filter out collisions with itself (e.g. @cutting)
            for (int i = 1; i < relatedObjects.Length; i++)
                if (relatedObjects[0].GetComponentInChildren<InteractableVRObject>() && relatedObjects[i].GetComponentInChildren<InteractableVRObject>() && relatedObjects[0].GetComponentInChildren<InteractableVRObject>().displayedName == relatedObjects[i].GetComponentInChildren<InteractableVRObject>().displayedName)
                    return;

        if (type == TrackingEvent.TrackingEventType.COLLISION && relatedObjects.Length > 1) // filter out collisions with non-VR-objects
            for (int i = 0; i < relatedObjects.Length; i++)
                if (!relatedObjects[i].GetComponentInChildren<InteractableVRObject>())
                    return;

        TrackingEvent te = new TrackingEvent(type, relatedObjects);
        eventList.Add(te);
        if(GameObject.Find("TaskFramework"))
            GameObject.Find("TaskFramework").GetComponent<TaskFramework>().showTrackedEvent(te);
        checkGoal(te);

        // play sound
        switch(type)
        {
            case TrackingEvent.TrackingEventType.COLLISION:
                AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("TrackedActions/Collision"), Camera.main.transform.position);
                break;
            case TrackingEvent.TrackingEventType.COLLISION_WHILE_HELD:
                AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("TrackedActions/CollisionWhileHeld"), Camera.main.transform.position);
                break;
            case TrackingEvent.TrackingEventType.PICKUP:
                AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("TrackedActions/Pickup"), Camera.main.transform.position);
                break;
            case TrackingEvent.TrackingEventType.USAGE:
                AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("TrackedActions/Usage"), Camera.main.transform.position);
                break;
        }
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
        foreach (GameObject go in te.relatedObjects)
            if (!go.GetComponent<InteractableVRObject>())
                return;

        // not e.g. twice the same knife for tablesetting:
        if (te.relatedObjects[0].GetComponentInChildren<InteractableVRObject>().usedForGoalCompletion && !te.relatedObjects[0].GetComponentInChildren<InteractableVRObject>().canBeUsedMultipleTimesForGoalCompletion)
            return;
        if (te.relatedObjects.Length > 1 && te.relatedObjects[1].GetComponentInChildren<InteractableVRObject>().usedForGoalCompletion && !te.relatedObjects[1].GetComponentInChildren<InteractableVRObject>().canBeUsedMultipleTimesForGoalCompletion)
            return;

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
                        GameObject star = Instantiate(GoalReachedStar_prefab, GameObject.Find("DialogCanvasVR").transform);
                        AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("TrackedActions/GoalReachedStar"), Camera.main.transform.position);
                        Destroy(star, 1.5f);
                        s = goal;
                        foreach (GameObject go in te.relatedObjects)
                            go.GetComponent<InteractableVRObject>().usedForGoalCompletion = true;
                        break;
                    }
                // vice versa:
                if(objs.Length > 1 && te.relatedObjects[0].GetComponentInChildren<InteractableVRObject>().displayedName.ToLower().Contains(objs[1].ToLower()))
                    if(te.relatedObjects.Length > 1 && te.relatedObjects[1].GetComponentInChildren<InteractableVRObject>() && te.relatedObjects[1].GetComponentInChildren<InteractableVRObject>().displayedName.ToLower().Contains(objs[0].ToLower()))
                    {
                        Debug.Log("Goal " + goal + " accomplished!");
                        GameObject star = Instantiate(GoalReachedStar_prefab, GameObject.Find("DialogCanvasVR").transform);
                        AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("TrackedActions/GoalReachedStar"), Camera.main.transform.position);
                        Destroy(star, 1.5f);
                        s = goal;
                        foreach (GameObject go in te.relatedObjects)
                            go.GetComponent<InteractableVRObject>().usedForGoalCompletion = true;
                        break;
                    }
            }
        }
        if (s != "")
            levelGoals.Remove(s);
        if (levelGoals.Count == 0 && !done)
        {
            DONE_text.SetActive(true);
            AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("TrackedActions/Done"), Camera.main.transform.position);
            done = true;
        }

        switch(PlayerPrefs.GetInt("progress"))
        {
            case 0:
                goalDisplay_level0();
                break;
        }
    }

    void goalDisplay_level0()
    {
        string t = "";
        if (levelGoals.Contains("COLLISION:Tisch,Teller"))
            t += "1. Decke 2 <color=yellow>Teller</color> auf den Tisch.\r\n\r\n";
        else
            t += "<color=grey>1. Decke 2 Teller auf den Tisch.</color>\r\n\r\n";

        if (levelGoals.Contains("COLLISION:Tisch,Glas"))
            t += "2. Decke 2 <color=yellow>Glaeser</color> auf den Tisch.\r\n\r\n";
        else
            t += "<color=grey>2. Decke 2 Glaeser auf den Tisch.</color>\r\n\r\n";

        if (levelGoals.Contains("COLLISION:Tisch,Gabel"))
            t += "3. Decke 2 <color=yellow>Gabeln</color> auf den Tisch.\r\n\r\n";
        else
            t += "<color=grey>3. Decke 2 Gabeln auf den Tisch.</color>\r\n\r\n";

        if (levelGoals.Contains("COLLISION:Tisch,Messer"))
            t += "4. Decke 2 <color=yellow>Messer</color> auf den Tisch.";
        else
            t += "<color=grey>4. Decke 2 Messer auf den Tisch.</color>";

        GameObject.Find("TaskDescriptionText").GetComponent<Text>().text = t;
    }
}
