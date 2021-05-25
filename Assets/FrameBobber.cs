using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameBobber : MonoBehaviour
{
    public Vector3 target;
    public GameObject head;
    [Range(0,1)]
    public float lerpFactor=0.2f;
    // Use this for initialization
    void OnEnable()
    {
        FaceTrackerClient.OnCenterFaceUpdated += ProcessFrame;
    }

    void OnDisable()
    {
        FaceTrackerClient.OnCenterFaceUpdated -= ProcessFrame;
    }

    void Update()
    {
        head.transform.localPosition = Vector3.Lerp(head.transform.localPosition, target, lerpFactor);
    }

    private void ProcessFrame(FaceTrackerClient.FrameData frame)
    {
        if (frame.centerIdx < 0)
            return;
        Rect center = frame.faces[frame.centerIdx];
        var a = 1.04e4f;
        var b = 2.74f;
        float dist = (a / center.width) - b;

        float xNorm = (center.x + center.width / 2) / frame.width - 0.5f;
        float yNorm = (frame.height - (center.y + center.height / 2)) / frame.height - 0.5f;
        target = new Vector3(xNorm, yNorm, dist / 100);
    }

}
