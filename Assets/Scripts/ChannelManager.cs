using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChannelManager : MonoBehaviour
{
    public static ChannelManager e;

    public static Channel[] allChannels;

    SpotsEffect spots;

    public bool clampBlending = true;

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

        for (int i = 0; i < allChannels.Length; i++)
        {
            allChannels[i].videoPlayer.Pause();
        }

        spots = FindObjectOfType<SpotsEffect>();

        // Init render textures
        rts = new RenderTexture[2];

        for (int i = 0; i < rts.Length; i++)
        {
            rts[i] = new RenderTexture(Screen.width, Screen.height, 24);
        }
    }

    public void SwitchChannels(Channel primaryChannel, Channel secondaryChannel)
    {
        // Pause previous channels
        if (channel1)
            channel1.videoPlayer.Pause(); // Move to method in channel
        if (channel2)
            channel2.videoPlayer.Pause();

        // assign new
        channel1 = primaryChannel;
        channel2 = secondaryChannel;
        
        channel1.SetOutput(rts[0]);
        channel2.SetOutput(rts[1]);

        // We don't need to change this every time right? (not sure)
        //if (spots.tex1 == null || spots.tex2 == null)
        //{
            spots.tex1 = rts[0];
            spots.tex2 = rts[1];
        //}
    }

    public void SetChannelBlendValue(float blend)
    {
        if (clampBlending) blend = Mathf.Clamp01(blend);

        spots.textureBlend = blend;

        if (channel1 != null)
        {
            channel1.audioSource.volume = 1 - blend;
            channel2.audioSource.volume = blend;
        }
    }
}