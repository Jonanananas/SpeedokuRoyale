using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using TMPro;

public class Timer : MonoBehaviour {
    public static Timer Instance;
    public long timeLimit = 30000L;
    public TextMeshProUGUI textTMP;
    // public string text { get; private set; }
    public long timeLong { get; private set; }
    public bool isRunning { get; private set; }
    public void StartTimer() {
        if (isRunning == false) {
            timeLong = timeLimit;
            isRunning = true;
        }
    }
    public void StopTimer() {
        isRunning = false;
    }
    void Awake() {
        #region Singleton
        if (Instance != null) {
            Trace.LogWarning("More than one instance of " + this.GetType().Name + " found!");
            return;
        }
        Instance = this;
        #endregion
    }
    void Start() {
        textTMP.text = "5:00:00";
    }
    void FixedUpdate() {
        if (isRunning) {
            if (timeLong == 0) {
                ManageGameSession.Instance.WinGame();
            }
            timeLong--;
            UpdateTimerText();
        }
    }
    void UpdateTimerText() {
        long minutes = (timeLong / 100L) / 60L;
        long seconds = (timeLong / 100L) - (60L * minutes);
        long fractions = timeLong % 100L;
        textTMP.text = minutes + ":" + seconds + "." + fractions;
    }
}
