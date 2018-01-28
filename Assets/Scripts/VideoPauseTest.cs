using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Video;

public class VideoPauseTest : MonoBehaviour
{

    public VideoPlayer videoPlayer;

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            videoPlayer.enabled = !videoPlayer.enabled;
        }

        /*
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (videoPlayer.isPlaying)
                videoPlayer.Pause();
            else
                videoPlayer.Play();
        }*/
    }
}
