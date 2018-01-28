using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioMixer mixer;
    [Range(0.0f, 1.0f)] public float distortion;
    public AnimationCurve distortionCurve;

    void Awake ()
    {
        if(instance != null)
        {
            Destroy(instance.gameObject);
        }

        instance = this;
    }
	
	void Update ()
    {
        mixer.SetFloat("Distortion", distortionCurve.Evaluate(distortion));
        mixer.SetFloat("CuttoffFreq", Mathf.Lerp(22000.00f, 22.0f, distortion));
        mixer.SetFloat("Volume", Mathf.Lerp(16.0f, -40.0f, distortion));
    }
}
