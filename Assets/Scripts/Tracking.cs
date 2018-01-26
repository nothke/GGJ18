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

    SteamVR_ControllerManager controllerManager;
    List<Transform> devices = new List<Transform>();

    int lastControllerAmount;

    List<Channel> channels = new List<Channel>();

    Vector3 trackingOriginOffset;

    bool started = false;

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

                Debug.Log(devices[i].name);
                if (device != null && device.index != SteamVR_TrackedObject.EIndex.Hmd)
                {
                    Debug.Log(SteamVR_Controller.Input((int)device.index).GetAxis(EVRButtonId.k_EButton_SteamVR_Trigger).x);
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

        for(int i = 0; i < channels.Count; ++i)
        {
            for (int j = 0; j < channels[i].poses.Length; ++j)
            {
                Debug.DrawLine(channels[i].poses[j].pos, RotatePointAroundPivot(channels[i].poses[j].pos + Vector3.up * 0.1f, channels[i].poses[j].pos, channels[i].poses[j].rot.eulerAngles), channels[i].debugColor);
            }
        }
    }

    void CreateChannels(Vector3 newOrigin)
    {
        trackingOriginOffset = newOrigin;

        for (int i = 0; i < 10; ++i)
        {
            Pose[] newPoses = new Pose[GetActiveChildCount(controllerManager.transform)];
            for (int j = 0; j < newPoses.Length; ++j)
            {
                newPoses[i] = new Pose(Random.insideUnitSphere + Vector3.up * 0.5f + trackingOriginOffset, Random.rotation);
            }

            channels.Add(new Channel(i, newPoses, Random.ColorHSV(0.0f, 1.0f, 0.6f, 1.0f, 0.3f, 0.6f)));
        }
    }

    void UpdateDeviceList()
    {
        devices.Clear();
        for (int i = 0; i < controllerManager.transform.childCount; ++i)
        {
            if (controllerManager.transform.GetChild(i).gameObject.activeSelf)
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
            if (controllerManager.transform.GetChild(i).gameObject.activeSelf)
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
