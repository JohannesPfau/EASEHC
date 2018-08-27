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

    public TrackingEvent(string type, GameObject[] relatedObjects)
    {
        eventType = (TrackingEventType)Enum.Parse(typeof(TrackingEventType), type);
        timestamp = DateTime.Now;
        this.relatedObjects = relatedObjects;

        objectPositions = new Vector3[relatedObjects.Length];
        int i = 0;
        foreach(GameObject go in relatedObjects)
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

    public TrackingEvent(TrackingEventType type, params GameObject[] relatedObjects)
    {
        eventType = type;
        timestamp = DateTime.Now;
        this.relatedObjects = relatedObjects;


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

    public enum TrackingEventType
    {
        MOVEMENT,
        PICKUP,
        COLLISION,
        COLLISION_WHILE_HELD,
        PUTDOWN,
        THROW
    }
}
