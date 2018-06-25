using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour{

	static AudioClip getAudio(string type)
    {
        Object[] ac = Resources.LoadAll(type);

        return (AudioClip)ac[Random.Range(0, ac.Length)];
    }

    public static void playAudio(string type, GameObject go)
    {
        if (go.GetComponent<AudioSource>() == null)
            go.AddComponent<AudioSource>();

        go.GetComponent<AudioSource>().clip = AudioManager.getAudio(type);
        go.GetComponent<AudioSource>().Play();

    }

}
