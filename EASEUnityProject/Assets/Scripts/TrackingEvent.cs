using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackingEvent : MonoBehaviour {

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
        if(eventType == te.eventType && relatedObjects.Length > 1)
            if((te.relatedObjects[0] == relatedObjects[0] && te.relatedObjects[1] == relatedObjects[1]) ||
               (te.relatedObjects[0] == relatedObjects[1] && te.relatedObjects[1] == relatedObjects[0]))
                return true;

        return false;
    }

    public enum TrackingEventType
    {
        MOVEMENT,
        PICKUP,
        COLLISION,
        COLLISION_WHILE_HELD,
        COLLISION_EXIT,
        PUTDOWN,
        USAGE,
        THROW //?
    }
}
