using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerProfile {
    public string username;
    public ulong bestScore, victories;

    public PlayerProfile(string username, ulong bestScore, ulong victories) {
        this.username = username;
        this.bestScore = bestScore;
        this.victories = victories;
    }
}
