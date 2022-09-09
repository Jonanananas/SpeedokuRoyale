using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerProfile {
    public string username, password, bestScore;

    public PlayerProfile(string username, string password, string bestScore) {
        this.username = username;
        this.password = password;
        this.bestScore = bestScore;
    }
}
