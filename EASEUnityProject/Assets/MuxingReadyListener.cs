using RockVR.Video;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuxingReadyListener : MonoBehaviour
{
    public GameObject toEnable;
    static bool done;
    public static void onMuxingReady()
    {
        done = true;
    }

    private void Update()
    {
        if(done)
        {
            toEnable.SetActive(true);
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
}
