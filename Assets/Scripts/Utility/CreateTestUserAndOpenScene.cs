using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

public class CreateTestUserAndOpenScene : MonoBehaviour {
    [SerializeField] string sceneToLoad;
    void Start() {
        LocalPlayer.Instance.SetLocalPlayerProfile(new PlayerProfile("TestUser", 0, 0, ""));
        SceneManager.LoadScene(sceneToLoad);
    }
}
