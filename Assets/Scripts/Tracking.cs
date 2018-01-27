using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;
using Valve.VR;

public class Tracking : MonoBehaviour
{
    struct Channel
    {
        public Channel(int n, Pose[] p, Color dc)
        {
            number = n;
            poses = p;
            debugColor = dc;
        }

        public int number;
        public Pose[] poses;
        public Color debugColor;
    }

    struct Pose
    {
        public Pose(Vector3 p, Quaternion r,float pCD)
        {
            pos = p;
            rot = r;
            pairedContDist = pCD; 
        }

        public Vector3 pos;
        public Quaternion rot;
        public float pairedContDist;
    }

    public TVParams tvParams;

    SteamVR_ControllerManager controllerManager;
    List<Transform> devices = new List<Transform>();

    int lastControllerAmount;

    List<Channel> channels = new List<Channel>();

    Vector3 trackingOriginOffset;

    bool started = false;

    int channelController = 0;
    int currentChannel;
    int compitingChannel = 0;
    float channelStrenght;
    int lastChannel;

    public AnimationCurve noiseCurve;
    public AnimationCurve channelBlendCurve;

    public Transform channelsParent;

    void Start ()
    {
        controllerManager = GetComponentInChildren<SteamVR_ControllerManager>();
        UpdateDeviceList();
        lastControllerAmount = controllerManager.transform.childCount;

        for (int i = 1; i < 5; ++i)
        {
            float value = noiseCurve.Evaluate(0.0f);
            tvParams.SetParameter(i - 1, value);
        }

        // TODO: Show tutorial channel
    }

    void Update()
    {
        // Device hot plugging support
        if (GetActiveChildCount(controllerManager.transform) != lastControllerAmount)
        {
            UpdateDeviceList();
        }
        lastControllerAmount = controllerManager.transform.childCount;

        // If no devices are connected or dashboard is open, wait for new devices
        if (devices.Count <= 0)
        {
            Debug.LogWarning("No devices!");
            return;
        }

        // Wait until user presses trigger to start
        if (!started)
        {
            for (int i = 0; i < devices.Count; ++i)
            {
                SteamVR_TrackedObject device = devices[i].GetComponent<SteamVR_TrackedObject>();

                if (device != null && device.index != SteamVR_TrackedObject.EIndex.Hmd)
                {
                    Debug.Log(SteamVR_Controller.Input((int)device.index).GetAxis(EVRButtonId.k_EButton_SteamVR_Trigger).x);
                    if (SteamVR_Controller.Input((int)device.index).GetAxis(EVRButtonId.k_EButton_SteamVR_Trigger).x > 0.8f)
                    {
                        GetChannels(devices[i].position);
                        started = true;
                    }
                }
            }

            // Only run main code once we have started
            return;
        }

        // Find closest channels
        currentChannel = -1;
        float closestDistance = 99999f;
        for (int i = 0; i < channels.Count; ++i)
        {
            float dist = Vector3.Distance(channels[i].poses[0].pos, devices[channelController].position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                channelStrenght = dist;
                currentChannel = i;
            }
        }

        // Switch and blend closest channels
        if (currentChannel != -1)
        {   
            if(lastChannel != currentChannel)
            {
                // Switch channel 
                compitingChannel = lastChannel;
            }
            lastChannel = currentChannel;

            float blend = channelBlendCurve.Evaluate(channelStrenght);

            Debug.DrawLine(channels[currentChannel].poses[0].pos, devices[channelController].position, Color.green);
            Debug.DrawLine(channels[compitingChannel].poses[0].pos, devices[channelController].position, Color.yellow);
        }

        // Update channel noise

        //pairs controllers with poses         
        if (currentChannel >= 0)
        {
            List<int> reservedPoses = new List<int>();
            float[] deviceNoises = new float[devices.Count];
            bool changed = false;

            for (int j = 1; j < devices.Count; ++j)
            {
                for (int i = 1; i < devices.Count; i++)
                {
                    channels[currentChannel].poses[i].pairedContDist = 99999f;
                }
                float closestPoseDist = 99999f;
                int closestPoseDevice = -1;
                int closestPose = -1;
                for (int i = 1; i < devices.Count; ++i)
                {
                    if (!reservedPoses.Contains(i))
                    {
                        float dist = Vector3.Distance(channels[currentChannel].poses[i].pos, devices[j].position);
                        if (dist < closestPoseDist && dist < channels[currentChannel].poses[i].pairedContDist)
                        {
                            if (channels[currentChannel].poses[i].pairedContDist != 99999f)
                            {
                                changed = true;
                            }
                            deviceNoises[i] = dist;
                            closestPoseDist = dist;
                            closestPoseDevice = j;
                            closestPose = i;
                            channels[currentChannel].poses[i].pairedContDist = dist;
                        }
                    }
                }

                reservedPoses.Add(closestPose);
                Debug.DrawLine(channels[currentChannel].poses[closestPose].pos, devices[closestPoseDevice].position, Color.cyan);
            }
            while (changed)
            {
                changed = false;
                for (int j = 1; j < devices.Count; ++j)
                {
                    float closestPoseDist = 99999f;
                    int closestPoseDevice = -1;
                    int closestPose = -1;
                    for (int i = 1; i < devices.Count; ++i)
                    {
                        if (!reservedPoses.Contains(i))
                        {
                            float dist = Vector3.Distance(channels[currentChannel].poses[i].pos, devices[j].position);
                            if (dist < closestPoseDist && dist < channels[currentChannel].poses[i].pairedContDist)
                            {
                                if (channels[currentChannel].poses[i].pairedContDist != 99999f)
                                {
                                    changed = true;
                                }
                                deviceNoises[i] = dist;
                                closestPoseDist = dist;
                                closestPoseDevice = j;
                                closestPose = i;
                                channels[currentChannel].poses[i].pairedContDist = dist;
                            }
                        }
                    }
                }
            }
            // sets shader parameters
            for (int i = 1; i < deviceNoises.Length; ++i)
            {
                float value = noiseCurve.Evaluate(deviceNoises[i]);
                tvParams.SetParameter(i - 1, value);
                Debug.Log(i - 1 + ", " + value);
            }

            // Draw debug for channel points
            for (int i = 0; i < channels.Count; ++i)
            {
                if (i == this.currentChannel)
                {
                    for (int j = 0; j < devices.Count; ++j)
                    {
                        Debug.DrawLine(channels[i].poses[j].pos, RotatePointAroundPivot(channels[i].poses[j].pos + Vector3.up * 0.1f, channels[i].poses[j].pos, channels[i].poses[j].rot.eulerAngles), channels[i].debugColor);
                    }
                }
            }
        }
    }

    void GetChannels(Vector3 newOrigin)
    {
        trackingOriginOffset = newOrigin;

        for (int i = 0; i < channelsParent.childCount; ++i)
        {
            Pose[] newPoses = new Pose[channelsParent.GetChild(i).childCount];
            for (int j = 0; j < newPoses.Length; ++j)
            {
                Transform channel = channelsParent.GetChild(i).GetChild(j).transform;
                newPoses[j] = new Pose(channel.position + trackingOriginOffset, channel.rotation, 99999f);
            }

            channels.Add(new Channel(i, newPoses, Random.ColorHSV(0.0f, 1.0f, 0.9f, 1.0f, 0.45f, 0.55f)));
        }
    }

    void CreateRandomChannels(Vector3 newOrigin)
    {
        trackingOriginOffset = newOrigin;

        for (int i = 0; i < 3; ++i)
        {
            Pose[] newPoses = new Pose[GetActiveChildCount(controllerManager.transform)];
            for (int j = 0; j < newPoses.Length; ++j)
            {
                newPoses[j] = new Pose(Random.insideUnitSphere * 0.4f + Vector3.up * 0.44f + trackingOriginOffset, Random.rotation, 99999f);
            }

            channels.Add(new Channel(i, newPoses, Random.ColorHSV(0.0f, 1.0f, 0.9f, 1.0f, 0.45f, 0.55f)));
        }
    }

    void UpdateDeviceList()
    {
        devices.Clear();
        for (int i = 0; i < controllerManager.transform.childCount; ++i)
        {
            if (controllerManager.transform.GetChild(i).gameObject.activeSelf && controllerManager.transform.GetChild(i).tag != "MainCamera")
            {
                devices.Add(controllerManager.transform.GetChild(i));
            }
        }
    }

    int GetActiveChildCount(Transform parent)
    {
        int val = 0;
        for (int i = 0; i < controllerManager.transform.childCount; ++i)
        {
            if (controllerManager.transform.GetChild(i).gameObject.activeSelf && controllerManager.transform.GetChild(i).tag != "MainCamera")
            {
                val++;
            }
        }

        return val;
    }

    public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
    {
        return Quaternion.Euler(angles) * (point - pivot) + pivot;
    }
}
