using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class TaskFramework : MonoBehaviour {

    public bool interactable;
    public GameObject go_TaskDone;
    public GameObject go_TaskCurrent;
    public GameObject go_TaskFuture;

    List<string> taskList;

    // Use this for initialization
    void Start () {
		taskList = new List<string>();
        exampleInstructions1(); // TODO: Interface to tasks coming from NLP

        init();
    }
	
	// Update is called once per frame
	void Update () {
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
        checkCurrentTask();
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
    }

}
