using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public static class GameStates {
    /// <summary>
    /// Has the player made the selection to play multiplayer or singleplayer.
    /// The value "<c>true</c>" means multiplayer and  "<c>false</c>" singleplayer.
    /// </summary>
    public static bool isOnlineMode { get; private set; }
    /// <summary> Is the player logged in or not.</summary>
    public static bool isLoggedIn { get; private set; }
    public static string registerStatus { get; private set; } = "";
    public static string loginStatus { get; private set; } = "";
    /// <summary> Has localization been initialized.</summary>
    public static bool localeGot = false;
    public static void SetOnlineMode(bool setOnlineMode) {
        isOnlineMode = setOnlineMode;
    }
    public static void SetLoggedStatus(bool setLoggedStatus) {
        isLoggedIn = setLoggedStatus;
    }
    public static void SetRegisterStatus(string setRegisterStatusTo) {
        registerStatus = setRegisterStatusTo;
    }
    public static void SetLoginStatus(string setLoginStatusTo) {
        loginStatus = setLoginStatusTo;
    }
}
