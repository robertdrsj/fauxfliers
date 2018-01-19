using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartLoader : MonoBehaviour {

    public StartManager startMenu;

	// Use this for initialization
	void Awake () {
        if (StartManager.instance == null)
            Instantiate(startMenu);
    }
}
