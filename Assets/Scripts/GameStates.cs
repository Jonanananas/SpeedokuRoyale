using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class GameStates : MonoBehaviour {
    public static bool isOnlineMode { get; private set; }

    public static void SetOnlineMode(bool setOnlineModeTo) {
        isOnlineMode = setOnlineModeTo;
    }
}
