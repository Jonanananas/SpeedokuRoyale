using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using TMPro;

public class Timer : MonoBehaviour {
    public static Timer Instance;
    /// <summary>
    /// How long a single game lasts
    /// </summary>
    public long timeLimit = 100000L; // Set in the game scene in the editor
    /// <summary>
    /// The timer's UI text element
    /// </summary>
    public TextMeshProUGUI textTMP;
    /// <summary>
    /// Current game time
    /// </summary>
    public long timeLong { get; private set; }
    /// <summary>
    /// How long a game has lasted
    /// </summary>
    public long elapsedTime { get; private set; }
    /// <summary>
    /// Is the timer running
    /// </summary>
    public bool isRunning { get; private set; }
    bool checkForDrops;
    [SerializeField] long eliminationInterval = 2000L; // Set in the game scene in the editor
    public void StartTimer() {
        if (isRunning == false) {
            timeLong = timeLimit;
            elapsedTime = 0;
            isRunning = true;
        }
    }
    /// <summary>
    /// Start checking if players should be dropped when a multiplayer game session starts.
    /// </summary>
    public void StartCheckingToDropPlayers() {
        checkForDrops = true;
    }
    public void StopTimer() {
        checkForDrops = false;
        isRunning = false;
    }
    public string GetElapsedTime() {
        return GetStringTime(elapsedTime);
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
    /// <summary>
    /// Convert the inserted gametime "<paramref name="time"/>" to a string format.
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    string GetStringTime(long time) {
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
    /// <summary>
    /// Update the timer text in the UI.
    /// </summary>
    void UpdateTimerText() {
        textTMP.text = GetStringTime(timeLong);
    }
}
