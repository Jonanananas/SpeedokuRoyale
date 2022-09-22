using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class ManageGameSession : MonoBehaviour {
    public static ManageGameSession Instance;
    void Awake() {
        #region Singleton
        if (Instance != null) {
            Trace.LogError("More than one instance of " + this.GetType().Name + " found!");
            return;
        }
        Instance = this;
        #endregion
    }
    public void StartGame() {
        LocalPlayer.Instance.ResetPoints();
        Timer.Instance.StartTimer();
    }
    public void LoseGame() {
        Timer.Instance.StopTimer();
        // TODO: Show placing screen etc.
    }
    public void WinGame() {
        LocalPlayer.Instance.IncrementVictories();
        // TODO: Show victory screen etc.
    }
}
