using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Video;

public class RTHandler : MonoBehaviour
{
    public Camera rt1Camera;
    public VideoPlayer rt1VideoPlayer;
    public SpotsEffect spots;

    public RenderTexture rt1;
    public RenderTexture rt2;

    /*
    private void Awake()
    {
        rt1 = new RenderTexture(Screen.width, Screen.height, 24);
    }*/
    
    void Start()
    {
        rt1 = new RenderTexture(Screen.width, Screen.height, 24);
        //if (rt1VideoPlayer.targetTexture != null) rt1VideoPlayer.targetTexture.Release();
        rt1VideoPlayer.targetTexture = rt1;
        //rt1VideoPlayer.Play();
        spots.tex1 = rt1;
        //rt1Camera.targetTexture = rt1;
    }
}