using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour {

    public GameObject questMan;
    public Rigidbody2D rigid;
    public SpriteRenderer sr;
    public SpriteRenderer cloudSR;
    public SpriteRenderer iconSR;
    public GUISkin gs;
    //public GameObject triggerObject;
    public float speed = 1000.0f;
    public Vector2 moveVelocity;
    public Sprite[] horizontalWalkAnimation;
    public Sprite[] downWalkAnimation;
    public Sprite[] upWalkAnimation;
    public float horAxis;
    public float verAxis;
    public int curAnimState = 0;
    public float animTimer = 0.0f;
    public int lastDirection = 2;
    public Quests.Moving[] movingList;
    public Sprite[] dialogIcons;
    public bool isDialoging;
    public int requestingDialogIcon;
    public Sprite[] thoughtsIcons;
    public string[] subStrings;
    public float thoughtsTimer;
    public float thoughtsPeriod;
    public float thoughtsDuration;
    public bool isThoughtsPeriodical;       //or permanent
    public bool isThoughtsOnlyWhenNear;     //or always
    public bool isPlayerStayOnTrigger;
    public GameObject pl;
    public Sprite thoughtSprite;
    public Sprite dialogSprite;

	// Use this for initialization
	void Start () {
        //moveNPCCords(1.12f, 0.35f, 1.0f);
        moveNPCXCord(1.12f);
        //moveNPCYCord(0.35f);
        pl = GameObject.FindGameObjectWithTag("Player");
	}

    /*IEnumerator Start () {
        //moveNPCCords(1.12f, 0.35f, 1.0f);
        yield return StartCoroutine(moveNPCXCordCor(1.12f, 1.0f));
        moveNPCYCord(0.35f);
	}*/
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("Use") && isPlayerStayOnTrigger)
        {
            requestingDialogIcon++;
            if (requestingDialogIcon >= dialogIcons.Length)
            {
                requestingDialogIcon = (-1);
                //triggerObject.SendMessage("AllowMoving", true);
                pl.SendMessage("AllowMoving", true);
                isDialoging = false;
            }
            else if (requestingDialogIcon == 0)
            {
                //triggerObject.SendMessage("AllowMoving", false);
                pl.SendMessage("AllowMoving", false);
                isDialoging = true;
            }
            //triggerObject.SendMessage("DialogIconIndex", requestingDialogIcon);
            //pl.SendMessage("DialogIconIndex", requestingDialogIcon);
            cloudSR.sprite = dialogSprite;
            iconSR.sprite = thoughtsIcons[requestingDialogIcon];
            cloudSR.enabled = isDialoging;
            iconSR.enabled = isDialoging;
        }
        if (!isDialoging)
        {
            if (isThoughtsPeriodical)
            {
                if (cloudSR.enabled)
                {
                    if (thoughtsTimer < thoughtsDuration)
                    {
                        thoughtsTimer += Time.deltaTime;
                    }
                    else
                    {
                        cloudSR.enabled = false;
                        iconSR.enabled = false;
                        thoughtsTimer = 0.0f;
                        requestingDialogIcon++;
                        if (requestingDialogIcon >= thoughtsIcons.Length)
                        {
                            requestingDialogIcon = 0;
                        }
                        iconSR.sprite = thoughtsIcons[requestingDialogIcon];
                    }
                }
                else
                {
                    if (thoughtsTimer < thoughtsPeriod)
                    {
                        thoughtsTimer += Time.deltaTime;
                    }
                    else
                    {
                        cloudSR.sprite = thoughtSprite;
                        cloudSR.enabled = true;
                        iconSR.enabled = true;
                        thoughtsTimer = 0.0f;
                    }
                }
            }
            else if (thoughtsIcons.Length > 0 && !isThoughtsOnlyWhenNear)
            {
                if (!cloudSR.enabled)
                {
                    cloudSR.enabled = true;
                }
                if (cloudSR.sprite != thoughtSprite)
                {
                    cloudSR.sprite = thoughtSprite;
                }
                if (iconSR.sprite != thoughtsIcons[requestingDialogIcon])
                {
                    iconSR.sprite = thoughtsIcons[requestingDialogIcon];
                }
            }
        }
	}

    void OnTriggerEntered(bool isIt)
    {
        isPlayerStayOnTrigger = isIt;
        if (isIt)
        {
            if (isThoughtsOnlyWhenNear && !isDialoging)
            {
                cloudSR.enabled = true;
                cloudSR.sprite = thoughtSprite;
                iconSR.enabled = true;
                iconSR.sprite = thoughtsIcons[requestingDialogIcon];
            }
        }
        else
        {
            cloudSR.enabled = false;
            iconSR.enabled = false;
        }
    }

    /*void OnGUI()
    {
        GUI.skin = gs;
        if (Input.GetKey("Use") && isPlayerStayOnTrigger)
        {
            GUI.Label(new Rect(0, Screen.height-50, Screen.width, 30), subStrings[requestingDialogIcon]);
        }
    }*/

    void OnGUI()
    {
        GUI.skin = gs;
        if (Input.GetButton("Subtitles") && requestingDialogIcon >= 0 && subStrings.Length > 0)
        {
            GUI.Label(new Rect(0, Screen.height - 50, Screen.width, 30), subStrings[requestingDialogIcon]);
        }
    }

    /*void SetViewDirection(int dir)
    {
        lastDirection = dir;
    }*/

    void SetMovingList(Quests.Moving[] movings)
    {
        movingList = movings;
        StartCoroutine(MoveInList());
    }

    IEnumerator MoveInList()
    {
        Debug.Log("1");
        for (int i = 0; i < movingList.Length; i++)
        {
            Debug.Log("2");
            if (movingList[i].isWaitFor)
            {
                if (movingList[i].isX)
                {
                    yield return new WaitForSeconds(movingList[i].value);       //just wait
                }
                else
                {
                    lastDirection = Mathf.RoundToInt(movingList[i].value);      //change view direction
                    computeMoveAnim();
                }
            }
            else
            {
                Debug.Log("3 " + movingList[i].value);
                if (movingList[i].isX)
                {
                    Debug.Log("4-1-1");
                    yield return StartCoroutine(moveNPCXCordCor(movingList[i].value, 1.0f));    //move on X
                    Debug.Log("4-1-2");
                }
                else
                {
                    Debug.Log("4-2-1");
                    yield return StartCoroutine(moveNPCYCordCor(movingList[i].value, 1.0f));    //move on Y
                    Debug.Log("4-2-2");
                }
            }
        }
        questMan.SendMessage("FinishedMoves");
    }

    void SetDialogList(Sprite[] _sprts)
    {
        requestingDialogIcon = (-1);
        dialogIcons = _sprts;
    }

    void SetThoughtsList(QuestManager.ThoughtsList _thoughts)
    {
        cloudSR.enabled = false;
        requestingDialogIcon = 0;
        thoughtsIcons = _thoughts.sprts;
        subStrings = _thoughts.subtitles;
        thoughtsPeriod = _thoughts.period;
        thoughtsDuration = _thoughts.duration;
        isThoughtsPeriodical = _thoughts.isPeriodical;
        isThoughtsOnlyWhenNear = _thoughts.isOnlyWhenNear;
        //pl.SendMessage("Subs", subStrings);
        //pl.SendMessage("DialogIconIndex", requestingDialogIcon);
    }

    public void moveNPC(float hor, float ver, float duration)
    {
        StartCoroutine(moveNPCCor(hor, ver, 1.0f, duration));
    }
    public void moveNPC(float hor, float ver, float speedMultiplier, float duration)
    {
        StartCoroutine(moveNPCCor(hor, ver, speedMultiplier, duration));
    }
    /*public void moveNPCCords(float toX, float toY)
    {
        StartCoroutine(moveNPCCordsCor(toX, toY, 1.0f));
    }
    public void moveNPCCords(float toX, float toY, float speedMultiplier)
    {
        StartCoroutine(moveNPCCordsCor(toX, toY, speedMultiplier));
    }*/
    public void moveNPCXCord(float toX)
    {
        StartCoroutine(moveNPCXCordCor(toX, 1.0f));
    }
    public void moveNPCXCord(float toX, float speedMultiplier)
    {
        StartCoroutine(moveNPCXCordCor(toX, speedMultiplier));
    }
    public void moveNPCYCord(float toY)
    {
        StartCoroutine(moveNPCYCordCor(toY, 1.0f));
    }
    public void moveNPCYCord(float toY, float speedMultiplier)
    {
        StartCoroutine(moveNPCYCordCor(toY, speedMultiplier));
    }

    IEnumerator moveNPCCor(float hor, float ver, float speedMultiplier, float duration)
    {
        float timer = 0.0f;
        while (timer < duration)
        {
            horAxis = hor * Time.deltaTime * speed * speedMultiplier;
            verAxis = ver * Time.deltaTime * speed * speedMultiplier;
            computeMoveAnim();
            UpdateLayer();
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        verAxis = 0.0f;
        horAxis = 0.0f;
    }

    /*IEnumerator moveNPCCordsCor(float toX, float toY, float speedMultiplier)
    {
        Vector3 pos = gameObject.transform.position;
        float xMul = 1.0f / (toX - pos.x);
        float yMul = 1.0f / (toY - pos.y);
        //while (Mathf.Abs(toX - pos.x) > 0.05f)
        float duration = new Vector2(toX - pos.x, toY - pos.y).magnitude / 0.4f;
        Debug.Log(xMul);
        Debug.Log(yMul);
        Debug.Log(duration);
        float timer = 0.0f;
        while (timer < duration)
        {
            verAxis = xMul * 0.4f * Time.deltaTime * speed * speedMultiplier;
            horAxis = yMul * 0.4f * Time.deltaTime * speed * speedMultiplier;
            computeMoveAnim();
            UpdateLayer();
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        verAxis = 0.0f;
        horAxis = 0.0f;
    }*/
    IEnumerator moveNPCXCordCor(float toX, float speedMultiplier)
    {
        yield return new WaitForSeconds(1.0f);
        Vector3 pos = gameObject.transform.position;
        float duration = (toX - pos.x) - 0.1f/* * 0.4f*/;
        float timer = 0.0f;
        //Debug.Log(duration);
        while (timer < duration)
        {
            horAxis = Time.deltaTime * speed * speedMultiplier;
            computeMoveAnim();
            UpdateLayer();
            timer += Time.deltaTime;
            Debug.Log("timer " + timer);
            yield return new WaitForEndOfFrame();
        }
        horAxis = 0.0f;
        rigid.velocity = new Vector2(0.0f, rigid.velocity.y);
        pos.x = toX;
        gameObject.transform.position = pos;
        computeMoveAnim();
        UpdateLayer();
    }
    IEnumerator moveNPCYCordCor(float toY, float speedMultiplier)
    {
        yield return new WaitForSeconds(1.0f);
        Vector3 pos = gameObject.transform.position;
        float duration = (toY - pos.y) - 0.1f/* * 0.4f*/;
        float timer = 0.0f;
        //Debug.Log(duration);
        while (timer < duration)
        {
            verAxis = Time.deltaTime * speed * speedMultiplier;
            computeMoveAnim();
            UpdateLayer();
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        verAxis = 0.0f;
        rigid.velocity = new Vector2(rigid.velocity.x, 0.0f);
        pos.y = toY;
        gameObject.transform.position = pos;
        computeMoveAnim();
        UpdateLayer();
    }

    void computeMoveAnim()
    {
        if (verAxis == 0.0f && horAxis == 0.0f)
        {
            switch (lastDirection)
            {
                case 0:
                    sr.sprite = upWalkAnimation[0];
                    break;
                case 1:
                case 3:
                    sr.sprite = horizontalWalkAnimation[0];
                    break;
                case 2:
                    sr.sprite = downWalkAnimation[0];
                    break;
            }
        }
        else
        {
            animTimer += Time.deltaTime;
            if (animTimer >= 0.08f)
            {
                curAnimState++;
                if (curAnimState >= 4)
                {
                    curAnimState = 0;
                }
                animTimer = 0.0f;
            }
            if (Mathf.Abs(verAxis) < Mathf.Abs(horAxis))
            {
                sr.sprite = horizontalWalkAnimation[curAnimState];
                sr.flipX = horAxis < 0.0f;
                lastDirection = sr.flipX ? 3 : 1;
            }
            else
            {
                if (verAxis > 0.0f)
                {
                    sr.sprite = upWalkAnimation[curAnimState];
                    lastDirection = 0;
                }
                else
                {
                    sr.sprite = downWalkAnimation[curAnimState];
                    lastDirection = 2;
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (horAxis != 0.0f || verAxis != 0.0f)
        {
            rigid.AddForce(new Vector2(horAxis, verAxis), ForceMode2D.Force);
        }
    }

    void UpdateLayer()
    {
        Vector3 pos = gameObject.transform.position;
        pos.z = pos.y / 100.0f;
        gameObject.transform.position = pos;
    }
}
