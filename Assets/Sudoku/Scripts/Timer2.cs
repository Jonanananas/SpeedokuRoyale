using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer2 : MonoBehaviour {
    public static Timer2 Instance;
    public float timeFloat { get; private set; }
    public float endTime { get; private set; }
    public float timeLimit = 300f;
    public TextMeshProUGUI textTMP;
    public bool isRunning { get; private set; }

    public void StartTimer() {
        if (timeFloat > 0) {
            isRunning = true;
            timeFloat = timeLimit;
        }
    }

    public void StopTimer() {
        isRunning = false;
        endTime = timeLimit - timeFloat;
    }

    void Awake() {
        #region Singleton
        if (Instance != null) {
            Trace.LogWarning("More than one instance of " + this.GetType().Name + " found!");
            return;
        }
        Instance = this;
        #endregion
        timeFloat = timeLimit;
        UpdateTimerText();
    }
    
    void Update() {
        if (isRunning) {
            if (timeFloat == 0) {
                ManageGameSession.Instance.EliminateOnePlayer();
                ManageGameSession.Instance.EndGame();
                return;
            }
            timeFloat-= Time.deltaTime;
            if (timeFloat < 0) {
                timeFloat = 0;
            }
            UpdateTimerText();
        }
    }

    void UpdateTimerText() {
        textTMP.text = getStringTime(timeFloat);
    }

    public string getEnd() {
        return getStringTime(endTime);
    }
    

    private string getStringTime(float seconds) {
        float m = Mathf.FloorToInt(seconds / 60),
            s = Mathf.FloorToInt(seconds % 60),
            ms = Mathf.FloorToInt(seconds * 60 % 60);

        string tuloste = "" + m;

        if (s < 10){
            tuloste += ":0" + s;
        } else {
            tuloste += ":" + s;
        }

        if (ms < 10){
            tuloste += ":0" + ms;
        } else {
            tuloste += ":" + ms;
        }

        return tuloste;
    }
}