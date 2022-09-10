using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Timer : MonoBehaviour {
    public static Timer Instance;
    public long timeLimit = 30000L;
    public string text { get; private set; }
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
            Debug.LogWarning("More than one instance of " + this.GetType().Name + " found!");
            return;
        }
        Instance = this;
        #endregion
    }
    void Start() {
        text = "5:00:00";
    }
    void FixedUpdate() {
        if (isRunning) {
            timeLong--;
            UpdateTimerText();
        }
    }
    void UpdateTimerText() {
        long minutes = (timeLong / 100L) / 60L;
        long seconds = (timeLong / 100L) - (60L * minutes);
        long fractions = timeLong % 100L;
        text = minutes + ":" + seconds + "." + fractions;
    }
}
