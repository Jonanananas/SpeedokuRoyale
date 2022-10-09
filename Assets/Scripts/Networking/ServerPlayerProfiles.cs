using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

using UnityEngine;
using UnityEngine.Networking;

using SimpleJSON;

public class ServerPlayerProfiles : MonoBehaviour {
    public static ServerPlayerProfiles Instance;
    JSONNode serverJSON = new JSONObject();

    void Start() {
        string serverSettingsPath = Application.dataPath + "/server-settings.json";

        if (File.Exists(serverSettingsPath)) {
            string fileContent = File.ReadAllText(serverSettingsPath);
            serverJSON = JSONNode.Parse(fileContent);
        }
        else {
            Trace.LogError("File \"server-settings\" is missing!");
        }

        #region Singleton
        if (Instance != null) {
            Destroy(gameObject);
            // Trace.LogWarning("More than one instance of " + this.GetType().Name + " found!");
            return;
        }
        Instance = this;
        // Don't destroy this gameobject on load
        DontDestroyOnLoad(gameObject);
        #endregion
    }
    public void LogIn(string userName, string password) {
        StartCoroutine(LogInIEnum(userName, password));
    }
    public void RegisterUser(string userName, string password) {
        StartCoroutine(RegisterUserIEnum(userName, password));
    }
    public void GetAndSetUserData(ulong userId, string username) {
        StartCoroutine(GetAndSetUserDataIEnum(userId, username));
    }
    public void DeleteUserProfile(string username, string password) {
        StartCoroutine(DeleteUserProfileIEnum(username, password));
    }
    public void ChangePassword(string username, string password, string newPassword) {
        StartCoroutine(ChangePasswordIEnum(username, password, newPassword));
    }
    public void GetLeaderboardProfiles() {
        StartCoroutine(GetLeaderboardProfilesIEnum());
    }

    IEnumerator LogInIEnum(string username, string password) {

        Trace.Log("username: " + username + "password: " + password);

        string url = serverJSON["baseUrl"] + "/Player/Login";
        if (url.Equals(serverJSON["baseUrl"])) {
            // #region Test code without server connection
            // GameStates.SetLoggedStatus(true);
            // LoginButton.Instance.CloseLoginMenu();
            // #endregion
            Trace.LogWarning("Full URL not set!"); yield break;
        }

        // Use these to send a hashed password to server later in development
        // byte[] passwordBytes = HashPassword.Hash(password);
        // WWWForm form = new WWWForm();
        // form.AddField("username", username);
        // form.AddBinaryData("password", passwordBytes);
        // UnityWebRequest req = UnityWebRequest.Put($"{url}", form.data);

        // foreach (var header in form.headers) {
        //     req.SetRequestHeader(header.Key, header.Value);
        // }

        JSONNode jsonNode = new JSONObject();
        jsonNode.Add("userName", username);
        jsonNode.Add("password", password);
        UnityWebRequest req = UnityWebRequest.Put($"{url}", jsonNode.ToString());
        req.SetRequestHeader("Content-Type", "application/json");
        req.method = "POST";

        GameStates.SetLoginStatus("Logging in...");

        yield return req.SendWebRequest();
        if (WasRequestSuccesful(req) && SetPlayerId(req.downloadHandler.text)) {
            GameStates.SetLoginStatus("Log in successful!");

            // JSONNode json = JSONNode.Parse(req.downloadHandler.text);

            // ulong bestScore;
            // ulong victories;

            // if (UInt64.TryParse(json["victories"].Value, out victories) &&
            //     UInt64.TryParse(json["bestScore"].Value, out bestScore)
            // ) {
            //     LocalPlayer.Instance.SetLocalPlayerProfile(new PlayerProfile(
            //         json["username"].Value,
            //         bestScore,
            //         victories
            //     ));
            GameStates.SetLoggedStatus(true);
            // if (LoginButton.Instance != null)
            //     LoginButton.Instance.CloseLoginMenu();
            Trace.Log("Login successful!");
            // }
            // else {
            //     Trace.LogError("Failed to parse numerical user profile data!");
            // }
            StartCoroutine(GetAndSetUserDataIEnum(UInt64.Parse(PlayerPrefs.GetString("playerId")), username));
        }
        else {
            GameStates.SetLoginStatus("Log in failed.");
            Trace.LogWarning("Error logging in!");
        }

        req.Dispose();
    }
    IEnumerator GetAndSetUserDataIEnum(ulong userId, string username) {
        // string url = serverJSON["baseUrl"] + "/Player/" + userId;
        string url = serverJSON["baseUrl"] + "/MultiplayerSession";
        if (url.Equals(serverJSON["baseUrl"])) { Trace.LogWarning("Full URL not set!"); yield break; }

        UnityWebRequest req = UnityWebRequest.Get($"{url}");

        yield return req.SendWebRequest();

        JSONNode json = JSONNode.Parse(req.downloadHandler.text);

        print(json);
        // print(json[0]["playerId"].Value);

        List<ulong> playedGamesIds = new List<ulong>();
        ulong bestScore = 0;

        foreach (var item in json) {
            ulong playerId = UInt64.Parse(item.Value["playerId"]);
            if (playerId == userId) {
                ulong gameScore = UInt64.Parse(item.Value["score"]);
                if (gameScore > bestScore)
                    bestScore = gameScore;
                playedGamesIds.Add(UInt64.Parse(item.Value["multiplayerGameId"]));
            }
        }
        print("bestScore: " + bestScore);

        List<ulong> gameWinnerIds = new List<ulong>();
        ulong loggedUserWins = 0;

        foreach (ulong gameId in playedGamesIds) {
            ulong gameBestScore = 0;
            ulong gameWinnerId = 0;
            print(gameId);
            foreach (var item in json) {
                if (item.Value["id"] == gameId) {
                    ulong gameScore = UInt64.Parse(item.Value["score"]);
                    if (gameScore > gameBestScore) {
                        gameBestScore = gameScore;
                        gameWinnerId = UInt64.Parse(item.Value["playerId"]);
                    }
                }
            }
            if (gameWinnerId == userId) {
                loggedUserWins++;
            }
        }
        LocalPlayer.Instance.SetLocalPlayerProfile(new PlayerProfile(
            username,
            bestScore,
            loggedUserWins
        ));

        print("loggedUserWins: " + loggedUserWins);

        // Dictionary<string, ulong> bestScores = new Dictionary<string, ulong>();
        // foreach (var profile in json) {
        //     ulong highscore;
        //     if (!UInt64.TryParse(profile.Value["score"], out highscore))
        //         Trace.LogError("Error parsing score data!");
        //     // bestScores.Add(profile.Value["name"], highscore);
        //     ScoreManager.Instance.AddScore(new Score(profile.Value["name"], highscore));
        // }

        // GameData.SetBestScores(bestScores);

        WasRequestSuccesful(req);

        req.Dispose();
    }
    IEnumerator RegisterUserIEnum(string username, string password) {
        GameStates.SetRegisterStatus("Registering...");

        string url = serverJSON["baseUrl"] + "/Player";
        if (url.Equals(serverJSON["baseUrl"])) {
            Trace.LogWarning("URL not set!");
            Trace.LogWarning("Creating test user.");
            LocalPlayer.Instance.SetLocalPlayerProfile(new PlayerProfile(username, 0, 0));
            yield break;
        }

        byte[] passwordBytes = HashPassword.Hash(password);

        JSONNode jsonNode = new JSONObject();

        WWWForm form = new WWWForm();
        // Use these to send a hashed password to server later in development
        // form.AddField("username", username);
        // form.AddBinaryData("password", passwordBytes);

        jsonNode.Add("email", "testEmail");
        jsonNode.Add("userName", username);
        jsonNode.Add("password", password);

        print(jsonNode);

        UnityWebRequest req = UnityWebRequest.Put($"{url}", jsonNode.ToString());
        req.SetRequestHeader("Content-Type", "application/json");
        req.method = "POST";

        // Accept any SSL certificate for now while in local development
        // var cert = new ForceAcceptAll();
        // req.certificateHandler = cert;
        // cert?.Dispose();

        print(req.url);

        yield return req.SendWebRequest();
        if (WasRequestSuccesful(req)) {
            // if (SetPlayerId(req.downloadHandler.text)) {
            GameStates.SetRegisterStatus("Register successful!");
            StartCoroutine(LogInIEnum(username, password));
            // }
            // else {
            //     Trace.LogWarning("Error parsing user id!");
            //     GameStates.SetRegisterStatus("Register failed!");
            // }
        }
        else {
            GameStates.SetRegisterStatus("Register failed!");
            Trace.LogWarning("Register failed!");
        }

        req.Dispose();
    }
    IEnumerator DeleteUserProfileIEnum(string username, string password) {
        string url = serverJSON["baseUrl"];
        if (url.Equals(serverJSON["baseUrl"])) { Trace.LogWarning("Full URL not set!"); yield break; }

        byte[] passwordBytes = HashPassword.Hash(password);

        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddBinaryData("password", passwordBytes);

        UnityWebRequest req = UnityWebRequest.Post($"{url}", form);
        req.method = "DELETE";
        yield return req.SendWebRequest();
        if (WasRequestSuccesful(req)) {
            Trace.Log("Profile deletion successful!");
            LocalPlayer.Instance.LogOut();
        }
        else {
            Trace.LogError("Error deleting profile!");
        }

        req.Dispose();
    }
    IEnumerator ChangePasswordIEnum(string username, string password, string newPassword) {
        string url = serverJSON["baseUrl"];
        if (url.Equals(serverJSON["baseUrl"])) { Trace.LogWarning("Full URL not set!"); yield break; }

        byte[] passwordBytes = HashPassword.Hash(password);
        byte[] newPasswordBytes = HashPassword.Hash(newPassword);

        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddBinaryData("password", passwordBytes);
        form.AddBinaryData("newPassword", newPasswordBytes);

        UnityWebRequest req = UnityWebRequest.Put($"{url}", form.data);

        foreach (var header in form.headers) {
            req.SetRequestHeader(header.Key, header.Value);
        }

        yield return req.SendWebRequest();
        WasRequestSuccesful(req);

        req.Dispose();
    }
    IEnumerator GetLeaderboardProfilesIEnum() {
        string url = serverJSON["baseUrl"];
        if (url.Equals(serverJSON["baseUrl"])) { Trace.LogWarning("Full URL not set!"); yield break; }

        UnityWebRequest req = UnityWebRequest.Get($"{url}");

        yield return req.SendWebRequest();

        JSONNode json = JSONNode.Parse(req.downloadHandler.text);

        // Dictionary<string, ulong> bestScores = new Dictionary<string, ulong>();
        foreach (var profile in json) {
            ulong highscore;
            if (!UInt64.TryParse(profile.Value["score"], out highscore))
                Trace.LogError("Error parsing score data!");
            // bestScores.Add(profile.Value["name"], highscore);
            ScoreManager.Instance.AddScore(new Score(profile.Value["name"], highscore));
        }

        // GameData.SetBestScores(bestScores);

        WasRequestSuccesful(req);

        req.Dispose();
    }
    bool SetPlayerId(string responseText) {
        ulong playerId;
        if (UInt64.TryParse(responseText, out playerId)) {
            PlayerPrefs.SetString("playerId", playerId.ToString());
            // print(PlayerPrefs.GetInt("playerId"));
            return true;
        }
        return false;
    }
    bool WasRequestSuccesful(UnityWebRequest req) {
        if (req.result != UnityWebRequest.Result.Success) {
            Trace.LogWarning(req.error);
            Trace.LogWarning(req.downloadHandler.text);
            return false;
        }
        else {
            Trace.Log("Request complete: " + req.downloadHandler.text);
            return true;
        }
    }
    // public IEnumerator UpdateBestScore(string username, ulong score) {
    //     string url = serverJSON["baseUrl"];
    //     if (url.Equals(serverJSON["baseUrl"])) { Trace.LogWarning("Full URL not set!"); yield break; }

    //     JSONObject json = new JSONObject();
    //     json.Add("bestScore", score);

    //     UnityWebRequest req = UnityWebRequest.Put($"{url}?username={username}", json);
    //     req.SetRequestHeader("Content-Type", "application/json");

    //     yield return req.SendWebRequest();
    //     WasRequestSuccesful(req);

    //     req.Dispose();
    // }
    // public IEnumerator UpdateVictories(string username, ulong victories) {
    //     string url = serverJSON["baseUrl"];
    //     if (url.Equals(serverJSON["baseUrl"])) { Trace.LogWarning("Full URL not set!"); yield break; }

    //     JSONObject json = new JSONObject();
    //     json.Add("victories", victories);

    //     UnityWebRequest req = UnityWebRequest.Put($"{url}?username={username}", json);
    //     req.SetRequestHeader("Content-Type", "application/json");
    //     // req.method = "PATCH";

    //     yield return req.SendWebRequest();
    //     WasRequestSuccesful(req);

    //     req.Dispose();
    // }
    // public class ForceAcceptAll : CertificateHandler {
    //     protected override bool ValidateCertificate(byte[] certificateData) {
    //         return true;
    //     }
    // }
}
