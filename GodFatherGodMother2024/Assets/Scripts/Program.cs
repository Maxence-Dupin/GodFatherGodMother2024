using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;

public class Program : MonoBehaviour
{
    const int DIVIDING_NUMBER = 1000000000; //Diminishing const

    private IEnumerator coroutine; //Coroutine

    public USBDeviceName headCheck = USBDeviceName.None; //HeadState

    // Start is called before the first frame update
    void Start()
    {
        coroutine = ClockForUSBCheck();
        StartCoroutine(coroutine);

        DriveInfo[] allDrives = DriveInfo.GetDrives();
        List <USBDeviceInfo> testList = GetUSBDevices();

        /*
        foreach (USBDeviceInfo d in testList)
        {
            Debug.Log(d.DeviceID);
            Debug.Log(d.TotalSize);
            Debug.Log(d.USBDeviceName);
        }
        */


        Debug.Log(GetHead());
    }

    static List<USBDeviceInfo> GetUSBDevices()
    {
        List<USBDeviceInfo> devices = new List<USBDeviceInfo> ();
        DriveInfo[] allDrives = DriveInfo.GetDrives();
        foreach (var device in allDrives)
        {
            if(device.IsReady == true)
            {
                if(device.TotalSize / DIVIDING_NUMBER < 4 && device.TotalSize / DIVIDING_NUMBER > 0)
                {
                    devices.Add(new USBDeviceInfo(device.Name, device.TotalSize / DIVIDING_NUMBER));
                }
            }
        }
        return devices;
    }

    USBDeviceName GetHead()
    {
        List<USBDeviceInfo> USBDeviceInfos = GetUSBDevices();
        if(USBDeviceInfos.Count == 1) return USBDeviceInfos[0].USBDeviceName;
        if (USBDeviceInfos.Count > 1) return USBDeviceName.Multiple;
        return USBDeviceName.None;
    }

    //Clock
    private IEnumerator ClockForUSBCheck()
    {
        headCheck = GetHead();
        Debug.Log("Chech");
        yield return new WaitForSeconds(.1f);
        StartCoroutine(ClockForUSBCheck());
    }
}


class USBDeviceInfo
{
    public USBDeviceInfo(string deviceID, long totalSize)
    {
        this.DeviceID = deviceID;
        this.TotalSize = totalSize;
        USBDeviceName = (USBDeviceName)totalSize;
    }
    public string DeviceID { get; private set; }
    public long TotalSize { get; private set; }

    public USBDeviceName USBDeviceName { get; private set; }
}

public enum USBDeviceName
{
    LeftHead = 1,
    MiddleHead = 2,
    RightHead = 3, 
    Multiple = 4,
    None = 5
}
