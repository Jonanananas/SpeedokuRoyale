using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class LocalPlayer : MonoBehaviour {
    public static LocalPlayer Instance;
    public PlayerProfile profile { get; private set; }

    void Awake() {
        #region Singleton
        if (Instance != null) {
            Debug.LogWarning("More than one instance of " + this.GetType().Name + " found!");
            return;
        }
        Instance = this;
        #endregion
    }
    public void SetLocalPlayerProfile(PlayerProfile profile) {
        this.profile = profile;
    }
}
