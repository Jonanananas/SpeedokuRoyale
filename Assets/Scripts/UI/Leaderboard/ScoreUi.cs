using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class ScoreUi : MonoBehaviour {
    public RowUi rowUi;
    List<GameObject> rows = new List<GameObject>();

    void OnEnable() {
        print("Test");

        foreach (GameObject item in rows) {
            Destroy(item);
        }
        rows.Clear();

        var scores = ScoreManager.Instance.GetHighScores().ToArray();
        for (int i = 0; i < scores.Length; i++) {
            var row = Instantiate(rowUi, transform).GetComponent<RowUi>();
            rows.Add(row.gameObject);
            row.rank.text = (i + 1).ToString();
            row.playerName.text = scores[i].name;
            row.score.text = scores[i].score.ToString();
        }
    }
}