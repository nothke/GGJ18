using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChannelDebugger : MonoBehaviour
{

    float input;

    public Channel testChannel;
    public Channel testChannel2;

    private void Start()
    {
        ChannelManager.e.SwitchChannels(testChannel, testChannel2);
    }

    private void Update()
    {
        input += Input.GetAxis("Horizontal") * 0.1f;

        ChannelManager.e.SetChannelBlendValue(input);
    }
}
