using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

//[RequireComponent(typeof(UnityEngine.Experimental.Rendering.Universal.PixelPerfectCamera))]
[RequireComponent(typeof(PixelPerfectCamera))]
public class CameraAspectRatio : MonoBehaviour
{
    Camera cam;
    PixelPerfectCamera pPCamera;
    void Awake()
    {
        cam = GetComponent<Camera>();
        pPCamera = GetComponent<PixelPerfectCamera>();
        cam.aspect = (float)pPCamera.refResolutionX / pPCamera.refResolutionY;
        pPCamera.cropFrameX = true;
        pPCamera.cropFrameY = true;
        pPCamera.stretchFill = true;
    }
}
