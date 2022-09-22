using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

public class ManageScenes : MonoBehaviour {
    public void LoadMainMenu() {
        SceneManager.LoadScene("Menu");
    }
}
