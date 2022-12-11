using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public static class GameStates {
    public static bool isOnlineMode { get; private set; }
    public static bool isLoggedIn { get; private set; }
    public static string registerStatus { get; private set; } = "";
    public static string loginStatus { get; private set; } = "";
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
