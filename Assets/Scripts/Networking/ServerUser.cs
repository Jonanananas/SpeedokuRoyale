using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using System;
using System.IO;
public class ServerUser : MonoBehaviour {
    public static ServerUser Instance;
    bool gameStarted;
    JSONNode serverJSON = new JSONObject();
    void Awake() {
        string serverSettingsPath = Application.dataPath + "/server-settings.json";

        if (File.Exists(serverSettingsPath)) {
            string fileContent = File.ReadAllText(serverSettingsPath);
            serverJSON = JSONNode.Parse(fileContent);
        }
        else {
            File.Create(serverSettingsPath).Dispose();
            serverJSON.Add("baseUrl", "http://127.0.0.1:8000/");
            string fileContent = serverJSON.ToString();
            File.WriteAllText(serverSettingsPath, fileContent);
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
    #region Profile methods
    public IEnumerator LogIn(string username, string password) {

        Trace.Log("username: " + username + "password: " + password);

        // #region Test code without server connection
        // GameStates.SetLoggedStatus(true);
        // LoginButton.Instance.CloseLoginMenu();
        // #endregion

        string url = serverJSON["baseUrl"] + "Player/Login";
        if (url.Equals(serverJSON["baseUrl"])) { Trace.LogWarning("Full URL not set!"); yield break; }

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

        if (WasRequestSuccesful(req)) {
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
        }
        else {
            GameStates.SetLoginStatus("Log in failed.");
            Trace.LogError("Error logging in!");
        }

        req.Dispose();
    }
    public IEnumerator LogOut(string username) {
        string url = serverJSON["baseUrl"];

        LogoutOrLoginButton.Instance.UpdateButtonText("Logging in...");
        #region Test code without server connection
        LocalPlayer.Instance.LogOut();
        LogoutOrLoginButton.Instance.UpdateButtonText();
        #endregion

        if (url.Equals(serverJSON["baseUrl"])) { Trace.LogWarning("Full URL not set!"); yield break; }

        UnityWebRequest req = new UnityWebRequest($"{url}?username={username}", "PUT");
        req.downloadHandler = new DownloadHandlerBuffer();

        yield return req.SendWebRequest();
        if (WasRequestSuccesful(req)) {
            LocalPlayer.Instance.LogOut();
            LogoutOrLoginButton.Instance.UpdateButtonText();
            Trace.Log("Logout successful!");
        }
        else {
            LogoutOrLoginButton.Instance.UpdateButtonText("Error logging out!");
            Trace.LogError("Error logging out!");
        }

        req.Dispose();
    }
    public IEnumerator RegisterUser(string username, string password) {
        GameStates.SetRegisterStatus("Registering...");

        string url = serverJSON["baseUrl"] + "Player";
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
            // JSONNode json = JSONNode.Parse(req.downloadHandler.text);
            int playerId;
            if (Int32.TryParse(req.downloadHandler.text, out playerId)) {
                PlayerPrefs.SetInt("playerId", playerId);
                print(PlayerPrefs.GetInt("playerId"));
                GameStates.SetRegisterStatus("Register successful!");
                StartCoroutine(LogIn(username, password));
            }
            else {
                Trace.LogWarning("Error parsing user id!");
                GameStates.SetRegisterStatus("Register failed!");
            }
        }
        else {
            GameStates.SetRegisterStatus("Register failed!");
            Trace.LogWarning("Register failed!");
        }

        req.Dispose();
    }
    public IEnumerator DeleteUserProfile(string username, string password) {
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
    public IEnumerator UpdateBestScore(string username, ulong score) {
        string url = serverJSON["baseUrl"];
        if (url.Equals(serverJSON["baseUrl"])) { Trace.LogWarning("Full URL not set!"); yield break; }

        JSONObject json = new JSONObject();
        json.Add("bestScore", score);

        UnityWebRequest req = UnityWebRequest.Put($"{url}?username={username}", json);
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();
        WasRequestSuccesful(req);

        req.Dispose();
    }
    public IEnumerator UpdateVictories(string username, ulong victories) {
        string url = serverJSON["baseUrl"];
        if (url.Equals(serverJSON["baseUrl"])) { Trace.LogWarning("Full URL not set!"); yield break; }

        JSONObject json = new JSONObject();
        json.Add("victories", victories);

        UnityWebRequest req = UnityWebRequest.Put($"{url}?username={username}", json);
        req.SetRequestHeader("Content-Type", "application/json");
        // req.method = "PATCH";

        yield return req.SendWebRequest();
        WasRequestSuccesful(req);

        req.Dispose();
    }
    public IEnumerator UpdateCurrentScore(string username, ulong score) {
        string url = serverJSON["baseUrl"];
        if (url.Equals(serverJSON["baseUrl"])) { Trace.LogWarning("Full URL not set!"); yield break; }

        JSONObject json = new JSONObject();
        json.Add("currentScore", score);

        UnityWebRequest req = UnityWebRequest.Put($"{url}?username={username}", json);
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();
        WasRequestSuccesful(req);

        req.Dispose();
    }
    public IEnumerator ChangePassword(string username, string password, string newPassword) {
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
    public IEnumerator GetLeaderboardProfiles() {
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
    #endregion

    #region Game room
    public IEnumerator JoinGameRoom() {
        string url = serverJSON["baseUrl"] + "testRoom/Join";
        if (url.Equals(serverJSON["baseUrl"])) { Trace.LogWarning("Full URL not set!"); yield break; }

        WWWForm form = new WWWForm();
        form.AddField("userId", PlayerPrefs.GetString("playerId"));
        form.AddField("userId", "testRoom");

        UnityWebRequest req = UnityWebRequest.Post($"{url}", form);

        yield return req.SendWebRequest();
        WasRequestSuccesful(req);

        req.Dispose();
    }
    public IEnumerator GetGameRoomStatus() {
        string url = serverJSON["baseUrl"] + "testRoom/Status";
        if (url.Equals(serverJSON["baseUrl"])) { Trace.LogWarning("Full URL not set!"); yield break; }

        UnityWebRequest req = new UnityWebRequest();

        while (!gameStarted) {
            req = UnityWebRequest.Get($"{url}");

            yield return req.SendWebRequest();

            if (WasRequestSuccesful(req)) {

                JSONNode json = JSONNode.Parse(req.downloadHandler.text);

                if (json["gameRoomStatus"] == "startGame") gameStarted = true;
            }
            else {
                Trace.LogError("Error starting game!");
                break;
            }
        }

        StartCoroutine(JoinGameRoom());

        req.Dispose();
    }
    #endregion
    bool WasRequestSuccesful(UnityWebRequest req) {
        if (req.result != UnityWebRequest.Result.Success) {
            Trace.LogError(req.error);
            Trace.LogError(req.downloadHandler.text);
            return false;
        }
        else {
            Trace.Log("Request complete: " + req.downloadHandler.text);
            return true;
        }
    }

    // public class ForceAcceptAll : CertificateHandler {
    //     protected override bool ValidateCertificate(byte[] certificateData) {
    //         return true;
    //     }
    // }
}
