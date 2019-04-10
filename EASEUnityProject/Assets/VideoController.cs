using RockVR.Video;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideoController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        VideoPlayer.instance.SetRootFolder();
        VideoPlayer.instance.PlayVideo();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
