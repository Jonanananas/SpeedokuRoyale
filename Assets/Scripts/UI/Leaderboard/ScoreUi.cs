using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScoreUi : MonoBehaviour {
    public RowUi rowUi;
    public ScoreManager scoreManager;

    void Start() {

        scoreManager.AddScore(new Score("huhuu", 12345));
        scoreManager.AddScore(new Score("sample", 50));
        scoreManager.AddScore(new Score("test", 509));
        scoreManager.AddScore(new Score("huhuu", 123456));
        scoreManager.AddScore(new Score("sample", 50));
        scoreManager.AddScore(new Score("sample", 50));
        scoreManager.AddScore(new Score("test", 509));
        scoreManager.AddScore(new Score("huhuu", 123456));
        scoreManager.AddScore(new Score("sample", 50));
        scoreManager.AddScore(new Score("sample", 50));




        var scores = scoreManager.GetHighScores().ToArray();
        for (int i = 0; i < scores.Length; i++) {
            var row = Instantiate(rowUi, transform).GetComponent<RowUi>();
            row.rank.text = (i + 1).ToString();
            row.playerName.text = scores[i].name;
            row.score.text = scores[i].score.ToString();
            
        }

    }
}