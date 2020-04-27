using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class addDisplays : MonoBehaviour
{
    // Start is called before the first frame update
    public Camera cam1;
    public Canvas canvas1;
    public Camera cam2;
    public Canvas canvas2;
    void Start()
    {
        for(int i = 0; i < Display.displays.Length; i++)
        {
           // Display.displays[i].Activate();
            if (i > 0)
            {
                Display.displays[i].Activate(1000, 1000, 60);
            }
            else
            {
                Display.displays[i].Activate();
            }
        }
    }

    public void swapDisplays(bool swap)
    {
        if (swap)
        {
            for (int i = 0; i < Display.displays.Length; i++)
            {
                // Display.displays[i].Activate();
                if (i > 0)
                {
                    Display.displays[i].Activate();
                }
                else
                {
                    Display.displays[i].Activate(1000, 1000, 60);
                }
            }

            cam1.targetDisplay = 2;
            cam2.targetDisplay = 1;
            canvas1.targetDisplay = 2;
            canvas2.targetDisplay = 1;
        }
        else
        {
            for (int i = 0; i < Display.displays.Length; i++)
            {
                // Display.displays[i].Activate();
                if (i > 0)
                {
                    Display.displays[i].Activate(1000, 1000, 60);
                }
                else
                {
                    Display.displays[i].Activate();
                }
            }

            cam1.targetDisplay = 1;
            cam2.targetDisplay = 2;
            canvas1.targetDisplay = 1;
            canvas2.targetDisplay = 2;
        }
    }

}
