using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAspectController : MonoBehaviour {

    //public int targetWidth;
    //public int targetHeight;

    public float targetAspectA;
    public float targetAspectB;

    float windowAspect;
    float scaleHeight;

	void Start () {

        Camera cam = GetComponent<Camera>();

        float targetAspect = targetAspectA / targetAspectB;         // Set the desired aspect ratio (ex. to get 16:10 -- set A to 16, and B to 10.)
        //Screen.SetResolution(targetWidth, targetHeight, true);     // Set screen resolution. (REMOVE IS TARGET ASPECT WORKS FINE.)

        windowAspect = Screen.width / Screen.height;                // Determine the game window's current aspect ratio.
        scaleHeight = windowAspect / targetAspect;                  // Current viewport height should be scaled by this amount.


        if (scaleHeight < 1.0f)                                     // If scaled height is less than current height, add letter box.
        {
            Rect rect = cam.rect;

            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0f;
            rect.y = (1.0f - scaleHeight) / 2.0f;

            cam.rect = rect;
        }
        else                                                        // Add pillar box.
        {
            float scaleWidth = 1.0f / scaleHeight;

            Rect rect = cam.rect;

            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f;
            rect.y = 0f;

            cam.rect = rect;
        }
	}
}
