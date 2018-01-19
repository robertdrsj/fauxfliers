using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartManager : MonoBehaviour {

    public static StartManager instance = null;
    GameObject canvas;
    CanvasGroup startUI;

	// Use this for initialization
	void Awake () {
        // Makes StartManager a singleton (only one instance runs ever)
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    void Start()
    {
        canvas = GameObject.Find("StartMenu");
        startUI = canvas.GetComponent<CanvasGroup>();
        startUI.alpha = 0;
    }
	
	// Update is called once per frame
	void Update () {
        startUI.alpha += Time.deltaTime * 0.3f;

        if(Input.anyKeyDown)
        {
            SceneManager.LoadScene("Main Game");
        }
    }
}