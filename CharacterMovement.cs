using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour {

    public GameObject questMan;
    public Rigidbody2D rigid;
    public SpriteRenderer sr;
    public GUISkin gs;
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
    //public int secs;
    public bool isPlayerStayOnTrigger;
    public string[] subStrings;
    public bool isAllowedToMove;
    public int requestingDialogIcon;
    public Quests.Moving[] movingList;
    public float thoughtsTimer;
    public float thoughtsPeriod;
    public float thoughtsDuration;
    public bool isThoughtsPeriodical;       //or permanent
    public bool isThoughtsOnlyWhenNear;     //or always
    public Sprite[] dialogIcons;
    public Sprite[] thoughtsIcons;
    public Sprite thoughtSprite;
    public Sprite dialogSprite;
    public SpriteRenderer cloudSR;
    public SpriteRenderer iconSR;
    public bool isDialoging;
    public bool permanentIsAllowedToMove;

	// Use this for initialization
	void Start () {
        
	}

    /*void OnTriggerEntered(bool isIt)
    {
        isPlayerStayOnTrigger = isIt;
    }

    void Subs(string[] subtits)
    {
        subStrings = subtits;
    }*/

    void AllowMoving(bool _isAllowedToMove)
    {
        isAllowedToMove = _isAllowedToMove;
        if (!isAllowedToMove)
        {
            rigid.velocity = Vector2.zero;
            horAxis = 0.0f;
            verAxis = 0.0f;
            computeMoveAnim();
            UpdateLayer();
        }
    }

    void SetQuestManager(GameObject qm)
    {
        questMan = qm;
    }

    /*void DialogIconIndex(int index)
    {
        requestingDialogIcon = index;
    }*/

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

    void OnGUI()
    {
        GUI.skin = gs;
        if (Input.GetButton("Subtitles") && requestingDialogIcon >= 0 && subStrings.Length > 0)
        {
            GUI.Label(new Rect(0, Screen.height - 30, Screen.width, 30), subStrings[requestingDialogIcon]);
        }
    }

	// Update is called once per frame
	void Update () {
        /*if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            secs = System.DateTime.Now.Millisecond;
        }
        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            secs -= System.DateTime.Now.Millisecond;
            Debug.Log(secs);
        }*/
        //Debug.Log(Input.GetAxisRaw("Horizontal") + " " + Input.GetAxisRaw("Vertical"));
        if (isAllowedToMove)
        {
            horAxis = Input.GetAxisRaw("Horizontal") * Time.deltaTime * speed;
            verAxis = Input.GetAxisRaw("Vertical") * Time.deltaTime * speed;
            //}
            if (verAxis == 0.0f && horAxis == 0.0f)
            {
                //sr.sprite = downWalkAnimation[0];
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
                UpdateLayer();
            }
        }
        Dialoging();
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

    void SetMovingList(Quests.Moving[] movings)
    {
        //Debug.Log("B");
        movingList = movings;
        StartCoroutine(MoveInList());
    }
    IEnumerator MoveInList()
    {
        isAllowedToMove = false;
        for (int i = 0; i < movingList.Length; i++)
        {
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
                if (movingList[i].isX)
                {
                    yield return StartCoroutine(moveNPCXCordCor(movingList[i].value, 1.0f));    //move on X
                }
                else
                {
                    yield return StartCoroutine(moveNPCYCordCor(movingList[i].value, 1.0f));    //move on Y
                }
            }
        }
        questMan.SendMessage("FinishedMoves");
        isAllowedToMove = permanentIsAllowedToMove;
    }
    void SetDialogList(Sprite[] _sprts)
    {
        dialogIcons = _sprts;
    }

    void SetThoughtsList(QuestManager.ThoughtsList _thoughts)
    {
        Debug.Log("THOU GET");
        cloudSR.enabled = false;
        requestingDialogIcon = 0;
        thoughtsIcons = _thoughts.sprts;
        Debug.Log("THOU " + thoughtsIcons.Length);
        subStrings = _thoughts.subtitles;
        thoughtsPeriod = _thoughts.period;
        thoughtsDuration = _thoughts.duration;
        isThoughtsPeriodical = _thoughts.isPeriodical;
        isThoughtsOnlyWhenNear = _thoughts.isOnlyWhenNear;
    }
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
                //Debug.Log("horWalkAnim " + curAnimState);
                sr.flipX = horAxis < 0.0f;
                lastDirection = sr.flipX ? 3 : 1;
            }
            else
            {
                if (verAxis > 0.0f)
                {
                    sr.sprite = upWalkAnimation[curAnimState];
                    //Debug.Log("upWalkAnim" + curAnimState);
                    lastDirection = 0;
                }
                else
                {
                    sr.sprite = downWalkAnimation[curAnimState];
                    //Debug.Log("downWalkAnim" + curAnimState);
                    lastDirection = 2;
                }
            }
        }
    }

    void ResetPreviousDialogs()
    {
        Debug.Log("ResetPreviousDialogs");
        dialogIcons = new Sprite[0];
        subStrings = new string[0];
        requestingDialogIcon = (-1);
        cloudSR.enabled = false;
        iconSR.enabled = false;
        thoughtsTimer = 0.0f;
    }
    void ResetPreviousThoughts()
    {
        Debug.Log("ResetPreviousThoughts");
        thoughtsIcons = new Sprite[0];
        subStrings = new string[0];
        requestingDialogIcon = (-1);
        cloudSR.enabled = false;
        iconSR.enabled = false;
        thoughtsTimer = 0.0f;
    }

    void Dialoging()
    {
        if (Input.GetButtonDown("Use") && isPlayerStayOnTrigger)
        {
            requestingDialogIcon++;
            if (requestingDialogIcon >= dialogIcons.Length)
            {
                requestingDialogIcon = (-1);
                AllowMoving(true);
                isDialoging = false;
            }
            else if (requestingDialogIcon == 0)
            {
                AllowMoving(false);
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
                if (!iconSR.enabled)
                {
                    iconSR.enabled = true;
                }
                if (cloudSR.sprite != thoughtSprite)
                {
                    cloudSR.sprite = thoughtSprite;
                }
                if (requestingDialogIcon >= 0 && iconSR.sprite != thoughtsIcons[requestingDialogIcon])
                {
                    iconSR.sprite = thoughtsIcons[requestingDialogIcon];
                }
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
                            //thoughtsIcons = new Sprite[0];
                            requestingDialogIcon = (-1);
                            //requestingDialogIcon = 0;
                            Debug.Log("ACS");
                            questMan.SendMessage("FinishedMoves");
                        }
                        else
                        {
                            iconSR.sprite = thoughtsIcons[requestingDialogIcon];
                        }
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
                        Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAAAAA");
                    }
                }
            }
        }
    }
}
