using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreKeeper : MonoBehaviour
{
    // updates GUI based on Batting Detector and Pitching Detector events

#region bound ui elements
    public TextMeshProUGUI tPitches;
    public TextMeshProUGUI tStrikeouts;
    public TextMeshProUGUI tBbs;
    public TextMeshProUGUI tOuts;
    public TextMeshProUGUI tCurrentCount;
    public TextMeshProUGUI tHits;
    public TextMeshProUGUI tHomers;
    public TextMeshProUGUI tLastResult;
#endregion

    private int pitches;
    private int strikeouts;
    private int bbs;
    private int outs;
    private int hits;
    private int homers;
    
    private string lastResult;

    private int currentStrikes;
    private int currentBalls;

    private Pitcher pitcherScript;
    private MainManager mainManagerScript;

    public bool trackStats = false;


    void Start()
    {
        // determine when to record stats
        mainManagerScript = GameObject.Find("MainManager").GetComponent<MainManager>();
        mainManagerScript.OnPlayBall += (obj, e) => { trackStats = true; };
        mainManagerScript.OnQuitBall += (obj, e) => { 
            trackStats = false;
            ResetAllStats();
        };

        // listen for OnDisplayBallResults events
        pitcherScript = GameObject.Find("PitchingManager").GetComponent<Pitcher>();
        pitcherScript.OnDisplayBallResults += RecordOnScoreboard;

        // setup at start
        SetValues();
    }

    private void RecordOnScoreboard(object sender, BallLifeCycleManager.BallOutcomeData e)
    {
        if (!trackStats)
        {
            return;
        }

        Debug.Log("recording");

        // categorize the incoming event, and register it as the correct related stat
        switch (e.Outcome)
        {
            case BallLifeCycleManager.BallOutcome.Strike:
                currentStrikes++;
                CountCheck();
                break;

            case BallLifeCycleManager.BallOutcome.Ball:
                currentBalls++;
                CountCheck();
                break;

            case BallLifeCycleManager.BallOutcome.HBP:
                bbs++;
                ResetCount();
                break;

            case BallLifeCycleManager.BallOutcome.Hit:
                hits++;
                ResetCount();
                break;

            case BallLifeCycleManager.BallOutcome.Homer:
                homers++;
                hits++;
                ResetCount();
                break;

            case BallLifeCycleManager.BallOutcome.Foul:
                if (currentStrikes < 2)
                {
                    currentStrikes++;
                }
                break;

            case BallLifeCycleManager.BallOutcome.Out:
                outs++;
                ResetCount() ; 
                break;

            default:
                Debug.Log($"Undetectable event {e.Outcome}");
                break;

        }


        // set textual description of the event
        lastResult = e.Outcome.ToString();

        pitches++;
        SetValues();
    }

    private void SetValues()
    {
        tPitches.text = $"Pitches: {pitches}";
        tStrikeouts.text = $"Ks: {strikeouts}";
        tBbs.text = $"BBs: {bbs}";
        tOuts.text = $"Outs: {outs}";
        tHits.text = $"Hits: {hits}";
        tHomers.text = $"HRs: {homers}";
        tLastResult.text = lastResult;

        tCurrentCount.text = $"Count: {currentBalls} - {currentStrikes}";
    }

    private void CountCheck()
    {
        if (currentStrikes >= 3)
        {
            strikeouts++;
            outs++;
            ResetCount();
        }

        if (currentBalls >= 4)
        {
            bbs++;
            ResetCount();
        }
    }

    private void ResetCount()
    {
        currentStrikes = 0;
        currentBalls = 0;
    }
    private void ResetAllStats()
    {
        ResetCount();
        pitches = 0;
        strikeouts = 0;
        bbs = 0;
        outs = 0;
        hits = 0;
        homers = 0;

        SetValues();
    }
}
