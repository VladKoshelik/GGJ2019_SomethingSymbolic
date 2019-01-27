using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour {

    public struct ThoughtsList
    {
        public Sprite[] sprts;
        public string[] subtitles;
        public float period;
        public float duration;
        public bool isPeriodical;
        public bool isOnlyWhenNear;
        public ThoughtsList(Sprite[] _sprts, string[] _subtitles, float _period, float _duration, bool _isPeriodical, bool _isOnlyWhenNear)
        {
            sprts = _sprts;
            subtitles = _subtitles;
            period = _period;
            duration = _duration;
            isPeriodical = _isPeriodical;
            isOnlyWhenNear = _isOnlyWhenNear;
        }
    }

    public Quests.Quest[] questArray;
    public GameObject[] PCGOs;
    public GameObject[] cameras;
    public AudioClip[] audios;
    public GameObject audioMan;
    public Sprite[] dialogIcons;
    public int currentQuest;
    public int currentCamera;
    //public bool isPlayerOnTrigger;
    public int extraVariable;
    public bool autoNext;
    public bool waitingForMovingEnd;

	// Use this for initialization
	void Start () {
        questArray = Quests.GetQuestList();
        currentQuest = (-1);
        currentCamera = 0;
        waitingForMovingEnd = true;
        //Debug.Log(questArray.Length);
        FinishedMoves();    //first launch
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEntered(PersonTrigger.Transit trans)
    {
        /*isPlayerOnTrigger = _isPlayerOnTrigger;
        if (isPlayerOnTrigger)
        {

        }*/
        cameras[trans.from].SetActive(false);
        currentCamera = trans.to;
        cameras[currentCamera].SetActive(true);
        ChangePCGOs();
        if (trans.from == 5 && trans.to == 6)
        {
            AudioSource aSo = audioMan.GetComponents<AudioSource>()[0];
            aSo.Stop();
            aSo = audioMan.GetComponents<AudioSource>()[1];
            aSo.Stop();
        }
    }

    void ChangePCGOs()
    {
        PCGOs = new GameObject[10];
        Transform trans = cameras[currentCamera].transform.Find("Player");
        if (trans != null)
        {
            PCGOs[0] = trans.gameObject;
            PCGOs[0].SendMessage("SetQuestManager", gameObject);
        }
        trans = cameras[currentCamera].transform.Find("Mom");
        /*if (currentCamera == 4)
        {
            Debug.Log(trans);
            Debug.Log(trans.gameObject);
        }*/
        if (trans != null)
        {
            PCGOs[1] = trans.gameObject;
            PCGOs[1].SendMessage("SetQuestManager", gameObject);
            /*if (currentCamera == 4)
            {
                Debug.Log(PCGOs[1]);
            }*/
        }
        trans = cameras[currentCamera].transform.Find("Papa");
        if (trans != null)
        {
            PCGOs[2] = trans.gameObject;
            PCGOs[2].SendMessage("SetQuestManager", gameObject);
        }
    }

    void StartEvent(int questIndex)
    {
        currentQuest = questIndex;
        FinishedMoves(false);
    }

    void FinishedMoves()
    {
        if (waitingForMovingEnd)
        {
            FinishedMoves(true);
        }
    }

    void FinishedMoves(bool incr)
    {
        Debug.Log("FinishedMoves");
        if (incr)
        {
            if (currentQuest >= 0)
            {
                if (questArray[questArray[currentQuest].nextQuest].resetPreviousDialogs)
                {
                    Debug.Log("A1");
                    PCGOs[questArray[currentQuest].personID].SendMessage("ResetPreviousDialogs");
                }
                if (questArray[questArray[currentQuest].nextQuest].resetPreviousThoughts)
                {
                    PCGOs[questArray[currentQuest].personID].SendMessage("ResetPreviousThoughts");
                }
                currentQuest = questArray[currentQuest].nextQuest;
            }
            else
            {
                currentQuest = 0;
            }
        }
        autoNext = questArray[currentQuest].autoNext;
        waitingForMovingEnd = true;
        if (questArray[currentQuest].changeMusic)
        {
            AudioSource aSo = audioMan.GetComponents<AudioSource>()[0];
            aSo.Stop();
            aSo.clip = audios[(int)questArray[currentQuest].musicIndex];
            aSo.loop = questArray[currentQuest].loopMusic;
            aSo.Play();
        }
        if (questArray[currentQuest].changeSound)
        {
            AudioSource aSo = audioMan.GetComponents<AudioSource>()[1];
            aSo.Stop();
            aSo.clip = audios[(int)questArray[currentQuest].soundIndex];
            aSo.loop = questArray[currentQuest].loopSound;
            aSo.Play();
        }
        if (questArray[currentQuest].stopPrevMusic)
        {
            audioMan.GetComponents<AudioSource>()[0].Stop();
        }
        if (questArray[currentQuest].stopPrevSound)
        {
            audioMan.GetComponents<AudioSource>()[1].Stop();
        }
        if (questArray[currentQuest].changeScene)
        {
            Application.LoadLevel(questArray[currentQuest].sceneIndex);
        }
        else if (questArray[currentQuest].changeCamera)
        {
            cameras[currentCamera].SetActive(false);
            currentCamera = questArray[currentQuest].cameraIndex;
            cameras[currentCamera].SetActive(true);
            ChangePCGOs();
        }
        if (questArray[currentQuest].isMoving)
        {
            //Debug.Log("B-1");
            //Debug.Log("FinishedMoves-isMoving");
            PCGOs[questArray[currentQuest].personID].SendMessage("SetMovingList", questArray[currentQuest].movings);
            Debug.Log(waitingForMovingEnd);
            waitingForMovingEnd = questArray[currentQuest].sendMovingEnd;
            
        }
        else
        {
            if (questArray[currentQuest].movings!=null && questArray[currentQuest].movings.Length > 0)
            {
                StartCoroutine(JustWaitHere(questArray[currentQuest].movings[0].value));
            }
           // else
           // {
                if (questArray[currentQuest].isThoughting)
                {
                    //Debug.Log("FinishedMoves-isThoughting");
                    Quests.CloudSet cs = questArray[currentQuest].thoughtSeries;
                    Sprite[] sprts = new Sprite[cs.thoughtsIcons.Length];
                    Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAAAAAA " + sprts.Length);
                    //Debug.Log(sprts.Length);
                    for (int i = 0; i < sprts.Length; i++)
                    {
                        sprts[i] = dialogIcons[(int)cs.thoughtsIcons[i]];
                    }
                    PCGOs[questArray[currentQuest].personID].SendMessage("SetThoughtsList", new ThoughtsList(sprts, cs.subtitles, cs.period, cs.duration, cs.isPeriodical, cs.isOnlyWhenNear));
                    Debug.Log("THOU SET");
                }
                else if (questArray[currentQuest].dialogSeries != null)
                {
                    Sprite[] sprts = new Sprite[questArray[currentQuest].dialogSeries.Length];
                    for (int i = 0; i < sprts.Length; i++)
                    {
                        sprts[i] = dialogIcons[(int)questArray[currentQuest].dialogSeries[i]];
                    }
                    PCGOs[questArray[currentQuest].personID].SendMessage("SetDialogList", sprts);
                }
            }
//        }
    }

    IEnumerator JustWaitHere(float forHowMany)
    {
        yield return new WaitForSeconds(forHowMany);
        if (autoNext)
        {
            FinishedMoves();
        }
    }
}
