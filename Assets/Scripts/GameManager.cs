using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    // Public variables
    public static GameManager instance = null;
    public GameObject[] BorderParent;

    // Private variables
    GameObject canvas;
    CanvasGroup gameOverScreen;
    bool gameOverState = false;
    float waitForTime = 1f;

    void Awake()
    {
        // Makes GameManager a singleton (only one instance runs ever)
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        foreach (GameObject border in BorderParent)
            Instantiate(border);
    }

	// Use this for initialization
	void Start () {
        canvas = GameObject.Find("GameOverScreen");
        gameOverScreen = canvas.GetComponent<CanvasGroup>();
        gameOverScreen.alpha = 0;
    }
	
	// Update is called once per frame
	void Update () {

        // if player dies -> game over
        if (GameObject.Find("Player") == null)
            gameOverState = true;

        // waits for 1 second before accepting input
        if (gameOverState)
        {
            if (waitForTime <= 0)
            {
                if (Input.anyKey)
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            else
                waitForTime -= Time.deltaTime;

        }
    }
}
