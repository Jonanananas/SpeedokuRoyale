using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerProfile {
    public string username, password;
    public int bestScore, victories;

    public PlayerProfile(string username, string password, int bestScore, int victories) {
        this.username = username;
        this.password = password;
        this.bestScore = bestScore;
        this.victories = victories;
    }
}
