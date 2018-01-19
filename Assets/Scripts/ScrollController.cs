using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollController : MonoBehaviour {

    public float speedX;
    public float speedY;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Vector2 offset = new Vector2(Time.time * speedX, Time.time * speedY);

        GetComponent<Renderer>().material.mainTextureOffset = offset;
	}
}
