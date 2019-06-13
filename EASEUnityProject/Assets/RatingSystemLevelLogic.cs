using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class RatingSystemLevelLogic : MonoBehaviour
{
    public bool isTwoVideoSystem;
    VideoDescriptionData vdd1;
    VideoDescriptionData vdd2;

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
    int starsSelected; // 1 ... 5
    int heartSelected; // 1 if video1, -1 if video2

    public int MAX_RATINGS = 10;
    int ratingsDone = 0;

    List<VideoDescriptionData> currentVDDs;

    private void Start()
    {
        //get available videos from persistence
        deserialize();

        if(vdd1 != null)
            videoPlayer1.url = vdd1.filename; // set by serialized json
        if (isTwoVideoSystem && vdd2 != null)
            videoPlayer2.url = vdd2.filename;
    }

    private void Update()
    {
        if(doneText.activeSelf && AuxiliaryFunctions.isGripButtonPressed())
        {
            // serialize rating
            if(!isTwoVideoSystem)
                updateJson(vdd1.videoname, starsSelected);
            else
            {
                updateJson(vdd1.videoname, heartSelected);
                updateJson(vdd2.videoname, -heartSelected);
            }

            // switch to next one
            Pause();
            if(selectNextVDDs())
            {
                doneText.SetActive(false);
                started = false;
                videoImage.SetActive(false);

                videoPlayer1.url = vdd1.filename;
                if (!isTwoVideoSystem)
                    showStars(0);
                else
                {
                    rateReset();
                    video2Image.SetActive(false);
                    videoPlayer2.url = vdd2.filename;
                }
            }
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
        starsSelected = count;
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
        heartSelected = 1;
        hearts[0].SetActive(true);
        hearts[1].SetActive(false);
    }
    public void rate2()
    {
        heartSelected = -1;
        hearts[1].SetActive(true);
        hearts[0].SetActive(false);

        if(started)
            doneText.SetActive(true);
    }
    void rateReset()
    {
        heartSelected = 0;
        hearts[0].SetActive(false);
        hearts[1].SetActive(false);
    }

    void deserialize()
    {
        string vddmPath = Application.persistentDataPath + "/VideoDescriptionDataManager.json";

        if (!File.Exists(vddmPath))
        {
            Debug.Log("No VideoDescriptionDataManager found.");
            SceneManager.LoadScene("RatingEvaluation_KITCHEN_CLASH_VR");
        }
        else
        {
            VideoDescriptionDataManager vddM = JsonUtility.FromJson<VideoDescriptionDataManager>(File.ReadAllText(vddmPath));
            currentVDDs = new List<VideoDescriptionData>();

            // filter out all with same userID to prevent rating own sessions
            foreach(string vddpath in vddM.videoDescriptionDataFiles)
            {
                VideoDescriptionData vdd = JsonUtility.FromJson<VideoDescriptionData>(File.ReadAllText(Application.persistentDataPath + "/" + vddpath));
                if(vdd.userID != PlayerPrefs.GetString("userID"))
                    currentVDDs.Add(vdd);
            }
            selectNextVDDs();
        }
    }

    bool selectNextVDDs()
    {
        if(ratingsDone >= MAX_RATINGS)
        {
            Debug.Log("Max. ratings (" + MAX_RATINGS + ") reached.");
            SceneManager.LoadScene("RatingEvaluation_KITCHEN_CLASH_VR");
            return false;
        }
        if (currentVDDs.Count == 0 || (isTwoVideoSystem && currentVDDs.Count <= 1))
        {
            Debug.Log("No more (suitable) Videos found.");
            SceneManager.LoadScene("RatingEvaluation_KITCHEN_CLASH_VR");
            return false;
        }
        if(!isTwoVideoSystem)
        {
            vdd1 = currentVDDs[Random.Range(0, currentVDDs.Count)];
            currentVDDs.Remove(vdd1);
        }
        else
        {
            // test if we can find 2 videos of same task
            List<VideoDescriptionData> dualVDDlist = new List<VideoDescriptionData>();
            foreach (VideoDescriptionData dual1 in currentVDDs)
            {
                foreach (VideoDescriptionData dual2 in currentVDDs)
                {
                    if(dual1 != dual2 && dual1.sceneName == dual2.sceneName) // at least 2 vdds with same scene
                    {
                        if (!dualVDDlist.Contains(dual1))
                            dualVDDlist.Add(dual1);
                        if (!dualVDDlist.Contains(dual2))
                            dualVDDlist.Add(dual2);
                    }
                }
            }
            if(dualVDDlist.Count == 0)
            {
                Debug.Log("No more (suitable) Videos found.");
                SceneManager.LoadScene("RatingEvaluation_KITCHEN_CLASH_VR");
                return false;
            }

            //1st vdd random
            vdd1 = dualVDDlist[Random.Range(0, dualVDDlist.Count)];
            dualVDDlist.Remove(vdd1);
            //2nd vdd constrained
            List<VideoDescriptionData> potentialSecondVDDList = new List<VideoDescriptionData>();
            foreach (VideoDescriptionData potentialSecondVDD in dualVDDlist)
                if (potentialSecondVDD.sceneName == vdd1.sceneName)
                    potentialSecondVDDList.Add(potentialSecondVDD);
            vdd2 = potentialSecondVDDList[Random.Range(0, potentialSecondVDDList.Count)];

            currentVDDs.Remove(vdd1);
            currentVDDs.Remove(vdd2);
        }
        ratingsDone++;
        return true;
    }
    
    void updateJson(string videoName, int value)
    {
        string vddPath = Application.persistentDataPath + "/" + videoName+ ".json";
        if (!File.Exists(vddPath))
        {
            Debug.Log("File not found: " + vddPath);
            return;
        }
        VideoDescriptionData vdd = JsonUtility.FromJson<VideoDescriptionData>(File.ReadAllText(vddPath));
        if(!isTwoVideoSystem)
        {
            // ABSOLUTE
            List<int> absoluteRatings = vdd.absoluteRatings.OfType<int>().ToList();
            absoluteRatings.Add(value);
            vdd.absoluteRatings = absoluteRatings.ToArray<int>();
        }
        else
        {
            // RELATIVE
            List<int> relativeRatings = vdd.relativeRatings.OfType<int>().ToList();
            relativeRatings.Add(value);
            vdd.relativeRatings = relativeRatings.ToArray<int>();
        }

        File.WriteAllText(vddPath, JsonUtility.ToJson(vdd, true));
    }
}
