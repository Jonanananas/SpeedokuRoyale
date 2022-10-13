using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {

    public void PlaySolo() {
        GameStates.SetOnlineMode(false);
        SceneManager.LoadScene("Singleplayer");
    }
    public void PlayOnline() {
        GameStates.SetOnlineMode(true);
        SceneManager.LoadScene("Multiplayer");
    }
    public void PlayOnlineTest() {
        GameStates.SetOnlineMode(true);
        SceneManager.LoadScene("MultiplayerTest");
    }
    public void Quit() {
        Application.Quit();
    }
}
