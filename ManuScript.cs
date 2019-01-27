using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManuScript : MonoBehaviour {

    public SpriteRenderer sr;
    public bool isStartGame;
    public bool isExitGame;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnMouseUp()
    {
        if (isStartGame)
        {
            sr.enabled = true;
            Application.LoadLevel(1);
        }
        else if (isExitGame)
        {
            Application.Quit();
        }
    }
}
