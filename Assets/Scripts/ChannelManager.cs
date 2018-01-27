using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChannelManager : MonoBehaviour
{
    public static ChannelManager e;

    public static Channel[] allChannels;

    public SpotsEffect spots;

    [Header("Debugging only:")]
    public Channel channel1;
    public Channel channel2;

    public RenderTexture[] rts;

    int currentRt;

    void Awake()
    {
        e = this;

        // Find all channels in scene
        allChannels = FindObjectsOfType<Channel>();

        // Init render textures
        rts = new RenderTexture[2];

        for (int i = 0; i < rts.Length; i++)
        {
            rts[i] = new RenderTexture(Screen.width, Screen.height, 24);
        }
    }

    public void SwitchChannels(Channel primaryChannel, Channel secondaryChannel)
    {
        channel1 = primaryChannel;
        channel2 = secondaryChannel;

        channel1.SetOutput(rts[0]);
        channel2.SetOutput(rts[1]);

        // We don't need to change this every time right?
        //if (spots.tex1 == null || spots.tex2 == null)
        //{
        spots.tex1 = rts[0];
        spots.tex2 = rts[1];
        //}
    }

    public void SetChannelBlendValue(float blend)
    {
        spots.textureBlend = blend;

        channel1.audioSource.volume = 1 - blend;
        channel2.audioSource.volume = blend;
    }
}