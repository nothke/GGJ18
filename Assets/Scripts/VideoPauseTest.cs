using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Video;

public class VideoPauseTest : MonoBehaviour
{

    public VideoPlayer videoPlayer;

    private void OnEnable()
    {
        videoPlayer.prepareCompleted += Prepared;
    }

    private void OnDisable()
    {
        videoPlayer.prepareCompleted -= Prepared;
    }

    void Prepared(VideoPlayer source)
    {
        source.time = Random.Range(0, (float)videoPlayer.clip.length);
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            videoPlayer.enabled = !videoPlayer.enabled;

            if (videoPlayer.enabled)
            {
                //videoPlayer.time = Random.Range(0, (float) videoPlayer.clip.length);
                //Debug.Log(videoPlayer.canSetTime);
            }
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
