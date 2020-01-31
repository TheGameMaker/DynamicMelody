using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class paramCubes : MonoBehaviour
{
    public int band;
    public float startScale, scaleMultiplier;
    public bool useBuffer;
    Material material;
    // Use this for initialization
    void Start()
    {
        material = GetComponent<MeshRenderer>().materials[0];
    }

    // Update is called once per frame
    void Update()
    {
        if (useBuffer)
        {
            transform.localScale = new Vector3(transform.localScale.x, (AudioVisualizer.audioBandBuffer[band] * scaleMultiplier) + startScale, transform.localScale.z);
            Color color = new Color(1, 0, 0, 1);
            material.SetColor("EmissionColor", color*AudioVisualizer.audioBand[0]);
        }

        if (!useBuffer)
        {
            transform.localScale = new Vector3(transform.localScale.x, (AudioVisualizer.audioBand[band] * scaleMultiplier) + startScale, transform.localScale.z);
            Color color = new Color(AudioVisualizer.audioBand[band], AudioVisualizer.audioBand[band], AudioVisualizer.audioBand[band]);
            material.SetColor("EmissionColor", color);
        }
    }
}
