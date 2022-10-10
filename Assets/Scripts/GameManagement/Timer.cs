using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using TMPro;

public class Timer : MonoBehaviour {
    public static Timer Instance;
    public long timeLimit = 100000L; // Set in game scene in editor
    public TextMeshProUGUI textTMP;
    // public string text { get; private set; }
    public long timeLong { get; private set; }
    public long elapsedTime { get; private set; }
    public bool isRunning { get; private set; }
    bool checkForDrops;
    long eliminationInterval = 500L;
    public void StartTimer() {
        if (isRunning == false) {
            timeLong = timeLimit;
            elapsedTime = 0;
            isRunning = true;
        }
    }
    public void StartCheckingToDropPlayers() {
        checkForDrops = true;
    }
    public void StopTimer() {
        checkForDrops = false;
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
        timeLong = timeLimit;
        UpdateTimerText();
    }
    void FixedUpdate() {
        if (isRunning) {
            if (timeLong == 0) {
                ManageGameSession.Instance.EliminateOnePlayer();
                ManageGameSession.Instance.WinGame();
                StopTimer();
                return;
            }
            elapsedTime++;
            timeLong--;
            UpdateTimerText();
            if (checkForDrops && elapsedTime % eliminationInterval == 0) {
                ServerGameRooms.Instance.DropLastPlayer();
            }
        }
    }
    void UpdateTimerText() {
        long minutes = (timeLong / 100L) / 60L;
        long seconds = (timeLong / 100L) - (60L * minutes);
        long fractions = timeLong % 100L;
        textTMP.text = minutes + ":" + seconds + "." + fractions;
    }
}
