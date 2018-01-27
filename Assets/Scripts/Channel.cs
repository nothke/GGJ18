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

    public void SetOutput(RenderTexture renderTexture)
    {
        rt = renderTexture as RenderTexture;

        if (sceneCamera)
            sceneCamera.targetTexture = renderTexture;

        if (videoPlayer)
        {
            // Make sure it's set to texture mode
            videoPlayer.renderMode = VideoRenderMode.RenderTexture;
            videoPlayer.targetTexture = renderTexture;

            audioSource.Play();

            audioSource.enabled = false;
            StartCoroutine(Skip());

            // Old
            //source = audioSource;
            //videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
            //videoPlayer.EnableAudioTrack(0, true);
            //videoPlayer.SetTargetAudioSource(0, audioSource);
            //source.Play();
        }
        Debug.Log("Texture set for " + name);
    }

    IEnumerator Skip()
    {
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        yield return null;

        audioSource.enabled = true;
        audioSource.Play();
    }
}
