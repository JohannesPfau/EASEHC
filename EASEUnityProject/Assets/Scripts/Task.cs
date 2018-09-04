using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Task : MonoBehaviour {
    public string taskName;
    public TaskState taskState;
    
    public void setName(string name)
    {
        taskName = name;
        GetComponentInChildren<Text>().text = name;
    }

    public enum TaskState
    {
        CURRENT,
        DONE,
        FUTURE,
        DELETED
    }

    public void button_Check()
    {
        if (taskState != TaskState.CURRENT)
            return;

        GetComponentInParent<TaskFramework>().changeTaskState(this, TaskState.DONE);
    }
}
