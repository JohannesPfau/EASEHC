using RockVR.Video;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

public class MuxingReadyListener : MonoBehaviour
{
    public GameObject toEnable;
    static bool done;
    static string _videoname;
    static string _filePath;
    bool serializingDone;
    public static void onMuxingReady(string videoname, string filePath)
    {
        done = true;
        _videoname = videoname;
        _filePath = filePath;
    }

    private void Update()
    {
        if(done)
        {
            toEnable.SetActive(true);

            //persistence
            if(!serializingDone)
                serialize();

            if (AuxiliaryFunctions.isGripButtonPressed())
                proceed();
        }

        if(Input.GetKeyDown(KeyCode.Return)) // debug
            proceed();
    }

    void proceed()
    {
        done = false;
        if (VideoCaptureCtrl.instance)
            Destroy(VideoCaptureCtrl.instance.gameObject);

        PlayerPrefs.SetInt("progress", PlayerPrefs.GetInt("progress") + 1);
        if(PlayerPrefs.GetInt("progress") < 3)
            UnityEngine.SceneManagement.SceneManager.LoadScene("TASK_SCENE");
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene("RatingEvaluation_KITCHEN_CLASH_VR");

    }

    void serialize()
    {
        serializingDone = true;

        string vddmPath = Application.persistentDataPath + "/VideoDescriptionDataManager.json";
        string vddName = _videoname + ".json";

        VideoDescriptionData vdd = new VideoDescriptionData(_videoname, _filePath, PlayerPrefs.GetString("currentTaskScene"), PlayerPrefs.GetInt("nrOfActionsSpent"), PlayerPrefs.GetFloat("secondsSpent"));
        VideoDescriptionDataManager vddM;
        if (!File.Exists(vddmPath))
        {
            vddM = new VideoDescriptionDataManager();
            vddM.videoDescriptionDataFiles = new string[] { vddName };
        }
        else
        {
            vddM = JsonUtility.FromJson<VideoDescriptionDataManager>(File.ReadAllText(vddmPath));
            List<string> vdds = vddM.videoDescriptionDataFiles.OfType<string>().ToList();
            vdds.Add(vddName);
            vddM.videoDescriptionDataFiles = vdds.ToArray<string>();
        }

        File.WriteAllText(Application.persistentDataPath + "/" + vddName, JsonUtility.ToJson(vdd, true));
        File.WriteAllText(vddmPath, JsonUtility.ToJson(vddM, true));
    }
}
