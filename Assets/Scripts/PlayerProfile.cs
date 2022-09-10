using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerProfile {
    public string username, password;
    public ulong bestScore, victories;

    public PlayerProfile(string username, string password, ulong bestScore, ulong victories) {
        this.username = username;
        this.password = password;
        this.bestScore = bestScore;
        this.victories = victories;
    }
}
