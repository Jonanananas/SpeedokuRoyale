using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using TMPro;

public class Timer : MonoBehaviour {
    public static Timer Instance;
    public long timeLimit = 100000L; // Set in game scene in editor
    public TextMeshProUGUI textTMP;
    public long timeLong { get; private set; }
    public long elapsedTime { get; private set; }
    public bool isRunning { get; private set; }
    bool checkForDrops;
    [SerializeField] long eliminationInterval = 2000L; // Set in game scene in editor
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
                if (GameStates.isOnlineMode)
                    ManageGameSession.Instance.WinGame();
                else
                    ManageGameSession.Instance.EndGame();
                return;
            }
            elapsedTime++;
            timeLong--;
            if (timeLong < 0) {
                timeLong = 0;
            }
            UpdateTimerText();
            if (checkForDrops && elapsedTime % eliminationInterval == 0) {
                ServerGameRooms.Instance.DropLastPlayer();
            }
        }
    }

    public string GetElapsedTime() {
        return GetStringTime(elapsedTime);
    }

    private string GetStringTime(long time) {
        long minutes = (time / 100L) / 60L,
            seconds = (time / 100L) - (60L * minutes),
            fractions = time % 100L;

        string tuloste = "" + minutes;

        if (seconds < 10) {
            tuloste += ":0" + seconds;
        }
        else {
            tuloste += ":" + seconds;
        }

        if (fractions < 10) {
            tuloste += ".0" + fractions;
        }
        else {
            tuloste += "." + fractions;
        }

        return tuloste;
    }
    void UpdateTimerText() {
        textTMP.text = GetStringTime(timeLong);
    }
}
