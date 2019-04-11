using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class RatingSystemLevelLogic : MonoBehaviour
{
    public bool isTwoVideoSystem;
    public string video1url;
    public string video2url;
    public VideoPlayer videoPlayer1;
    public VideoPlayer videoPlayer2;

    public GameObject videoImage;
    public GameObject video2Image;
    public GameObject pauseObj;
    public GameObject playObj;
    public GameObject[] stars;
    public Material starEnabledMat;
    public Material starDisabledMat;
    public GameObject doneText;
    public GameObject[] hearts;

    bool started;

    private void Start()
    {
        videoPlayer1.url = video1url; // TODO: get from playerprefs
        if (isTwoVideoSystem)
            videoPlayer2.url = video2url;
    }

    private void Update()
    {
        if(doneText && AuxiliaryFunctions.isGripButtonPressed())
        {
            // switch to next one
        }
    }

    public void Pause()
    {
        pauseObj.SetActive(false);
        playObj.SetActive(true);

        videoPlayer1.playbackSpeed = 0;
        if(isTwoVideoSystem)
            videoPlayer2.playbackSpeed = 0;
    }

    public void Play()
    {
        pauseObj.SetActive(true);
        playObj.SetActive(false);

        if(!started)
        {
            videoImage.SetActive(true);
            if(isTwoVideoSystem)
                video2Image.SetActive(true);
            videoPlayer1.Play();
            if (isTwoVideoSystem)
                videoPlayer2.Play();
            started = true;
        }

        videoPlayer1.playbackSpeed = 1;
        if (isTwoVideoSystem)
            videoPlayer2.playbackSpeed = 1;
    }

    public void Replay()
    {
        started = false;
        Play();
    }

    public void Zeitlupe()
    {
        if (videoPlayer1.playbackSpeed > 0.125f)
        {
            videoPlayer1.playbackSpeed = videoPlayer1.playbackSpeed / 2;
            if (isTwoVideoSystem)
                videoPlayer2.playbackSpeed = videoPlayer1.playbackSpeed;
        }
    }

    public void Zeitraffer()
    {
        if (videoPlayer1.playbackSpeed < 8)
        {
            videoPlayer1.playbackSpeed = videoPlayer1.playbackSpeed * 2;
            if (isTwoVideoSystem)
                videoPlayer2.playbackSpeed = videoPlayer1.playbackSpeed;
        }
    }

    public void showStars(int count)
    {
        for (int i = 0; i < count; i++)
            stars[i].GetComponent<MeshRenderer>().material = starEnabledMat;
        for (int i = count; i < 5; i++)
            stars[i].GetComponent<MeshRenderer>().material = starDisabledMat;
        if (started)
            doneText.SetActive(true);
    }

    public void star1()
    {
        showStars(1);
    }
    public void star2()
    {
        showStars(2);
    }
    public void star3()
    {
        showStars(3);
    }
    public void star4()
    {
        showStars(4);
    }
    public void star5()
    {
        showStars(5);
    }

    public void rate1()
    {
        hearts[0].SetActive(true);
        hearts[1].SetActive(false);
    }
    public void rate2()
    {
        hearts[1].SetActive(true);
        hearts[0].SetActive(false);

        if(started)
            doneText.SetActive(true);
    }
}
