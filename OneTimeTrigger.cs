using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneTimeTrigger : MonoBehaviour {

    public GameObject qm;
    public bool alreadyActivated;
    public int questIndex;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            if (!alreadyActivated)
            {
                qm.SendMessage("StartEvent", questIndex);
                alreadyActivated = true;
            }
        }
    }
}
