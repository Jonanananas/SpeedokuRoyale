using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{

    public void PlaySolo()
    {
        GameStates.SetOnlineMode(false);
        SceneManager.LoadScene("Multiplayer");
    }

    public void PlayOnline()
    {
        GameStates.SetOnlineMode(true);
        SceneManager.LoadScene("Multiplayer");
    }

    public void Quit()
    {
        Debug.Log("Quit");
        Application.Quit();
    }

}
