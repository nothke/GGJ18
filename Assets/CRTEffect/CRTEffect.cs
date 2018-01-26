using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class CRTEffect : EffectBase
{
    public YIQTransformCurves YIQPreset;

    [Range(0, 0.2f)]
    public float distance = 0.01f;
    [Range(0, 2)]
    public float sourceIntensity = 1;
    [Range(0, 2)]
    public float intensity = 1;

    void SetCurves()
    {
        /*
        var materialProperty = new MaterialPropertyBlock();

        materialProperty.SetFloatArray("arrayName", floatArray);
        gameObject.GetComponent<Renderer>().SetPropertyBlock(materialProperty);

        material.set*/

        if (!YIQPreset) return;

        float[] yValues = new float[10];
        float[] iqValues = new float[10];

        for (int i = 0; i < 10; i++)
        {
            yValues[i] = YIQPreset.YCurve.Evaluate((float)i / 10);
            iqValues[i] = YIQPreset.IQCurve.Evaluate((float)i / 10);
        }

        //material.SetFloatArray("yCurve", yValues);
        //material.SetFloatArray("iqCurve", iqValues);

        Shader.SetGlobalFloatArray("_YCurve", yValues);
        Shader.SetGlobalFloatArray("_IQCurve", iqValues);
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        SetCurves();

        material.SetFloat("_Distance", distance);
        material.SetFloat("_Intensity", intensity);
        material.SetFloat("_SourceIntensity", sourceIntensity);

        Graphics.Blit(source, destination, material);
    }
}
