using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipControls : MonoBehaviour
{
    Camera mainCamera;
    GameObject ship;


    // Controls
    float rotationSpeed = 30;
    float speed = 0.01f;
    Vector3 localForward;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        ship = GameObject.FindGameObjectWithTag("Ship");
        localForward = transform.worldToLocalMatrix.MultiplyVector(transform.forward);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            ship.transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime, Space.Self);
            transform.Rotate(Vector3.down * rotationSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            ship.transform.Rotate(Vector3.back * rotationSpeed * Time.deltaTime, Space.Self);
            transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            ship.transform.Rotate(Vector3.right * rotationSpeed * Time.deltaTime, Space.Self);
            transform.Rotate(Vector3.right * rotationSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            ship.transform.Rotate(Vector3.left * rotationSpeed * Time.deltaTime, Space.Self);
            transform.Rotate(Vector3.left * rotationSpeed * Time.deltaTime);
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            speed *= 1.1f;
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            speed *= 0.9f;
        }



        if (ship.transform.localRotation != Quaternion.identity)
        {
            ship.transform.localRotation = Quaternion.Lerp(ship.transform.localRotation, Quaternion.identity, 0.02f);
        }


        localForward = transform.localToWorldMatrix.MultiplyVector(Vector3.forward);
        transform.position += localForward * speed;

        mainCamera.transform.localPosition = Vector3.back * 2 + Vector3.up;
        mainCamera.transform.rotation = Quaternion.LookRotation(transform.position - mainCamera.transform.position);

    }
}
