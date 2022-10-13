using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class ScoreManager : MonoBehaviour {
    public static ScoreManager Instance;
    private ScoreData sd;
    void Awake() {
        #region Singleton
        if (Instance != null) {
            Trace.LogError("More than one instance of " + this.GetType().Name + " found!");
            return;
        }
        Instance = this;
        #endregion
        sd = new ScoreData();
    }

    public IEnumerable<Score> GetHighScores() {
        parse();
        return sd.scores.OrderByDescending(x => x.score);
    }

    public void AddScore(Score score) {
        sd.scores.Add(score);
    }

    void parse() {
        //Parse length to 10
        while (sd.scores.Count > 10) {
            sd.scores.RemoveAt(sd.scores.Count - 1);
        }
    }
}
