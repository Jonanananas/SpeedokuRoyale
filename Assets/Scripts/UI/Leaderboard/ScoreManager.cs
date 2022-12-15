using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

/// <summary>
/// Manage the scores which are displayed on the leaderboard
/// </summary>
public class ScoreManager : MonoBehaviour {
    public static ScoreManager Instance;
    private ScoreData sd;
    void Awake() {
        #region Singleton
        if (Instance != null) {
            Destroy(gameObject);
            // Trace.LogError("More than one instance of " + this.GetType().Name + " found!");
            return;
        }
        Instance = this;
        // Don't destroy this gameobject on load
        DontDestroyOnLoad(gameObject);
        #endregion
        sd = new ScoreData();
    }

    public IEnumerable<Score> GetHighScores() {
        Parse();
        return sd.scores.OrderByDescending(x => x.score);
    }

    public void AddScore(Score score) {
        print(score.name);
        print(score.score);
        sd.scores.Add(score);
    }

    public void ClearScores() {
        sd.scores.Clear();
    }

    void Parse() {
        //Parse length to 10
        while (sd.scores.Count > 10) {
            sd.scores.RemoveAt(sd.scores.Count - 1);
        }
    }
}
