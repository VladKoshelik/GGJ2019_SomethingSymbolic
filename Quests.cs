using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quests {

    public enum GameTexture { Spot, Tank, Tower, TowerExp, Rudes, NoSignal, TowerBroken,
        Wrench, TowerSignal, Good, Happy, Phone, Question, Lamp, Spring, Screwdriver,
        Wires, Button, Vase, Pillow, Clothes, Timer, Peace, Thoughts, Market, Plus,
        Apple, Butter, Flour, Potato };

    public enum GameAudio { TerribleDream, SawAButterfly, AlarmClockBell, Birds, HomeTheme, EndTheme };

    public struct CloudSet
    {
        public GameTexture[] thoughtsIcons;
        public string[] subtitles;
        public float period;
        public float duration;
        public bool isPeriodical;
        public bool isOnlyWhenNear;
        public CloudSet(GameTexture[] _thoughtsIcons, string[] _subtitles, float _period, float _duration, bool _isPeriodical, bool _isOnlyWhenNear)
        {
            thoughtsIcons = _thoughtsIcons;
            subtitles = _subtitles;
            period = _period;
            duration = _duration;
            isPeriodical = _isPeriodical;
            isOnlyWhenNear = _isOnlyWhenNear;
        }
    }

    public struct Moving
    {
        public string name;
        public float value;
        public bool isX;
        public bool isWaitFor;
        /// <summary>
        /// Full ctor
        /// </summary>
        public Moving(string _name, float _value, bool _isX, bool _isWaitFor)
        {
            name = _name;
            value = _value;
            isX = _isX;
            isWaitFor = _isWaitFor;
        }
        /// <summary>
        /// Dir changer
        /// </summary>
        public Moving(float _value, bool _isX, bool _isWaitFor)
        {
            name = "";
            value = _value;
            isX = false;
            isWaitFor = true;
        }
        /// <summary>
        /// Mover
        /// </summary>
        public Moving(float _value, bool _isX)
        {
            name = "";
            value = _value;
            isX = _isX;
            isWaitFor = false;
        }
        /// <summary>
        /// Waiter
        /// </summary>
        public Moving(float _value)
        {
            name = "";
            value = _value;
            isX = true;
            isWaitFor = true;
        }
    }

    public struct Quest
    {
        public string name;
        public int personID;        //0-Player 1-Mother 2-Father
        public bool isMoving;
        public bool isThoughting;
        public Moving[] movings;
        public GameTexture[] dialogSeries;
        public CloudSet thoughtSeries;
        public string endMessage;
        public int nextQuest;
        public bool resetPreviousDialogs;
        public bool resetPreviousThoughts;
        public bool changeCamera;
        public int cameraIndex;
        public bool changeScene;
        public int sceneIndex;      //(-1) if exit to menu
        public bool changeMusic;
        public GameAudio musicIndex;
        public bool changeSound;
        public GameAudio soundIndex;
        public bool loopMusic;
        public bool loopSound;
        public bool stopPrevMusic;
        public bool stopPrevSound;
        public bool autoNext;
        public bool sendMovingEnd;
    }

    public static Quest[] GetQuestList()
    {
        Quest[] questArray = new Quest[10];

        questArray[0].name = "Nigthdream1";
        questArray[0].personID = 0;
        questArray[0].isMoving = true;
        questArray[0].sendMovingEnd = true;
        questArray[0].movings = new Moving[]{
            new Moving(-1.0f, true),
            new Moving(0.5f),
            new Moving(0.0f, false, false),
            new Moving(0.5f),
            new Moving(0.0f, true),
            new Moving(0.5f)
        };
        questArray[0].nextQuest = 1;

        questArray[1].name = "Nigthdream2";
        questArray[1].personID = 0;
        questArray[1].isThoughting = true;
        questArray[1].sendMovingEnd = true;
        questArray[1].thoughtSeries = new CloudSet(
            new GameTexture[] { GameTexture.Spot/*, GameTexture.Tank*/ },
            new string[] { "Ow, a butterfly!" },
            0.01f, 1.5f, false, false);
        questArray[1].nextQuest = 2;

        questArray[2].name = "Nigthdream3";
        questArray[2].personID = 0;
        questArray[2].isMoving = true;
        questArray[2].movings = new Moving[]{
            new Moving(0.5f),
            new Moving(1.168f, true),
            new Moving(0.2f),
            new Moving(0.0f, false, false),
            new Moving(0.5f)
        };
        questArray[2].sendMovingEnd = true;
        questArray[2].resetPreviousThoughts = true;
        questArray[2].nextQuest = 3;

        questArray[3].name = "Nigthdream4";
        questArray[3].isMoving = false;
        questArray[3].autoNext = true;
        questArray[3].movings = new Moving[]{
            new Moving(4.5f)
        };
        questArray[3].changeCamera = true;
        questArray[3].cameraIndex = 1;
        questArray[3].nextQuest = 4;

        questArray[4].name = "Nigthdream5";
        questArray[4].isMoving = false;
        questArray[4].autoNext = true;
        questArray[4].movings = new Moving[]{
            new Moving(3.5f)
        };
        questArray[4].changeCamera = true;
        questArray[4].changeSound = true;
        questArray[4].soundIndex = GameAudio.AlarmClockBell;
        questArray[4].cameraIndex = 2;
        questArray[4].nextQuest = 5;

        questArray[5].name = "Home1";
        questArray[5].isMoving = false;
        questArray[5].movings = new Moving[]{
            new Moving(2.0f)
        };
        questArray[5].changeCamera = true;
        questArray[5].cameraIndex = 3;
        //questArray[5].stopPrevMusic = true;
        //questArray[5].stopPrevSound = true;
        questArray[5].changeMusic = true;
        questArray[5].musicIndex = GameAudio.HomeTheme;
        questArray[5].loopMusic = true;
        questArray[5].nextQuest = 6;

        questArray[6].name = "Home2";
        questArray[6].personID = 1;
        /*questArray[6].isMoving = true;
        questArray[6].movings = new Moving[]{
            new Moving(3.0f),
            new Moving(40.575f, true),
            new Moving(-0.575f, false)
        };*/
        questArray[6].sendMovingEnd = true;
        questArray[6].isThoughting = true;
        questArray[6].thoughtSeries = new CloudSet(
            new GameTexture[] { GameTexture.Thoughts },
            new string[] { "[thinks]" },
            0.5f, 1.5f, false, false);
        questArray[6].nextQuest = 7;

        questArray[7].name = "Home2";
        questArray[7].personID = 1;
        questArray[7].isMoving = true;
        questArray[7].movings = new Moving[]{
            new Moving(3.0f),
            new Moving(40.575f, true),
            new Moving(-0.575f, false)
        };
        questArray[7].sendMovingEnd = false;
        questArray[7].nextQuest = 7;

        /*questArray[1].name = "Nigthdream2";
        questArray[1].personID = 0;
        questArray[1].isThoughting = true;
        questArray[1].sendMovingEnd = true;
        questArray[1].thoughtSeries = new CloudSet(
            new GameTexture[] { GameTexture.Spot },
            new string[] { "Ow, a butterfly!" },
            0.01f, 1.5f, false, false);
        questArray[1].nextQuest = 2;*/

        questArray[7].name = "Home3";
        questArray[7].personID = 1;
        questArray[7].resetPreviousThoughts = false;
        questArray[7].nextQuest = 8;

        return questArray;
    }
}
