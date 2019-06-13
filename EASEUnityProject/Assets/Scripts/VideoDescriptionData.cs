using System;
using UnityEngine;

[Serializable]
public class VideoDescriptionData
{
    public string userID;
    public string videoname;
    public string filename;
    public int timeScore; // 1...4 stars/trophy for best time
    public int actionScore; // 1...3 stars
    public int[] absoluteRatings; // 1 ... 5
    public int[] relativeRatings; // -x ... 0 ... +x
    public string sceneName;
    public int nrOfActionsSpent;
    public float secondsSpent;
    public string[] eventsTracked;

    public VideoDescriptionData(string videoname, string filename, string sceneName, int nrOfActionsSpent, float secondsSpent, int timeScore, int actionScore)
    {
        this.videoname = videoname;
        userID = PlayerPrefs.GetString("userID");
        this.filename = filename;
        this.sceneName = sceneName;
        this.nrOfActionsSpent = nrOfActionsSpent;
        this.secondsSpent = secondsSpent;
        this.timeScore = timeScore;
        this.actionScore = actionScore;
        absoluteRatings = new int[0];
        relativeRatings = new int[0];
        eventsTracked = new string[PlayerPrefs.GetInt("EventSize")];
        for (int i=0; i < eventsTracked.Length; i++)
        {
            eventsTracked[i] = PlayerPrefs.GetString("Event"+i);
            PlayerPrefs.DeleteKey("Event" + i);
        }
    }

    public float getAbsoluteRating()
    {
        if (absoluteRatings == null || absoluteRatings.Length == 0)
            return -1;

        float r = 0f;
        foreach (int i in absoluteRatings)
            r += i;
        return r / absoluteRatings.Length;
    }

    public float getRelativeRating()
    {
        if (relativeRatings == null || relativeRatings.Length == 0)
            return 0;

        float r = 0f;
        foreach (int i in relativeRatings)
            r += i;
        return r;
    }
}
