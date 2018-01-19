using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour {

    public GameManager gameManager;

    void Awake()
    {
        if (gameManager == null)
            Instantiate(gameManager);
    }
}
