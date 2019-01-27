using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonTrigger : MonoBehaviour {

    public struct Transit
    {
        public int to;
        public int from;
        public Transit(int _to, int _from)
        {
            to = _to;
            from = _from;
        }
    }

    //public bool isOnTriggerStay;
    public GameObject pl;
    public bool isTransition;
    public int toScene;
    public int fromScene;
    //public int spawnDirection;
    public bool alreadyWas;

	// Use this for initialization
	void Start () {
        //pl = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            if (isTransition)
            {
                if (!alreadyWas)
                {
                    pl.SendMessage("OnTriggerEntered", new Transit(toScene, fromScene));
                    alreadyWas = true;
                }
                /*Vector2 pos = col.gameObject.transform.position;
                Vector2 selfPos = gameObject.transform.position;
                Vector2 selfScl = gameObject.transform.lossyScale;
                switch (spawnDirection)
                {
                    case 0:
                        pos.y += selfPos.y + selfScl.y + 0.001f;
                        break;
                    case 1:
                        pos.x += selfPos.x + selfScl.x + 0.001f;
                        break;
                    case 2:
                        pos.y -= selfPos.y + selfScl.y + 0.001f;
                        break;
                    case 3:
                        pos.x -= selfPos.x + selfScl.x + 0.001f;
                        break;
                }
                col.gameObject.transform.position = pos;*/
            }
            else
            {
                gameObject.transform.parent.gameObject.SendMessage("OnTriggerEntered", true);
                //isOnTriggerStay = true;
                pl = col.gameObject;
            }
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (isTransition)
        {
            alreadyWas = false;
        }
        else
        {
            if (col.gameObject.tag == "Player")
            {
                gameObject.transform.parent.gameObject.SendMessage("OnTriggerEntered", false);
                //isOnTriggerStay = true;
            }
        }
    }

    /*void Subs(string[] subtits)
    {
        pl.SendMessage("Subs", subtits);
    }

    void AllowMoving(bool _isAllowedToMove)
    {
        pl.SendMessage("AllowMoving", _isAllowedToMove);
    }

    void DialogIconIndex(int index)
    {
        pl.SendMessage("DialogIconIndex", index);
    }*/
}
