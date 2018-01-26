using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TVParams : MonoBehaviour
{

    [System.Obsolete("Use SetParameter(i, parameter) instead, fuck you Samuli (Horatiu forced me to write this)")]
    public float scrollSpeed;
    [System.Obsolete("Use SetParameter(i, parameter) instead")]
    public float snowBandsThreshold;
    [System.Obsolete("Use SetParameter(i, parameter) instead")]
    public float crtDistance;
    [System.Obsolete("Use SetParameter(i, parameter) instead")]
    public float spotsSize;

    CRTEffect _crtEffect;
    CRTEffect crtEffect { get { if (!_crtEffect) _crtEffect = GetComponent<CRTEffect>(); return _crtEffect; } }

    SpotsEffect _spotsEffect;
    SpotsEffect spotsEffect { get { if (!_spotsEffect) _spotsEffect = GetComponent<SpotsEffect>(); return _spotsEffect; } }

    [SerializeField]
    float[] testParameters;

    private void Start()
    {
        testParameters = new float[4];
    }

    public void SetParameter(int i, float parameter)
    {
        switch (i)
        {
            case 0: spotsEffect.scrollSpeed = parameter; break;
            case 1: crtEffect.distance = Mathf.Lerp(0.01f, 0.6f, parameter); break;
            case 2: spotsEffect.linesThreshold = 1 - parameter; break;
            case 3: spotsEffect.spotsSize = parameter; break;
            default: Debug.Log("YOu have more controllers than parameters"); break;
        }
    }

    public bool test;

    void Update()
    {
        if (test)
        {
            for (int i = 0; i < testParameters.Length; i++)
            {
                SetParameter(i, testParameters[i]);
            }
        }
    }
}
