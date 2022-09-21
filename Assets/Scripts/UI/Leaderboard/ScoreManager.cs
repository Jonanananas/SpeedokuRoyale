using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class ScoreManager : MonoBehaviour {
    private ScoreData sd;
    void Awake() {
        sd = new ScoreData();
    }

    public IEnumerable<Score> GetHighScores() {
        return sd.scores.OrderByDescending(x => x.score);
    }

    public void AddScore(Score score) {
        sd.scores.Add(score);
    }


}
