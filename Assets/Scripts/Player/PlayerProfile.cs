using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerProfile {
    public string username, email;
    public ulong bestScore, victories;

    public PlayerProfile(string username, ulong bestScore, ulong victories, string email) {
        this.username = username;
        this.email = email;
        this.bestScore = bestScore;
        this.victories = victories;
    }
}
