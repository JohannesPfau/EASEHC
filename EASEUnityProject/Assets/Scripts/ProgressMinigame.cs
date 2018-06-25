using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressMinigame  {

    GameObject progressGO;
    public int progressCount;
    int maxProgress;
    DateTime startingTime;

    public ProgressMinigame(int maxProgress, GameObject progressGO)
    {
        progressCount = 0;
        this.maxProgress = maxProgress;
        this.progressGO = progressGO;
        startingTime = DateTime.Now;
        progressGO.transform.localScale = new Vector3(1, 1, 1);
        progressGO.SetActive(true);
    }

    public void addProgress()
    {
        progressCount++;
        updateAndDisplay();
    }

    public void updateAndDisplay()
    {
        float percentage = 100 * progressCount / maxProgress;
        GameObject.Find("ProgressPercent").GetComponent<Text>().text = percentage + "%";
        TimeSpan ts = DateTime.Now.Subtract(startingTime);
        string seconds = ts.Seconds.ToString();
        if (seconds.Length == 1)
            seconds = "0" + seconds;
        GameObject.Find("ProgressTime").GetComponent<Text>().text = ts.Minutes + ":" + seconds;
        GameObject.Find("ProgressImage").GetComponent<Image>().fillAmount = ((float)progressCount) / ((float)maxProgress);

        // emoji
        bool changed = true;

        Mood progressMood = Mood.SLEEPY;

        if (percentage <= 20)
        {
            if (GameObject.Find("PROGRESS_Emoji").GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).IsName("EmojiSleepy"))
                changed = false;
        }

        if(percentage > 20)
        {
            progressMood = Mood.NEUTRAL;
            if (GameObject.Find("PROGRESS_Emoji").GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).IsName("EmojiWaiting"))
                changed = false;
        }
        if (percentage > 40)
        {
            progressMood = Mood.THINKING;
            if (GameObject.Find("PROGRESS_Emoji").GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).IsName("EmojiThinking"))
                changed = false;
        }
        if (percentage > 60)
        {
            progressMood = Mood.HAPPY;
            if (GameObject.Find("PROGRESS_Emoji").GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).IsName("EmojiHappy"))
                changed = false;
        }
        if (percentage > 80)
        {
            progressMood = Mood.COOL;
            if (GameObject.Find("PROGRESS_Emoji").GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).IsName("EmojiCool"))
                changed = false;
        }
        if (percentage > 90)
        {
            progressMood = Mood.STARS;
            if (GameObject.Find("PROGRESS_Emoji").GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).IsName("EmojiStars"))
                changed = false;
        }
        if(changed)
            GameObject.Find("PROGRESS_Emoji").GetComponentInChildren<Animator>().SetTrigger(progressMood.ToString());
    }

    public bool isFinished()
    {
        if (progressCount >= maxProgress)
        {
            progressGO.transform.localScale = new Vector3(0, 0, 0);
            progressGO.SetActive(false);
            return true;
        }
        return false;
    }

}
