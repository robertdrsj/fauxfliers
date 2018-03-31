using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseHover : MonoBehaviour {

    PlayerController player;

	void Start () {
        player = FindObjectOfType<PlayerController>();
	}
	
    private void OnMouseOver()
    {
        player.mouseOverDashboard = true;
    }

    private void OnMouseExit()
    {
        player.mouseOverDashboard = false;
    }
}
