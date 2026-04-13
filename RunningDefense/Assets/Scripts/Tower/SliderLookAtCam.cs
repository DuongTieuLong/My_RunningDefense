using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderLookAtCam : MonoBehaviour
{
    private Camera cam;

    public List<GameObject> gameObjects;

    void Start()
    {
        cam = Camera.main;
    }
    void LateUpdate()
    {
        if (cam == null) return;
        foreach (var obj in gameObjects)
        {
            if (obj != null)
            {
                obj.transform.LookAt(cam.transform);
                obj.transform.Rotate(0, 180, 0);
            }
        }
    }
}
