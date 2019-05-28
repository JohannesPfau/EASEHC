using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TaskSceneLogic : MonoBehaviour {

    public bool isProcessingScene;
    public Text TaskDescriptionText;
    public Text TaskText;
    public Text bestTimeText;
    public Text meanTimeText;
    public Text nrActionsText;

    // Use this for initialization
    void Start () {
        switch(PlayerPrefs.GetInt("progress"))
        {
            case -1: // tutorial
                TaskText.text = "Tutorial";
                TaskDescriptionText.text = "Sieh Dich in der virtuellen Umgebung gut um.\r\n\r\n" +
                                           "In den folgenden Leveln wirst Du Aufgaben erhalten, die mit den Gegenstaenden in dieser Wohnung zu tun haben.\r\n\r\n" +
                                           "Wenn Du fertig bist, sag dem Experimentleiter einfach Bescheid.";
                bestTimeText.text = "00:00:00";
                meanTimeText.text = "00:00:00";
                nrActionsText.text = "0";
                PlayerPrefs.SetString("currentTaskScene", "RatingEvaluation_Tutorial");

                break;
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
                break;
            case 1:
                TaskText.text = "Gurkensalat";
                TaskDescriptionText.text =  "1. Schneide eine <color=yellow>Gurke</color> in Scheiben.\r\n\r\n"+
                                            "2. Lege die Scheiben in eine <color=yellow>Schuessel</color>.\r\n\r\n"+
                                            "3. Begiesse den Salat mit <color=yellow>Öl</color>.";
                bestTimeText.text = "00:00:00";
                meanTimeText.text = "00:00:00";
                nrActionsText.text = "0";
                PlayerPrefs.SetString("currentTaskScene", "RatingEvaluation_Task1");
                break;
        }
        if (isProcessingScene)
            TaskDescriptionText.text = "Geschafft!\r\n\r\n";
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
