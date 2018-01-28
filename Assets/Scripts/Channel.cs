using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Video;

public class Channel : MonoBehaviour
{
    public Camera sceneCamera;
    public VideoPlayer videoPlayer;

    RenderTexture rt;
    [HideInInspector]
    public AudioSource audioSource;

    private void Awake()
    {
        audioSource = videoPlayer.GetTargetAudioSource(0);
        audioSource.volume = 0.0001f;
    }

    public void Disable()
    {
        //videoPlayer.Pause();
        videoPlayer.enabled = false;
    }

    public void SetOutput(RenderTexture renderTexture)
    {
        rt = renderTexture as RenderTexture;

        if (sceneCamera)
            sceneCamera.targetTexture = renderTexture;

        if (videoPlayer)
        {
            videoPlayer.enabled = true;
            // Make sure it's set to texture mode
            videoPlayer.renderMode = VideoRenderMode.RenderTexture;
            videoPlayer.targetTexture = renderTexture;

            float time = Time.time % (float)videoPlayer.clip.length;

            
            videoPlayer.Play();
            videoPlayer.time = time;

            StartCoroutine(Skip());
        }
        Debug.Log("Texture set for " + name);
    }

    IEnumerator Skip()
    {
        yield return null;

        //audioSource.enabled = true;
    }
}
