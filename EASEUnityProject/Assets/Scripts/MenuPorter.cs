using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPorter : MonoBehaviour {

    public void cmd_PortToLevel1()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Prolog");
    }
}
