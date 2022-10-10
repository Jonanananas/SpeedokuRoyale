using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using SimpleJSON;
public class TestScriptJonathan : MonoBehaviour {
    // Start is called before the first frame update
    void Start() {
        // HashPassword.Hash("passssssswww");
        JSONNode json = new JSONObject();
        JSONNode profile = new JSONObject();
        profile.Add("name", "test1");
        profile.Add("score", "123124");
        JSONNode profile2 = new JSONObject();
        profile2.Add("name", "test2");
        profile2.Add("score", "2325235");
        JSONNode profile3 = new JSONObject();
        profile3.Add("name", "test3");
        profile3.Add("score", "3254286");
        json.Add(profile);
        json.Add(profile2);
        json.Add(profile3);

        Dictionary<string, ulong> bestScores = new Dictionary<string, ulong>();
        foreach (var profileJson in json) {
            ulong highscore;
            string score = profileJson.Value["score"];
            string name = profileJson.Value["name"];
            System.UInt64.TryParse(score, out highscore);
            bestScores.Add(name, highscore);
        }

        foreach (var profileTest in bestScores) {
            print(profileTest);
        }
    }

    // Update is called once per frame
    void Update() {
        // Timer.Instance.StartTimer();
        // Trace.Log(Timer.Instance.text);
    }
}
