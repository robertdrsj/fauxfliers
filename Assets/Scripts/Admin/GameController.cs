using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    // Public variables
    public static GameController instance = null;
    public GameObject[] BorderParent;

    // Private variables
    GameObject canvas;
    CanvasGroup gameOverScreen;
    bool gameOverState = false;
    float waitForTime = 1f;

    void Awake()
    {
        // Makes GameController a singleton (only one instance runs ever)
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        foreach (GameObject border in BorderParent)
            Instantiate(border);
    }

    void Start()
    {
        canvas = GameObject.Find("GameOverScreen");
        gameOverScreen = canvas.GetComponent<CanvasGroup>();
        gameOverScreen.alpha = 0;
    }

    void Update()
    {

        // if player dies -> game over
        if (GameObject.Find("Player") == null)
            gameOverState = true;

        // Commented out because SceneManager doesn't exist (?)

        /*
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
        */
    }
}
