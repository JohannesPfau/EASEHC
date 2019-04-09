using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TaskSceneLogic : MonoBehaviour {

    public Text TaskDescriptionText;
    public Text TaskText;
    public Text bestTimeText;
    public Text meanTimeText;
    public Text nrActionsText;

    // Use this for initialization
    void Start () {
        switch(PlayerPrefs.GetInt("progress"))
        {
            case 0:
                TaskText.text = "Tischdecken fuer 2";
                TaskDescriptionText.text = "1. Decke 2 <color=yellow>Teller</color> auf den Tisch.\r\n\r\n" +
                                           "2. Decke 2 <color=yellow>Glaeser</color> auf den Tisch.\r\n\r\n" +
                                           "3. Decke 2 <color=yellow>Gabeln</color> auf den Tisch.\r\n\r\n" +
                                           "4. Decke 2 <color=yellow>Messer</color> auf den Tisch.";
                bestTimeText.text = "00:00:00";
                meanTimeText.text = "00:00:00";
                nrActionsText.text = "0";
                PlayerPrefs.SetString("currentTaskScene", "RatingEvaluation_Task0");
                //PlayerPrefs.SetInt("")
                break;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void progress()
    {
        StartCoroutine(LoadSceneAsync());
    }

    IEnumerator LoadSceneAsync()
    {
        //AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("OpenWorld");
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(PlayerPrefs.GetString("currentTaskScene"));
        while (!asyncLoad.isDone)
            yield return null;
    }
}
