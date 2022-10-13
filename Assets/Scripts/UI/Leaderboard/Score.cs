using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Score {
    public string name;
    public ulong score;
    public Score(string name, ulong score) {
        this.name = name;
        this.score = score;
    }
}
