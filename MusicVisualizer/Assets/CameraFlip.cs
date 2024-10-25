using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFlip : MonoBehaviour
{
    public bool flip = false;
    [Range(-1, 1)]
    [SerializeField] private float xFlip = 1;
    [Range(-1, 1)]
    [SerializeField] private float yFlip = 1;

    private Camera cameraFlip;
    private Matrix4x4 origialProjectionMatrix;

    // Start is called before the first frame update
    void Start()
    {
        cameraFlip = GetComponent<Camera>();

        origialProjectionMatrix = cameraFlip.projectionMatrix;
        
    }

    // Update is called once per frame
    void Update()
    {

        
    }

    private void OnPreCull()
    {
        if (flip)
        {
            cameraFlip.ResetWorldToCameraMatrix();
            cameraFlip.ResetProjectionMatrix();
            cameraFlip.projectionMatrix = cameraFlip.projectionMatrix * Matrix4x4.Scale(new Vector3(xFlip, yFlip, 1));
            //flip = false;
        }
    }

    private void OnPreRender()
    {
        GL.invertCulling = true;
    }

    private void OnPostRender()
    {
        GL.invertCulling = false;
    }
}
