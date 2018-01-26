using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotsEffect : EffectBase {

    public Texture2D texture;

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        //SetCurves();

        material.SetTexture("_MainTexture", texture);
        //material.SetFloat("_Intensity", intensity);
        //material.SetFloat("_SourceIntensity", sourceIntensity);

        Graphics.Blit(source, destination, material);
    }
}
