using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCog : MonoBehaviour {

    public float degreesPerSecond;

	void Update () {
        transform.Rotate(Vector3.up, degreesPerSecond * Time.deltaTime, Space.Self);
	}
}
