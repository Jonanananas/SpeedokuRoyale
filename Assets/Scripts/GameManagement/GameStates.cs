using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public static class GameStates {
    public static bool isOnlineMode { get; private set; }
    public static bool isLoggedIn { get; private set; }
    public static void SetOnlineMode(bool setOnlineMode) {
        isOnlineMode = setOnlineMode;
    }
    public static void SetLoggedStatus(bool setLoggedStatus) {
        isLoggedIn = setLoggedStatus;
    }
}
