using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Dropdown))]
public class FillMicrophoneList : MonoBehaviour
{
    public Dropdown micMenu;
    // Start is called before the first frame update
    void Start()
    {
        List<string> devices = new List<string>();

        for(int i = 0; i < Microphone.devices.Length; i++)
        {
            devices.Add(Microphone.devices[i].ToString());
        }
        micMenu.AddOptions(devices);
    }

    public void GetMicName(int id)
    {
        AudioVisualizer.selectedDevice = Microphone.devices[id].ToString();
    }
}
