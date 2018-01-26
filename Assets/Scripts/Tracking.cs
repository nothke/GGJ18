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
        public Pose(Vector3 p, Quaternion r)
        {
            pos = p;
            rot = r;
        }

        public Vector3 pos;
        public Quaternion rot;
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

    public AnimationCurve noiseCurve;

    void Start ()
    {
        controllerManager = GetComponentInChildren<SteamVR_ControllerManager>();
        UpdateDeviceList();
        lastControllerAmount = controllerManager.transform.childCount;
    }

	void Update ()
    {
        if(!started)
        {
            for(int i = 0; i < devices.Count; ++i)
            {
                SteamVR_TrackedObject device = devices[i].GetComponent<SteamVR_TrackedObject>();

                //Debug.Log(devices[i].name);
                if (device != null && device.index != SteamVR_TrackedObject.EIndex.Hmd)
                {
                    //Debug.Log(SteamVR_Controller.Input((int)device.index).GetAxis(EVRButtonId.k_EButton_SteamVR_Trigger).x);
                    if (SteamVR_Controller.Input((int)device.index).GetAxis(EVRButtonId.k_EButton_SteamVR_Trigger).x > 0.8f)
                    {
                        CreateChannels(devices[i].position);
                        started = true;
                    }
                }
            }
        }

       if(GetActiveChildCount(controllerManager.transform) != lastControllerAmount)
        {
            UpdateDeviceList();
        }
        lastControllerAmount = controllerManager.transform.childCount;

        // Find closest channel
        int closestChannel = -1;
        float closestDistance = 99999f;
        for(int i = 0; i < channels.Count; ++i)
        {
            float dist = Vector3.Distance(channels[i].poses[0].pos, devices[channelController].position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                closestChannel = i;
                currentChannel = closestChannel;
            }
        }
        if (closestChannel != -1)
        {
            Debug.DrawLine(channels[closestChannel].poses[0].pos, devices[channelController].position, Color.green);
        }

        // Get noise for channel
        if (closestChannel >= 0)
        {
            float[] channelNoises = new float[channels[closestChannel].poses.Length];
            for (int i = 1; i < channels[closestChannel].poses.Length; ++i)
            {
                float closestPoseDist = 99999f;
                int closestPoseDevice = -1;
                for (int j = 1; j < devices.Count; ++j)
                {
                    float dist = Vector3.Distance(channels[closestChannel].poses[i].pos, devices[j].position);
                    if (dist < closestPoseDist)
                    {
                        channelNoises[i] = dist;
                        closestPoseDist = dist;
                        closestPoseDevice = j;
                    }
                }
                Debug.DrawLine(channels[closestChannel].poses[i].pos, devices[closestPoseDevice].position, Color.cyan);
            }

            for (int i = 1; i < channelNoises.Length; ++i)
            {
                float value = noiseCurve.Evaluate(channelNoises[i]);
                tvParams.SetParameter(i-1, value);
            }
        }

        // Draw debug for channel points
        for (int i = 0; i < channels.Count; ++i)
        {
            if (i == currentChannel)
            {
                for (int j = 0; j < channels[i].poses.Length; ++j)
                {
                    Debug.DrawLine(channels[i].poses[j].pos, RotatePointAroundPivot(channels[i].poses[j].pos + Vector3.up * 0.1f, channels[i].poses[j].pos, channels[i].poses[j].rot.eulerAngles), channels[i].debugColor);
                }
            }
        }
    }

    void CreateChannels(Vector3 newOrigin)
    {
        trackingOriginOffset = newOrigin;

        for (int i = 0; i < 3; ++i)
        {
            Pose[] newPoses = new Pose[GetActiveChildCount(controllerManager.transform)];
            for (int j = 0; j < newPoses.Length; ++j)
            {
                newPoses[j] = new Pose(Random.insideUnitSphere * 0.4f + Vector3.up * 0.44f + trackingOriginOffset, Random.rotation);
            }

            channels.Add(new Channel(i, newPoses, Random.ColorHSV(0.0f, 1.0f, 0.6f, 1.0f, 0.3f, 0.6f)));
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
