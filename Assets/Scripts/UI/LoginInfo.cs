using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using TMPro;

/// <summary>
/// Display info about the login status while logging in with a web request.
/// </summary>
public class LoginInfo : MonoBehaviour {
    public static LoginInfo Instance;
    void Awake() {
        #region Singleton
        if (Instance != null) {
            Destroy(gameObject);
            Trace.LogError("More than one instance of " + this.GetType().Name + " found!");
            return;
        }
        Instance = this;
        #endregion
    }
    [SerializeField] TextMeshProUGUI infoTMP;
    [SerializeField] GameObject backButton, mainMenuButton;
    public void SetInfoText(string text) {
        infoTMP.text = text;
    }
    public void ToggleBackButton(bool toggle) {
        backButton.SetActive(toggle);
    }
    public void ToggleMainMenuButton(bool toggle) {
        mainMenuButton.SetActive(toggle);
    }
    bool logginIn;
    void OnEnable() {
        logginIn = true;
    }
    void OnDisable() {
        logginIn = false;
    }
    /// <summary>
    /// Update UI with the current login status while logging in.
    /// </summary>
    void Update() {
        if (!logginIn) return;

        SetInfoText(GameStates.loginStatus);

        if (GameStates.loginStatus.Equals("Log in successful!")) {
            ToggleMainMenuButton(true);
            SetInfoText(GameStates.loginStatus);
        }
        else if (GameStates.loginStatus.Equals("Log in failed.")) {
            SetInfoText(GameStates.loginStatus);
            ToggleBackButton(true);
        }
    }
}
