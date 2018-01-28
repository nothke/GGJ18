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


    float globalMinDist = 999f;
    List<int> bestPairing = new List<int>();


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
    public AnimationCurve rotationNoiseCurve;
    public AnimationCurve channelBlendCurve;

    public Transform channelsParent;

    public GameObject[] connectionBar;

    public AnimationCurve hapticsCurve;
    float hapticsVal = 0.0f;

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

        ChannelManager.e.SwitchChannels(ChannelManager.allChannels[0], ChannelManager.allChannels[1]);

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
                compitingChannel = lastChannel;
                // Research this
                ChannelManager.e.SwitchChannels(ChannelManager.allChannels[currentChannel], ChannelManager.allChannels[compitingChannel]);
            }
            lastChannel = currentChannel;

            float blend = channelBlendCurve.Evaluate(channelStrenght);
            ChannelManager.e.SetChannelBlendValue(blend);

           Debug.DrawLine(channels[currentChannel].poses[0].pos, devices[channelController].position, Color.green);
            Debug.DrawLine(channels[compitingChannel].poses[0].pos, devices[channelController].position, Color.yellow);
        }

        // Update channel noise

        //pairs controllers with poses         
        if (currentChannel >= 0)
        {


            float[] deviceNoises = new float[devices.Count];
            float[] deviceRotationNoises = new float[devices.Count];

            globalMinDist = 999f;
            bestPairing = new List<int>();

            recCompMin(new List<int>());


            //bestPairing.ForEach(dBug);
            for (int i = 1; i < devices.Count; ++i) {
                //reservedPoses.Add(i);
                //Debug.Log(i.ToString());
                //Debug.Log(bestPairing.Count.ToString());
                //Debug.Log(globalMinDist.ToString());
                Debug.DrawLine(channels[currentChannel].poses[i].pos,devices[bestPairing[i-1]].position, Color.cyan);
                deviceNoises[i] = Vector3.Distance(channels[currentChannel].poses[i].pos, devices[bestPairing[i - 1]].position);
                deviceRotationNoises[i] = Quaternion.Angle(channels[currentChannel].poses[i].rot, devices[bestPairing[i - 1]].rotation) / 180.0f;
                Debug.Log("Rot " + deviceRotationNoises[i]);
            }


            // sets shader parameters and haptics
            for (int i = 1; i < 5; ++i)
            {
                tvParams.SetParameter(i - 1, 0.0f);
            }

            for (int i = 1; i < deviceNoises.Length; ++i)
            {
                float value = noiseCurve.Evaluate(deviceNoises[i]);
                value += rotationNoiseCurve.Evaluate(deviceRotationNoises[i]);

                Debug.Log(value);

                tvParams.SetParameter(i - 1, value);

                hapticsVal += Time.deltaTime * value * 10.0f;
                if (hapticsVal > 1.0f)
                {
                    hapticsVal -= 1.0f;

                    ushort hapticsAmount = (ushort)(1800.0f * (1.0f - value));
                    SteamVR_TrackedObject device = devices[i].GetComponent<SteamVR_TrackedObject>();
                    if (device != null && device.index != SteamVR_TrackedObject.EIndex.Hmd)
                    {
                        SteamVR_Controller.Input((int)device.index).TriggerHapticPulse(hapticsAmount, EVRButtonId.k_EButton_SteamVR_Touchpad);
                    }
                }
                
            }

            // Manage audio and UI
            if (AudioManager.instance != null)
            {
                float avarage = 0.0f;
                float avarageRot = 0.0f;
                for (int i = 1; i < deviceNoises.Length; ++i)
                {
                    avarage += deviceNoises[i];
                    avarageRot += deviceRotationNoises[i];
                }

                avarage = avarage / (deviceNoises.Length - 1);
                avarageRot = avarageRot / (deviceNoises.Length - 1);
                avarage = Mathf.Clamp01(avarage);
                avarageRot = Mathf.Clamp01(avarageRot);

                float connectionAmount = noiseCurve.Evaluate(avarage) + rotationNoiseCurve.Evaluate(avarageRot);
                connectionBar[0].SetActive(connectionAmount < 0.8f);
                connectionBar[1].SetActive(connectionAmount < 0.6f);
                connectionBar[2].SetActive(connectionAmount < 0.4f);
                connectionBar[3].SetActive(connectionAmount < 0.2f);
                connectionBar[4].SetActive(connectionAmount < 0.1f);

                AudioManager.instance.distortion = connectionAmount;
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

    void dBug(int thing)
    {
        Debug.Log(thing);
    }

    //recursive compound minimum distance of n points

    void recCompMin(List<int> used)
    {
        if (used.Count == devices.Count-1)
        {
            //check min length and do it
            float temp = 0f;
            for (int i = 1; i < devices.Count; i++)
            {
                temp += Vector3.Distance(channels[currentChannel].poses[i].pos, devices[used[i-1]].position);
            }
            if (temp < globalMinDist)
            {
                globalMinDist = temp;
                bestPairing = new List<int>();
                used.ForEach(addToBest);
            }
            return;
        }
        for (int i = 1; i < devices.Count; i++)
        {
            if (!used.Contains(i))
            {
                used.Add(i);
                recCompMin(used);
                used.RemoveAt(used.Count - 1);
            }

        }
    }
    void addToBest(int x)
    {
        bestPairing.Add(x);
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
        for (int i = 1; i < 5; ++i)
        {
            float value = noiseCurve.Evaluate(0.0f);
            tvParams.SetParameter(i - 1, value);
        }

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
