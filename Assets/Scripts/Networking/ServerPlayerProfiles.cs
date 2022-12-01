using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
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

        GetLeaderboardProfiles();
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
    public void DeleteUserProfile() {
        StartCoroutine(DeleteUserProfileIEnum());
    }
    public void ChangePassword(string password, string newPassword) {
        StartCoroutine(ChangePasswordIEnum(password, newPassword));
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
            GameStates.SetLoggedStatus(true);
            Trace.Log("Login successful!");
            StartCoroutine(GetAndSetUserDataIEnum(UInt64.Parse(LocalPlayer.Instance.playerId), username));
        }
        else {
            GameStates.SetLoginStatus("Log in failed.");
            Trace.LogWarning("Error logging in!");
        }

        req.Dispose();
    }
    IEnumerator GetAndSetUserDataIEnum(ulong userId, string username) {
        string url = serverJSON["baseUrl"] + "/MultiplayerSession";
        if (url.Equals(serverJSON["baseUrl"])) { Trace.LogWarning("Full URL not set!"); yield break; }

        UnityWebRequest req = UnityWebRequest.Get($"{url}");

        yield return req.SendWebRequest();

        if (WasRequestSuccesful(req)) {

            JSONNode json = JSONNode.Parse(req.downloadHandler.text);
            Trace.Log(json);

            List<ulong> playedGamesIds = new List<ulong>();
            ulong bestScore = 0;
            string email = "";

            foreach (var item in json) {
                ulong playerId = UInt64.Parse(item.Value["playerId"]);
                if (playerId == userId) {
                    ulong gameScore = UInt64.Parse(item.Value["score"]);
                    if (gameScore > bestScore)
                        bestScore = gameScore;
                    playedGamesIds.Add(UInt64.Parse(item.Value["multiplayerGameId"]));
                }
            }
            Trace.Log("bestScore: " + bestScore);

            List<ulong> gameWinnerIds = new List<ulong>();
            ulong loggedUserWins = 0;

            foreach (ulong gameId in playedGamesIds) {
                ulong gameBestScore = 0;
                ulong gameWinnerId = 0;
                Trace.Log(gameId.ToString());
                foreach (var item in json) {
                    if (item.Value["multiplayerGameId"] == gameId) {
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
                loggedUserWins,
                email
            ));

            Trace.Log("loggedUserWins: " + loggedUserWins);
        }

        req.Dispose();
    }
    IEnumerator RegisterUserIEnum(string username, string password) {
        GameStates.SetRegisterStatus("Registering...");

        string url = serverJSON["baseUrl"] + "/Player";
        if (url.Equals(serverJSON["baseUrl"])) {
            Trace.LogWarning("URL not set!");
            Trace.LogWarning("Creating test user.");
            LocalPlayer.Instance.SetLocalPlayerProfile(new PlayerProfile(username, 0, 0, "test"));
            yield break;
        }

        byte[] passwordBytes = HashPassword.Hash(password);

        JSONNode jsonNode = new JSONObject();

        WWWForm form = new WWWForm();

        // Use these to send a hashed password to server later in development:
        // form.AddField("username", username);
        // form.AddBinaryData("password", passwordBytes);

        jsonNode.Add("email", System.Guid.NewGuid().ToString());
        jsonNode.Add("userName", username);
        jsonNode.Add("password", password);

        UnityWebRequest req = UnityWebRequest.Put($"{url}", jsonNode.ToString());
        req.SetRequestHeader("Content-Type", "application/json");
        req.method = "POST";

        yield return req.SendWebRequest();
        if (WasRequestSuccesful(req)) {
            GameStates.SetRegisterStatus("Register successful!");
            StartCoroutine(LogInIEnum(username, password));
        }
        else {
            GameStates.SetRegisterStatus("Register failed!");
            Trace.LogWarning("Register failed!");
        }

        req.Dispose();
    }
    IEnumerator DeleteUserProfileIEnum() {
        NameValueCollection queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);
        queryString.Add("id", LocalPlayer.Instance.playerId);

        string url = serverJSON["baseUrl"] + "/Player?" + queryString.ToString();
        if (url.Equals(serverJSON["baseUrl"])) { Trace.LogWarning("Full URL not set!"); yield break; }

        UnityWebRequest req = UnityWebRequest.Delete($"{url}");
        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success) {
            Trace.LogWarning(req.error);
            Trace.LogError("Error deleting profile!");
        }
        else {
            Trace.Log("Profile deletion successful!");
            LocalPlayer.Instance.LogOut();
        }

        // Update leaderboard
        ScoreManager.Instance.ClearScores();
        ServerPlayerProfiles.Instance.GetLeaderboardProfiles();

        req.Dispose();
    }
    IEnumerator ChangePasswordIEnum(string password, string newPassword) {
        // Test login
        string url = serverJSON["baseUrl"] + "/Player/Login";
        if (url.Equals(serverJSON["baseUrl"])) {
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

        string username = LocalPlayer.Instance.profile.username;

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
            GameStates.SetLoggedStatus(true);
            Trace.Log("Login successful!");

            string urlGetEmail = serverJSON["baseUrl"] + "/Player/" + req.downloadHandler.text;
            if (urlGetEmail.Equals(serverJSON["baseUrl"])) {
                Trace.LogWarning("Full URL not set!"); yield break;
            }
            UnityWebRequest reqGetEmail = UnityWebRequest.Get($"{urlGetEmail}");
            yield return reqGetEmail.SendWebRequest();
            if (WasRequestSuccesful(reqGetEmail)) {
                JSONNode json = JSONNode.Parse(reqGetEmail.downloadHandler.text);

                string urlChangePass = serverJSON["baseUrl"] + "/Player"; ;
                if (urlChangePass.Equals(serverJSON["baseUrl"])) { Trace.LogWarning("Full URL not set!"); yield break; }

                // Use these to send a hashed password to server later in development
                // byte[] passwordBytes = HashPassword.Hash(password);
                // byte[] newPasswordBytes = HashPassword.Hash(newPassword);

                jsonNode = new JSONObject();
                jsonNode.Add("id", req.downloadHandler.text);
                jsonNode.Add("email", json["email"]);
                jsonNode.Add("name", username);
                jsonNode.Add("password", newPassword);

                UnityWebRequest reqChangePass = UnityWebRequest.Put($"{urlChangePass}", jsonNode.ToString());
                reqChangePass.SetRequestHeader("Content-Type", "application/json");

                yield return reqChangePass.SendWebRequest();
                if (WasRequestSuccesful(reqChangePass)) {
                    // Close change password menu
                    ChangePasswordButton.Instance.changePasswordGO.SetActive(false);
                    ChangePasswordButton.Instance.userSettingsGO.SetActive(true);
                }

                reqChangePass.Dispose();
            }
            reqGetEmail.Dispose();
        }
        else {
            GameStates.SetLoginStatus("Log in failed.");
            Trace.LogWarning("Error logging in!");
        }

        req.Dispose();
    }
    IEnumerator GetLeaderboardProfilesIEnum() {
        string url = serverJSON["baseUrl"] + "/MultiplayerSession";
        if (url.Equals(serverJSON["baseUrl"])) { Trace.LogWarning("Full URL not set!"); yield break; }
        UnityWebRequest reqMPSessions = UnityWebRequest.Get($"{url}");
        yield return reqMPSessions.SendWebRequest();

        if (WasRequestSuccesful(reqMPSessions)) {

            string urlPlayers = serverJSON["baseUrl"] + "/Player";
            if (urlPlayers.Equals(serverJSON["baseUrl"])) { Trace.LogWarning("Full URL not set!"); yield break; }
            UnityWebRequest reqPlayers = UnityWebRequest.Get($"{urlPlayers}");
            yield return reqPlayers.SendWebRequest();

            if (WasRequestSuccesful(reqPlayers)) {

                JSONNode jsonMPSessions = JSONNode.Parse(reqMPSessions.downloadHandler.text);
                JSONNode jsonPlayers = JSONNode.Parse(reqPlayers.downloadHandler.text);
                Trace.Log(reqMPSessions.downloadHandler.text);
                Trace.Log(reqPlayers.downloadHandler.text);

                Dictionary<ulong, string> playersNames = new Dictionary<ulong, string>();
                Dictionary<ulong, ulong> playersRecords = new Dictionary<ulong, ulong>();

                foreach (var item in jsonPlayers) {
                    playersNames.Add(UInt64.Parse(item.Value["id"]), item.Value["name"]);
                }

                foreach (var item in jsonMPSessions) {
                    ulong playerId = UInt64.Parse(item.Value["playerId"]);
                    ulong gameScore = UInt64.Parse(item.Value["score"]);

                    if (playersRecords.ContainsKey(playerId)) {
                        if (gameScore > playersRecords[playerId])
                            playersRecords[playerId] = gameScore;
                    }
                    else {
                        playersRecords.Add(playerId, gameScore);
                    }
                    // if (!UInt64.TryParse(item.Value["score"], out highscore))
                    //     Trace.LogError("Error parsing score data!");
                }

                foreach (var item in playersRecords) {
                    ScoreManager.Instance.AddScore(
                        new Score(playersNames[item.Key], item.Value));
                }
                reqPlayers.Dispose();
            }
        }
        // WasRequestSuccesful(reqMPSessions);

        reqMPSessions.Dispose();
    }
    bool SetPlayerId(string responseText) {
        ulong playerId;
        if (UInt64.TryParse(responseText, out playerId)) {
            LocalPlayer.Instance.playerId = playerId.ToString();
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
}
