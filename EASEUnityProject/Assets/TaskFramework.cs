﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR.InteractionSystem;

public class TaskFramework : MonoBehaviour {

    public bool interactable;
    public GameObject go_TaskDone;
    public GameObject go_TaskCurrent;
    public GameObject go_TaskFuture;

    public GameObject TaskProgressBar;
    public GameObject TotalProgressBar;
    public GameObject TaskProgressTime;
    public GameObject TotalProgressTime;

    public GameObject bestIndicator;
    public GameObject bestIndicatorTotal;
    public GameObject bestStartP;
    public GameObject bestEndP;
    public GameObject bestTotalStartP;
    public GameObject bestTotalEndP;

    float taskTime;
    bool taskTimeCounting;
    float totalTime;
    bool totalTimeCounting;

    float bestTime = 2; // TODO: get empirically
    float bestTimeTotal = 20;
    float meanTime = 5;
    float meanTimeTotal = 50;

    List<string> taskList;

    // Use this for initialization
    void Start () {
		taskList = new List<string>();
        exampleInstructions1(); // TODO: Interface to tasks coming from NLP

        init();
        startCountTask();
        startCountTotal();
    }

    void startCountTask()
    {
        taskTime = Time.time;
        taskTimeCounting = true;
        float perc = bestTime / (meanTime * 2);
        bestIndicator.transform.position = bestStartP.transform.position + (bestEndP.transform.position - bestStartP.transform.position) * perc;
        resetTrackedEvents();
    }
    void startCountTotal()
    {
        totalTime = Time.time;
        totalTimeCounting = true;
        float perc = bestTimeTotal / (meanTimeTotal * 2);
        bestIndicatorTotal.transform.position = bestTotalStartP.transform.position + (bestTotalEndP.transform.position - bestTotalStartP.transform.position) * perc;
    }

    // Update is called once per frame
    void Update () {
        if(taskTimeCounting)
        {
            float t = Time.time - taskTime;
            int m = (((int)t) / 60);
            int s = ((int)t - m);
            int ms = (int)((t * 100f) -100*m -100*s);
            string str_m = m > 9 ? "" + m : "0" + m;
            string str_s = s > 9 ? "" + s : "0" + s;
            string str_ms = ms > 9 ? "" + ms : "0" + ms;
            TaskProgressTime.GetComponentInChildren<Text>().text = str_m + ":" + str_s + ":" + str_ms;
            TaskProgressBar.GetComponentsInChildren<Image>()[1].fillAmount = t / (meanTime * 2);
            if (t <= bestTime)
                TaskProgressBar.GetComponentsInChildren<Image>()[1].color = new Color(1, 221f / 255f, 0);
            else if (t <= meanTime)
                TaskProgressBar.GetComponentsInChildren<Image>()[1].color = new Color(135f / 255f, 1, 0);
            else
                TaskProgressBar.GetComponentsInChildren<Image>()[1].color = new Color(150f/225f, 150f / 225f, 150f / 225f);

        }
        if(totalTimeCounting)
        {
            float t = Time.time - totalTime;
            int m = (((int)t) / 60);
            int s = ((int)t - m);
            int ms = (int)((t * 100f) - 100 * m - 100 * s);
            string str_m = m > 9 ? "" + m : "0" + m;
            string str_s = s > 9 ? "" + s : "0" + s;
            string str_ms = ms > 9 ? "" + ms : "0" + ms;
            TotalProgressTime.GetComponentInChildren<Text>().text = str_m + ":" + str_s + ":" + str_ms;
            TotalProgressBar.GetComponentsInChildren<Image>()[1].fillAmount = t / (meanTimeTotal * 2);

            if (t <= bestTimeTotal)
                TotalProgressBar.GetComponentsInChildren<Image>()[1].color = new Color(1, 221f / 255f, 0);
            else if (t <= meanTimeTotal)
                TotalProgressBar.GetComponentsInChildren<Image>()[1].color = new Color(135f / 255f, 1, 0);
            else
                TotalProgressBar.GetComponentsInChildren<Image>()[1].color = new Color(150f / 225f, 150f / 225f, 150f / 225f);
        }

        if(Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("menu button pressed");
            proceed();
        }
    }

    void init()
    {
        foreach(string s in taskList)
            createNewTask(s, Task.TaskState.FUTURE);
    }

    void exampleInstructions1()
    {
        taskList.Add("Grab an apple.");   
        taskList.Add("Put it into a bowl.");
        taskList.Add("Cover it.");
    }
    public void createNewTask(string taskName, Task.TaskState taskState)
    {
        createNewTask(taskName, taskState, -1);
    }
    public void createNewTask(string taskName, Task.TaskState taskState, int siblingIndex)
    {
        GameObject go = null;
        switch (taskState)
        {
            case Task.TaskState.CURRENT:
                go = Instantiate(go_TaskCurrent, transform);
                break;
            case Task.TaskState.FUTURE:
                go = Instantiate(go_TaskFuture, transform);
                break;
            case Task.TaskState.DONE:
                go = Instantiate(go_TaskDone, transform);
                break;
        }
        go.GetComponent<Task>().setName(taskName);
        if (siblingIndex != -1)
            go.GetComponent<RectTransform>().SetSiblingIndex(siblingIndex); // remain position even though gameObjects change
        checkCurrentTask();
    }

    public Task[] getAllTasks()
    {
        return GetComponentsInChildren<Task>();
    }

    public bool checkCurrentTask()
    {
        if (!interactable)
            return true;
        foreach(Task t in getAllTasks())
        {
            if (t.taskState == Task.TaskState.CURRENT) // current already exists
                return true;
            if (t.taskState == Task.TaskState.FUTURE) // search 1st one
            {
                changeTaskState(t, Task.TaskState.CURRENT);
                return true;
            }
        }
        return false; // everything done
    }

    public void changeTaskState(Task oldTask, Task.TaskState newState)
    {
        createNewTask(oldTask.taskName, newState, oldTask.GetComponent<RectTransform>().GetSiblingIndex());
        oldTask.taskState = Task.TaskState.DELETED;
        Destroy(oldTask.gameObject);
        bool currentlyRunning = checkCurrentTask();
        if(!currentlyRunning)
        {
            taskTimeCounting = false;
            totalTimeCounting = false;
        }
    }

    public Task getCurrentTask()
    {
        foreach (Task t in getAllTasks())
            if (t.taskState == Task.TaskState.CURRENT)
                return t;
        return null;
    }

    public void proceed()
    {
        if(getCurrentTask())
            getCurrentTask().button_Check();
        startCountTask();
    }

    public GameObject nrActionsText;
    public GameObject trackedActionsPanel;
    public GameObject trackedActionPrefab;
    public Sprite PICKUPsprite;
    public Sprite PUTDOWNsprite;
    public Sprite COLLISIONsprite;
    public Sprite COLLISIONwhileHELDsprite;
    public Sprite COLLISIONEXITsprite;
    public Sprite dotsSprite;
    TrackingEvent lastTrackedEvent;
    public void showTrackedEvent(TrackingEvent te)
    {
        if (te.eventType == TrackingEvent.TrackingEventType.MOVEMENT)
            return;

        if (lastTrackedEvent != null && te.similarTo(lastTrackedEvent))
            return;
        lastTrackedEvent = te;

        nrActionsText.GetComponent<Text>().text = int.Parse(nrActionsText.GetComponent<Text>().text)+1 + "";

        GameObject go = Instantiate(trackedActionPrefab, trackedActionsPanel.transform);
        switch (te.eventType)
        {
            case TrackingEvent.TrackingEventType.PICKUP:
                go.GetComponent<Image>().sprite = PICKUPsprite;
                break;
            case TrackingEvent.TrackingEventType.PUTDOWN:
                go.GetComponent<Image>().sprite = PUTDOWNsprite;
                break;
            case TrackingEvent.TrackingEventType.COLLISION:
                go.GetComponent<Image>().sprite = COLLISIONsprite;
                break;
            case TrackingEvent.TrackingEventType.COLLISION_WHILE_HELD:
                go.GetComponent<Image>().sprite = COLLISIONwhileHELDsprite;
                break;
            case TrackingEvent.TrackingEventType.COLLISION_EXIT:
                go.GetComponent<Image>().sprite = COLLISIONEXITsprite;
                break;
            default:
                go.GetComponent<Image>().sprite = null;
                break;
        }
        string t = te.relatedObjects[0].GetComponentInChildren<InteractableVRObject>().displayedName;
        if (te.relatedObjects.Length > 1)
            if (te.relatedObjects[1].GetComponentInChildren<InteractableVRObject>())
                t += " and " + te.relatedObjects[1].GetComponentInChildren<InteractableVRObject>().displayedName;
        go.GetComponentInChildren<Text>().text = t;

        // overflow
        while(trackedActionsPanel.GetComponentsInChildren<Image>().Length > 10)
        {
            DestroyImmediate(trackedActionsPanel.GetComponentsInChildren<Image>()[0].gameObject);
            // new [0]
            trackedActionsPanel.GetComponentsInChildren<Image>()[0].sprite = dotsSprite;
            trackedActionsPanel.GetComponentsInChildren<Image>()[0].GetComponentInChildren<Text>().text = "";
        }
    }

    public void resetTrackedEvents()
    {
        foreach (Image i in trackedActionsPanel.GetComponentsInChildren<Image>())
            Destroy(i.gameObject);
    }
}
