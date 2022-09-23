using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class LocalPlayer : MonoBehaviour {
    public static LocalPlayer Instance;
    public PlayerProfile profile { get; private set; }
    ulong points;

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
        if (GameStates.isOnlineMode) {
            ServerUser.Instance.UpdateVictories(profile.username, profile.victories);
        }
    }
    public void AddPoints(ulong pointsToAdd) {
        points += pointsToAdd;
        if (GameStates.isOnlineMode) {
            if (profile == null) { Trace.LogWarning("Not logged in!"); return; }
            ServerUser.Instance.UpdateCurrentScore(profile.username, points);
        }
    }
    public ulong GetPoints() {
        return points;
    }
    public void ResetPoints() {
        points = 0;
        if (GameStates.isOnlineMode) {
            if (profile == null) { Trace.LogWarning("Not logged in!"); return; }
            ServerUser.Instance.UpdateCurrentScore(profile.username, points);
        }
    }
}
