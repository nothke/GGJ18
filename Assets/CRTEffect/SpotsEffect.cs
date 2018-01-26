using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotsEffect : EffectBase
{

    public Texture2D texture;
    public float noiseThreshold;
    [Range(0, 1)]
    public float intensity = 0.5f;

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        material.SetTexture("_MainTexture", texture);
        material.SetFloat("_NoiseThreshold", noiseThreshold);
        material.SetFloat("_Intensity", intensity);

        Graphics.Blit(source, destination, material);
    }
}
