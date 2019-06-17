using RockVR.Video;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
    public GameObject doneStars;
    public GameObject[] tStarsWon;
    public GameObject[] aStarsWon;
    public GameObject doneText;

    // Use this for initialization
    void Start () {
        switch(PlayerPrefs.GetInt("progress"))
        {
            case -1: // tutorial
                TaskText.text = "Tutorial";
                TaskDescriptionText.text = "Sieh Dich in der virtuellen Umgebung gut um.\r\n\r\n" +
                                           "In den folgenden Leveln wirst Du Aufgaben erhalten, die mit den Gegenstaenden in dieser Wohnung zu tun haben.\r\n\r\n" +
                                           "Wenn Du fertig bist, sag dem Experimentleiter einfach Bescheid.";
                PlayerPrefs.SetString("currentTaskScene", "RatingEvaluation_Tutorial");
                if(doneText && isProcessingScene)
                    doneText.SetActive(true);
                break;
            case 0:
                TaskText.text = "Tischdecken fuer 2";
                TaskDescriptionText.text = "1. Decke 2 <color=yellow>Teller</color> auf den Tisch.\r\n\r\n" +
                                           "2. Decke 2 <color=yellow>Glaeser</color> auf den Tisch.\r\n\r\n" +
                                           "3. Decke 2 <color=yellow>Gabeln</color> auf den Tisch.\r\n\r\n" +
                                           "4. Decke 2 <color=yellow>Messer</color> auf den Tisch.";
                PlayerPrefs.SetString("currentTaskScene", "RatingEvaluation_Task0");
                break;
            case 1:
                TaskText.text = "Gurkensalat";
                TaskDescriptionText.text = "1. Schneide eine <color=yellow>Gurke</color> in Scheiben.\r\n\r\n" +
                                            "2. Lege die Scheiben in eine <color=yellow>Schuessel</color>.\r\n\r\n" +
                                            "3. Begiesse den Salat mit <color=yellow>Öl</color>.";
                PlayerPrefs.SetString("currentTaskScene", "RatingEvaluation_Task1");
                break;
            case 2:
                TaskText.text = "Steak";
                TaskDescriptionText.text = "1. Stelle eine <color=yellow>Pfanne</color> auf den Herd.\r\n\r\n" +
                                            "2. Erhitze die <color=yellow>Herdplatte</color>.\r\n\r\n" +
                                            "3. Gib etwas <color=yellow>Öl</color> in die Pfanne.\r\n\r\n" +
                                            "4. Lege ein <color=yellow>Steak</color> dazu.\r\n\r\n" +
                                            "5. Brate es auf den gewuenschten Grad und lege es dann auf den <color=yellow>Teller</color>.\r\n\r\n"; 
                PlayerPrefs.SetString("currentTaskScene", "RatingEvaluation_Task2");
                break;
        }
        loadVDDsAndCalculateMeans();
        if (isProcessingScene)
            displayScore();
    }
	
	// Update is called once per frame
	void Update () {
        if ((!isProcessingScene || PlayerPrefs.GetInt("progress") == -1) && AuxiliaryFunctions.isGripButtonPressed())
            progress();

        if (Input.GetKeyDown(KeyCode.Escape))
            SceneManager.LoadScene("RatingEvaluation_KITCHEN_CLASH_VR");
    }

    float bestTime;
    float meanTime;
    float meanNrOfActions;
    void loadVDDsAndCalculateMeans()
    {
        string vddmPath = Application.persistentDataPath + "/VideoDescriptionDataManager.json";

        bestTime = Mathf.Infinity;

        if (!File.Exists(vddmPath))
            Debug.Log("No VideoDescriptionDataManager found.");
        else
        {
            VideoDescriptionDataManager vddM = JsonUtility.FromJson<VideoDescriptionDataManager>(File.ReadAllText(vddmPath));
            int count = 0;

            // filter out different tasks
            foreach (string vddpath in vddM.videoDescriptionDataFiles)
            {
                VideoDescriptionData vdd = JsonUtility.FromJson<VideoDescriptionData>(File.ReadAllText(Application.persistentDataPath + "/" + vddpath));
                if (vdd.sceneName == PlayerPrefs.GetString("currentTaskScene"))
                {
                    meanTime += vdd.secondsSpent;
                    meanNrOfActions += vdd.nrOfActionsSpent;
                    if (vdd.secondsSpent < bestTime)
                        bestTime = vdd.secondsSpent;
                    count++;
                }
            }
            if (count > 0)
            {
                if (count == 1)
                {
                    meanTime *= 2;
                    meanNrOfActions *= 2;
                }
                else
                {
                    meanTime = meanTime / count;
                    meanNrOfActions = meanNrOfActions / count;
                }
                displayBestAndMeanTimes(bestTime, meanTime, meanNrOfActions);
            }
            else
                Debug.Log("No prior executions of this task found.");
        }
    }

    void displayBestAndMeanTimes(float bestTime, float meanTime, float meanNrOfActions)
    {
        GameObject.Find("BestTimeText").GetComponent<Text>().text = AuxiliaryFunctions.timeToString(bestTime);
        GameObject.Find("MeanTimeText").GetComponent<Text>().text = AuxiliaryFunctions.timeToString(meanTime);
        GameObject.Find("NrActionsText").GetComponent<Text>().text = (int)meanNrOfActions + "";
    }

    void displayScore()
    {
        if(PlayerPrefs.GetInt("progress") == -1) // tutorial
        {
            TaskDescriptionText.text = "Erledige die Aufgaben der naechsten Level moeglichst schnell, mit wenig Arbeitsschritten und ueberzeugend.\r\n\r\n" +
                "Deine Ausfuehrung wird nach jedem Level bewertet!";
            return;
        }

        int nrActions = PlayerPrefs.GetInt("nrOfActionsSpent");
        float time = PlayerPrefs.GetFloat("secondsSpent");

        TaskDescriptionText.text = "Geschafft!\r\n\r\n"
        + "<size=25>Deine Zeit:</size>\r\n" + AuxiliaryFunctions.timeToString(time) + "\r\n\r\n\r\n"
        + "<size=25> Deine Aktionen:</size>\r\n" + nrActions;
        doneStars.SetActive(true);

        for (int i = 0; i < PlayerPrefs.GetInt("TimeScore"); i++)
            tStarsWon[i].SetActive(true);
        for (int i = 0; i < PlayerPrefs.GetInt("ActionsScore"); i++)
            aStarsWon[i].SetActive(true);
    }

    void progress()
    {
        if(PlayerPrefs.GetInt("progress") == -1 && isProcessingScene)
        {
            PlayerPrefs.SetInt("progress", 0);
            if (VideoCaptureCtrl.instance)
                Destroy(VideoCaptureCtrl.instance.gameObject);
            PlayerPrefs.SetString("currentTaskScene", "TASK_SCENE");
        }
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
