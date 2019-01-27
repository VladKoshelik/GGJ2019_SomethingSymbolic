using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimToLeft : MonoBehaviour {

    public GameObject qm;
    public float duration = 8.5f;
    public float xSrc;
    public float xDest;
    public bool getNext;

	// Use this for initialization
	void Start () {
        xSrc = gameObject.transform.localPosition.x;
        if (xSrc > xDest)
        {
            StartCoroutine(MoveObjToLeft());
        }
        else
        {
            StartCoroutine(MoveObjToRight());
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator MoveObjToLeft()
    {
        Vector3 pos = gameObject.transform.localPosition;
        float speed = (xDest - xSrc) / duration;
        while (pos.x > xDest)
        {
            pos.x += speed * Time.deltaTime;
            gameObject.transform.localPosition = pos;
            yield return new WaitForEndOfFrame();
        }
        pos.x = xDest;
        gameObject.transform.localPosition = pos;
        if (getNext)
        {
            qm.SendMessage("OnTriggerEntered", new PersonTrigger.Transit(6, 5));
        }
    }

    IEnumerator MoveObjToRight()
    {
        Vector3 pos = gameObject.transform.localPosition;
        float speed = (xDest - xSrc) / duration;
        while (pos.x < xDest)
        {
            pos.x += speed * Time.deltaTime;
            gameObject.transform.localPosition = pos;
            yield return new WaitForEndOfFrame();
        }
        pos.x = xDest;
        gameObject.transform.localPosition = pos;
    }
}
