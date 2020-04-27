using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Make512Cubes : MonoBehaviour {
    public GameObject sampleCubePrefab;
    GameObject[] sampleCube = new GameObject[512];
    public float maxScale;

    public List<MeshRenderer> cubeMeshRenderers;
    public Material material;
    private Material[] audioMaterials = new Material[512];
    public bool useColor1, useColor2;
    public string colorName1, colorName2;
    public Gradient gradient1, gradient2;
    private Color[] color1, color2;
    [Range(0f, 1f)]
    public float colorThreshold1, colorThreshold2;
    public float colorMultiplier1, colorMultiplier2;
    // Use this for initialization
    void Start () {
        cubeMeshRenderers = new List<MeshRenderer>();
        audioMaterials = new Material[512];
        color1 = new Color[512];
        color2 = new Color[512];
        for (int i = 0; i < 512; i++)
        {
            color1[i] = gradient1.Evaluate((1f / 512f) * i);
            color2[i] = gradient2.Evaluate((1f / 512f) * i);
            audioMaterials[i] = new Material(material);

            GameObject instanceSampleCube = (GameObject)Instantiate (sampleCubePrefab);
            instanceSampleCube.transform.position = this.transform.position;
            instanceSampleCube.transform.parent = this.transform;
            instanceSampleCube.name = "SampleCube" + i;
            this.transform.eulerAngles = new Vector3 (0, -0.703125f * i, 0);
            instanceSampleCube.transform.position = Vector3.forward * 100;
            cubeMeshRenderers.Add(instanceSampleCube.GetComponent<MeshRenderer>());
            cubeMeshRenderers[i].material = audioMaterials[i];
            sampleCube[i] = instanceSampleCube;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
        for (int i = 0; i < 512; i++)
        {
            if(sampleCube != null)
            {
                sampleCube[i].transform.localScale = new Vector3(10, ((AudioVisualizer.samplesLeft[i] + AudioVisualizer.samplesRight[i]) * maxScale) + 2, 10);
            }
            if (useColor1)
            {
                if ((AudioVisualizer.samplesLeft[i] + AudioVisualizer.samplesRight[i]) > colorThreshold1)
                {
                    audioMaterials[i].SetColor(colorName1, color1[i] * (AudioVisualizer.samplesLeft[i] + AudioVisualizer.samplesRight[i]) * colorMultiplier1);
                }
                else
                {
                    audioMaterials[i].SetColor(colorName1, color1[i] * 0f);
                }
            }
            if (useColor2)
            {
                if ((AudioVisualizer.samplesLeft[i] + AudioVisualizer.samplesRight[i]) > colorThreshold2)
                {
                    audioMaterials[i].SetColor(colorName2, color2[i] * (AudioVisualizer.samplesLeft[i] + AudioVisualizer.samplesRight[i]) * colorMultiplier2);
                }
                else
                {
                    audioMaterials[i].SetColor(colorName2, color2[i] * 0f);
                }
            }
        }
    }
}
