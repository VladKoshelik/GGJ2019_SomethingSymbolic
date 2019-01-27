using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComiscScript : MonoBehaviour {

    public SpriteRenderer sr;
    public Sprite[] comicsSprites;
    //public int curComics;

	// Use this for initialization
	void Start () {
        //curComics = 0;
        StartCoroutine(Comics());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator Comics()
    {
        for (int i = 0; i < comicsSprites.Length; i++)
        {
            sr.sprite = comicsSprites[i];
            yield return new WaitForSeconds(3.5f);
        }
        yield return new WaitForSeconds(10.0f);
        Application.LoadLevel(0);
    }
}
