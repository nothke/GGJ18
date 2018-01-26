using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TVParams : MonoBehaviour
{

    [Range(0, 1)]
    public float scrollSpeed;
    [Range(0, 1)]
    public float snowBandsThreshold;
    [Range(0, 1)]
    public float crtDistance;
    [Range(0, 1)]
    public float spotsSize;

    CRTEffect _crtEffect;
    CRTEffect crtEffect { get { if (!_crtEffect) _crtEffect = GetComponent<CRTEffect>(); return _crtEffect; } }

    SpotsEffect _spotsEffect;
    SpotsEffect spotsEffect { get { if (!_spotsEffect) _spotsEffect = GetComponent<SpotsEffect>(); return _spotsEffect; } }

    void Update()
    {
        spotsEffect.scrollSpeed = scrollSpeed;
        crtEffect.distance = Mathf.Lerp(0.01f, 0.6f, crtDistance);
        spotsEffect.linesThreshold = 1 - snowBandsThreshold;
        spotsEffect.spotsSize = spotsSize;
    }
}
