using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TVAudio : MonoBehaviour {

    System.Random random;

    public float clip = 0.1f;

    [Range(0, 1)]
    public float blend = 1;

    private void Start()
    {
        random = new System.Random();
    }

    private void OnAudioFilterRead(float[] data, int channels)
    {
        for (int i = 0; i < data.Length; i++)
        {
            float noise = (float)random.NextDouble();
            noise = Mathf.Clamp(noise, clip, 1 - clip);
            data[i] = Mathf.Lerp(noise, data[i], blend);
        }
    }
}
