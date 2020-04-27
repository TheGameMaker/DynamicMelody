using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;

[RequireComponent (typeof (Light))]
public class LightAudio : MonoBehaviour {
    public int band;
    public float minIntensity, maxIntensity;
    Light _light;

    public AudioVisualizer viz;

    // Use this for initialization
    void Start () {
        _light = GetComponent<Light>();
	}

    // Update is called once per frame
    void Update () {
        _light.intensity = (viz.audioBandBuffer[band] * (maxIntensity - minIntensity)) + minIntensity;
	}
}
