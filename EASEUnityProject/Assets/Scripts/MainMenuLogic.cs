using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR.InteractionSystem;

public class MainMenuLogic : LevelLogic {

    bool started;
    public GameObject[] texts;
    public GameObject cam;

	// Use this for initialization
	void Start ()
    {
        base.Start();
        Level4Logic.turnOnWashingMachine();

        DialogSys.showMultipleChoice("story", "cmd_1a", "open world", "cmd_1b", "quit", "cmd_1c");

    }

    public void cmd_1a()
    {
        SceneManager.LoadScene("Level1_VR");
    }
    public void cmd_1b()
    {
        SceneManager.LoadScene("OpenWorld");
    }
    public void cmd_1c()
    {
        Application.Quit();
    }

    // Update is called once per frame
    void Update () {
        base.Update();
		//if(!started && Input.GetKeyDown(KeyCode.Return))
  //      {
  //          started = true;
  //          foreach(GameObject text in texts)
  //              text.GetComponent<Animator>().SetTrigger("FadeOut");
  //          cam.GetComponent<Animator>().SetTrigger("Start");
  //      }
	}

    public void onWashingLoadEnter()
    {

    }


}
