using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowfieldParticle : MonoBehaviour
{
    public float moveSpeed, scale;
    public int audioBand;

    // Start is called before the first frame update
    void Start()
    {
        audioBand = (int)Random.Range(0, 7);
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position += transform.forward * moveSpeed * Time.deltaTime;
    }

    public void ApplyRotation(Vector3 rot, float rotSpeed)
    {
        Quaternion targetRotation = Quaternion.LookRotation(rot.normalized);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotSpeed * Time.deltaTime);
    }
}
