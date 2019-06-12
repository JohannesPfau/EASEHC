using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class TrackingEvent {

    public DateTime timestamp;
    public TrackingEventType eventType;
    public GameObject[] relatedObjects;
    public Vector3[] objectPositions;
    public Vector3[] objectRotations;
    public string currentTask;
    
    public TrackingEvent(TrackingEventType type, params GameObject[] relatedObjects)
    {
        eventType = type;
        timestamp = DateTime.Now;
        this.relatedObjects = relatedObjects;
        if (GameObject.Find("TaskFramework") && GameObject.Find("TaskFramework").GetComponent<TaskFramework>().getCurrentTask())
            currentTask = GameObject.Find("TaskFramework").GetComponent<TaskFramework>().getCurrentTask().taskName;
        else
            currentTask = "";

        objectPositions = new Vector3[relatedObjects.Length];
        int i = 0;
        foreach (GameObject go in relatedObjects)
        {
            Vector3 position = go.transform.position;
            objectPositions[i] = position;
            i++;
        }

        objectRotations = new Vector3[relatedObjects.Length];
        i = 0;
        foreach (GameObject go in relatedObjects)
        {
            Vector3 rotation = go.transform.rotation.eulerAngles;
            objectRotations[i] = rotation;
            i++;
        }
    }

    public bool similarTo(TrackingEvent te)
    {
        if (te == null || relatedObjects == null || relatedObjects.Length < 2 || te.relatedObjects == null || te.relatedObjects.Length < 2 || te.relatedObjects[0] == null || te.relatedObjects[1] == null || relatedObjects[0] == null || relatedObjects[1] == null
            || !relatedObjects[0].GetComponentInChildren<InteractableVRObject>() || !relatedObjects[1].GetComponentInChildren<InteractableVRObject>() || !te.relatedObjects[0].GetComponentInChildren<InteractableVRObject>() || !te.relatedObjects[1].GetComponentInChildren<InteractableVRObject>())
            return false;
        if(eventType == te.eventType && relatedObjects.Length > 1)
        {
            //Debug.Log("Comparing " + te.relatedObjects[0].GetComponentInChildren<InteractableVRObject>().displayedName + " with " + relatedObjects[0].GetComponentInChildren<InteractableVRObject>().displayedName + " and " + te.relatedObjects[1].GetComponentInChildren<InteractableVRObject>().displayedName + " with " + relatedObjects[1].GetComponentInChildren<InteractableVRObject>().displayedName);
            if ((te.relatedObjects[0].GetComponentInChildren<InteractableVRObject>().displayedName == relatedObjects[0].GetComponentInChildren<InteractableVRObject>().displayedName && te.relatedObjects[1].GetComponentInChildren<InteractableVRObject>().displayedName == relatedObjects[1].GetComponentInChildren<InteractableVRObject>().displayedName) ||
               (te.relatedObjects[0].GetComponentInChildren<InteractableVRObject>().displayedName == relatedObjects[1].GetComponentInChildren<InteractableVRObject>().displayedName && te.relatedObjects[1].GetComponentInChildren<InteractableVRObject>().displayedName == relatedObjects[0].GetComponentInChildren<InteractableVRObject>().displayedName))
                return true;
        }
        // topf topf apfel apfel
        return false;
    }

    override public string ToString()
    {
        string s = "(" + timestamp + ") " + eventType;
        for(int i=0; i < relatedObjects.Length; i++)
            if(relatedObjects[i].GetComponentInChildren<InteractableVRObject>())
                s += " | " + relatedObjects[i].GetComponentInChildren<InteractableVRObject>().displayedName + " (pos):" + objectPositions[i] + " (rot):" + objectRotations[i];
            else
                s += " | " + relatedObjects[i].name + " (pos):" + objectPositions[i] + " (rot):" + objectRotations[i];
        return s;
    }

    public enum TrackingEventType
    {
        MOVEMENT,
        PICKUP,
        COLLISION,
        COLLISION_WHILE_HELD, // eg knife with fruit
        COLLISION_EXIT,
        PUTDOWN,
        USAGE, // eg pressing button
        THROW //?
    }
}
