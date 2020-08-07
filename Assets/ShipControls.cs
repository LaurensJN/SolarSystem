using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipControls : MonoBehaviour
{
    Camera mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        mainCamera.transform.rotation = transform.rotation;
        mainCamera.transform.position = transform.position + transform.rotation * Vector3.back * 2;
    }
}
