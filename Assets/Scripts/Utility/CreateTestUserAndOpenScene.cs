using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// A class used to help with testing the game.
/// </summary>
public class CreateTestUserAndOpenScene : MonoBehaviour {
    [SerializeField] string sceneToLoad;
    void Start() {
        LocalPlayer.Instance.SetLocalPlayerProfile(new PlayerProfile("TestUser", 0, 0, ""));
        SceneManager.LoadScene(sceneToLoad);
    }
}
