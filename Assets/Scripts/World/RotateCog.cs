using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCog : MonoBehaviour {

    public float xDegreesPerSecond;
    public float yDegreesPerSecond;
    public float zDegreesPerSecond;


    public bool rotateX;
    public bool rotateY;
    public bool rotateZ;

	void FixedUpdate () {

        if (rotateX)
            transform.Rotate(Vector3.right, xDegreesPerSecond * Time.deltaTime, Space.Self);

        if (rotateY)
            transform.Rotate(Vector3.up, yDegreesPerSecond * Time.deltaTime, Space.Self);

        if (rotateY)
            transform.Rotate(Vector3.forward, zDegreesPerSecond * Time.deltaTime, Space.Self);
    }
}
