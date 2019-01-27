using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimator : MonoBehaviour {

    private SpriteRenderer sr;
    public bool fromSideToSide;
    public bool animateFrames;
    public Sprite[] frames;
    public float animTimer;
    public int curAnimState;
    public float animatingPeriod;

	// Use this for initialization
	void Start () {
        sr = gameObject.GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
        animTimer += Time.deltaTime;
        if (animTimer >= animatingPeriod)
        {
            doAnimation();
            animTimer = 0.0f;
        }
        if (0 <= curAnimState && curAnimState < frames.Length)
        {
            sr.sprite = frames[curAnimState];
        }
	}

    void doAnimation()
    {
        if (animateFrames)
        {
            curAnimState++;
            if (curAnimState >= frames.Length)
            {
                curAnimState = 0;
            }
        }
        if (fromSideToSide)
        {
            sr.flipX = !sr.flipX;
        }
    }
}
