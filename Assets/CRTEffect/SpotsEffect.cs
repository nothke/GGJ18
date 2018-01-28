using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotsEffect : EffectBase
{
    public float squash = 1;
    public float uiSquash = 1;

    public Texture2D texture;
    public float noiseThreshold;
    [Range(0, 1)]
    public float intensity = 0.5f;
    [Range(0, 1)]
    public float spotsSize = 0.01f;

    public float scrollSpeed = 10;

    public float linesThreshold = 0.5f;

    float scrollPosition;

    public RenderTexture tex1;
    public RenderTexture tex2;

    [Range(0, 1)]
    public float textureBlend;

    public Camera uiCamera;
    public RenderTexture uiRT;

    public float contrast = 1;

    private void Awake()
    {
        CreateUIRenderTexture();
    }

    void CreateUIRenderTexture()
    {
        uiRT = new RenderTexture(Screen.width, Screen.height, 24);
        uiCamera.targetTexture = uiRT; // Is this VR camera with vive??
        material.SetTexture("_UITex", uiRT);
    }

    /*
    private void Update()
    {
        textureBlend += Input.GetAxis("Horizontal") * 0.1f;
    }*/

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        material.SetTexture("_Tex1", tex1);
        material.SetTexture("_Tex2", tex2);

        material.SetFloat("_Squash", squash);
        material.SetFloat("_UISquash", uiSquash);

        material.SetFloat("_ChannelBlend", textureBlend);
        material.SetFloat("_NoiseThreshold", noiseThreshold);
        material.SetFloat("_Intensity", intensity);
        material.SetFloat("_SpotsSize", spotsSize);
        scrollPosition += Time.deltaTime * scrollSpeed;
        material.SetFloat("_ScrollPosition", scrollPosition);
        material.SetFloat("_LinesThreshold", linesThreshold);
        material.SetFloat("_Contrast", contrast);

        Graphics.Blit(source, destination, material);
    }
}
