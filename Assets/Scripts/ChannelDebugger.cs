using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChannelDebugger : MonoBehaviour
{

    float input;

    public Channel testChannel;
    public Channel testChannel2;

    TVParams tvParams;

    float vert;
    float scroll;

    private void Start()
    {
        ChannelManager.e.SwitchChannels(testChannel, testChannel2);
        tvParams = FindObjectOfType<TVParams>();
    }

    private void Update()
    {
        input += Mathf.Clamp(Input.GetAxis("Horizontal") * 0.02f, -1, 2);

        ChannelManager.e.SetChannelBlendValue(input);

        if (Input.GetKeyDown(KeyCode.Space))
            ChannelManager.e.SwitchChannels(
                ChannelManager.allChannels[Random.Range(0, ChannelManager.allChannels.Length)],
                ChannelManager.allChannels[Random.Range(0, ChannelManager.allChannels.Length)]);

        scroll += Input.GetAxis("Mouse ScrollWheel") * 0.1f;
        vert += Input.GetAxis("Vertical") * 0.03f;

        scroll = Mathf.Clamp01(scroll);
        vert = Mathf.Clamp01(vert);

        tvParams.SetParameter(2, scroll);
        tvParams.SetParameter(0, vert);
    }
}
