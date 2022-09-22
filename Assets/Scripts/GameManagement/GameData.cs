using System.Collections;
using System.Collections.Generic;
public static class GameData {
    public static Dictionary<string, ulong> highscoreProfiles { get; private set; }

    public static void SetBestScores(Dictionary<string, ulong> profiles) {
        highscoreProfiles = new Dictionary<string, ulong>(profiles);
    }
}
