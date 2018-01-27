using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChannelManager : MonoBehaviour
{
    public static ChannelManager e;

    public SpotsEffect spots;

    public Channel channel1;
    public Channel channel2;

    public AudioSource audioSource1;
    public AudioSource audioSource2;

    [Header("Debugging only:")]
    public Channel[] channels;
    public RenderTexture[] rts;

    int currentRt;



    void Awake()
    {
        e = this;

        // Find all channels in scene
        channels = FindObjectsOfType<Channel>();

        // Init render textures
        rts = new RenderTexture[2];

        for (int i = 0; i < rts.Length; i++)
        {
            rts[i] = new RenderTexture(Screen.width, Screen.height, 24);
            Debug.Log("Made texture");
        }
    }

    public void SwitchChannels(Channel primaryChannel, Channel secondaryChannel)
    {
        channel1 = primaryChannel;
        channel2 = secondaryChannel;

        channel1.SetOutput(rts[0]);
        channel2.SetOutput(rts[1]);

        // We don't need to change this every time right?
        spots.tex1 = rts[0];
        spots.tex2 = rts[1];
    }

    public void SetChannelBlendValue(float blend)
    {
        spots.textureBlend = blend;

        channel1.audioSource.volume = 1 - blend;
        channel2.audioSource.volume = blend;
    }

    // OLD

    /*
    public int MakeChannelVisible(Channel channel)
    {
        currentRt++;
        if (currentRt >= rts.Length) currentRt = 0;
        channel.SetTexture(rts[currentRt]);

        if (currentRt == 0) // NOT SCALABLE
            spots.tex1 = rts[currentRt];
        else
            spots.tex2 = rts[currentRt];

        return currentRt;
    }*/



    /*
    public void SetChannelOpacities(params float[] values)
    {
        // TODO: Add different blend technique when adding more channels
        float blend = spots.textureBlend = values[0];
    }*/
}