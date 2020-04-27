using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;

public class paramCubes : MonoBehaviour
{
    public int band;
    public float startScale, scaleMultiplier;
    public bool useBuffer;
    public bool useLeft;
    Material material;

    public AudioVisualizer viz;

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
            if (useLeft)
            {
                transform.localScale = new Vector3(transform.localScale.x, (viz.audioBandBuffer[band] * scaleMultiplier) + startScale, transform.localScale.z);
                Color color = new Color(1, 0, 0, 1);
                material.SetColor("_EmissionColor", color * viz.audioBand[band]);
            }
            else
            {
                transform.localScale = new Vector3(transform.localScale.x, (viz.audioBandBuffer[band] * scaleMultiplier) + startScale, transform.localScale.z);
                Color color = new Color(1, 0, 0, 1);
                material.SetColor("_EmissionColor", color * viz.audioBand[band]);
            }
        }

        if (!useBuffer)
        {
            transform.localScale = new Vector3(transform.localScale.x, (viz.audioBand[band] * scaleMultiplier) + startScale, transform.localScale.z);
            Color color = new Color(viz.audioBand[band], viz.audioBand[band], viz.audioBand[band]);
            material.SetColor("_EmissionColor", color);
        }
    }
}
