using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class LocalPlayer : MonoBehaviour {
    public static LocalPlayer Instance;
    public PlayerProfile profile { get; private set; }
    public string playerId;
    ulong score;

    void Awake() {
        #region Singleton
        if (Instance != null) {
            Destroy(gameObject);
            // Trace.LogWarning("More than one instance of " + this.GetType().Name + " found!");
            return;
        }
        Instance = this;
        // Don't destroy this gameobject on load
        DontDestroyOnLoad(gameObject);
        #endregion
    }
    public void SetLocalPlayerProfile(PlayerProfile profile) {
        this.profile = profile;
    }
    public void IncrementVictories() {
        if (profile == null) { Trace.LogWarning("Not logged in!"); return; }
        profile.victories++;
    }
    public void UpdateLocalHighScore() {
        if (profile == null) { Trace.LogWarning("Not logged in!"); return; }
        if (score > profile.bestScore) {
            profile.bestScore = score;
        }
    }
    public void AddPoints(ulong pointsToAdd) {
        score += pointsToAdd;
        if (GameStates.isOnlineMode) {
            if (profile == null) { Trace.LogWarning("Not logged in!"); return; }
            ServerGameRooms.Instance.AddScore(pointsToAdd);
        }
        ManageGameSession.Instance.UpdateScore(score);
    }
    public ulong GetScore() {
        return score;
    }
    public void ResetScore() {
        score = 0;
        // if (GameStates.isOnlineMode) {
        //     if (profile == null) { Trace.LogWarning("Not logged in!"); return; }
        //     ServerGameRooms.Instance.UpdateCurrentScore(profile.username, score);
        // }
    }
    public void LogOut() {
        profile = null;
        GameStates.SetLoggedStatus(false);
    }
}
