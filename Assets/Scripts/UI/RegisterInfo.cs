using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using TMPro;
/// <summary>
/// Display info about the registeration status while registering a user with a web request.
/// </summary>
public class RegisterInfo : MonoBehaviour {
    public static RegisterInfo Instance;
    bool registering;
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
    void OnEnable() {
        registering = true;
    }
    void OnDisable() {
        registering = false;
    }
    /// <summary>
    /// Update UI with the current registeration status while logging in.
    /// </summary>
    void Update() {
        if (!registering) return;

        SetInfoText(GameStates.registerStatus);

        if (GameStates.registerStatus.Equals("Register failed!")) {
            ToggleMainMenuButton(false);
            ToggleBackButton(true);
        }
        if (GameStates.loginStatus.Equals("Log in successful!")) {
            ToggleMainMenuButton(true);
            ToggleBackButton(false);
            SetInfoText(GameStates.loginStatus);
        }
        else if (GameStates.loginStatus.Equals("Log in failed.")) {
            SetInfoText(GameStates.loginStatus);
            ToggleBackButton(true);
            ToggleMainMenuButton(false);
        }
    }
}
