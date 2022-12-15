using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

public class ManageScenes : MonoBehaviour {
    public static ManageScenes Instance;
    void Awake() {
        #region Singleton
        if (Instance != null) {
            Trace.LogWarning("More than one instance of " + this.GetType().Name + " found!");
            return;
        }
        Instance = this;
        #endregion
    }
    public void LoadMainMenu() {
        SceneManager.LoadScene("Menu");
    }
    public void ReloadCurrentScene() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public string GetSceneName() {
        return SceneManager.GetActiveScene().name;
    }
}
