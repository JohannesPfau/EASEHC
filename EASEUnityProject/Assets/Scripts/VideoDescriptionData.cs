using System;
using UnityEngine;

[Serializable]
public class VideoDescriptionData
{
    public string userID;
    public string videoname;
    public string filename;
    public int[] absoluteRatings; // 1 ... 5
    public int[] relativeRatings; // -x ... 0 ... +x

    public VideoDescriptionData(string videoname, string filename)
    {
        this.videoname = videoname;
        userID = PlayerPrefs.GetString("userID");
        this.filename = filename;
        absoluteRatings = new int[0];
        relativeRatings = new int[0];
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
