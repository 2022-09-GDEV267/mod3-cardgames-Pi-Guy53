using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eScoreEvent {  draw, mine, mineGold, gameWin, gameLoss}

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager s;

    public static int SCORE_FROM_PREV_ROUND;
    public static int HIGH_SCORE = 0;

    public int chain = 0, scoreRun = 0, score = 0;
    public int scoreMulti;
    public static int prevScoreMulti;

    private void Awake()
    {
        s = this;

        if (PlayerPrefs.HasKey("ProspectorHighScore"))
        {
            HIGH_SCORE = PlayerPrefs.GetInt("ProspectorHighScore");
        }

        score += SCORE_FROM_PREV_ROUND;
        SCORE_FROM_PREV_ROUND = 0;

        scoreMulti = 1;
    }

    public static void EVENT(eScoreEvent evt)
    {
        try
        {
            s.Event(evt);
        }
        catch (System.NullReferenceException nre)
        {
            Debug.LogError("ScoreManager:EVENT() called while s=null. \n" + nre);
        }
    }

    void Event(eScoreEvent evt)
    {
        switch (evt)
        {
            case eScoreEvent.draw:
            case eScoreEvent.gameWin:
            case eScoreEvent.gameLoss:
                chain = 0;

                scoreRun *= scoreMulti;

                score += scoreRun;
                scoreRun = 0;

                prevScoreMulti = scoreMulti;
                scoreMulti = 1;
                break;
            case eScoreEvent.mine:
                chain++;
                scoreRun += chain;
                break;
            case eScoreEvent.mineGold:
                chain++;
                scoreRun += chain;
                scoreMulti *= 2;
                break;
        }

        switch (evt)
        {
            case eScoreEvent.gameWin:
                SCORE_FROM_PREV_ROUND = score;
                print("You won: " + score);
                break;
            case eScoreEvent.gameLoss:
                if (HIGH_SCORE <= score)
                {
                    print("New high score: " + score);
                    HIGH_SCORE = score;
                    PlayerPrefs.SetInt("ProspectorHighScore", score);
                }
                else
                {
                    print("You lost, Your score: " + score);
                }
                break;
            default:
                print("Score: " + score + " Score Run: " + scoreRun + " chain: " + chain);
                break;
        }
    }

    public static int CHAIN { get { return s.chain; } }
    public static int SCORE { get { return s.score; } }
    public static int SCORE_RUN { get { return s.scoreRun; } }
}