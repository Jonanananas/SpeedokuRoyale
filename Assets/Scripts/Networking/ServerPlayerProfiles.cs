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
    string baseServerURL = ServerSettings.baseURL;
    void Start() {
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
    /// <summary>
    /// Start a coroutine to log a player in on the server.
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    public void LogIn(string username, string password) {
        StartCoroutine(LogInIEnum(username, password));
    }
    /// <summary>
    /// Start a coroutine to register a new player profile on the server.
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    public void RegisterUser(string username, string password) {
        StartCoroutine(RegisterUserIEnum(username, password));
    }
    /// <summary>
    /// Start a coroutine to get user data from the server and set the data locally.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="username"></param>
    public void GetAndSetUserData(ulong userId, string username) {
        StartCoroutine(GetAndSetUserDataIEnum(userId, username));
    }
    /// <summary>
    /// Start a coroutine to delete the local user's profile on the server.
    /// </summary>
    public void DeleteUserProfile() {
        StartCoroutine(DeleteUserProfileIEnum());
    }
    /// <summary>
    /// Start a coroutine to change the local user's password on the server.
    /// </summary>
    /// <param name="password"></param>
    /// <param name="newPassword"></param>
    public void ChangePassword(string password, string newPassword) {
        StartCoroutine(ChangePasswordIEnum(password, newPassword));
    }
    /// <summary>
    /// Start a coroutine to get leaderboard data from the server.
    /// </summary>
    public void GetLeaderboardProfiles() {
        StartCoroutine(GetLeaderboardProfilesIEnum());
    }
    /// <summary>
    /// Log a player in on the server.
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    IEnumerator LogInIEnum(string username, string password) {
        Trace.Log("username: " + username + "password: " + password);

        string url = baseServerURL + "/Player/Login";
        if (url.Equals(baseServerURL)) {
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
    /// <summary>
    /// Get user data from the server and set the data locally.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="username"></param>
    IEnumerator GetAndSetUserDataIEnum(ulong userId, string username) {
        string url = baseServerURL + "/MultiplayerSession";
        if (url.Equals(baseServerURL)) { Trace.LogWarning("Full URL not set!"); yield break; }

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
    /// <summary>
    /// Register a new player profile on the server.
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    IEnumerator RegisterUserIEnum(string username, string password) {
        GameStates.SetRegisterStatus("Registering...");

        string url = baseServerURL + "/Player";
        if (url.Equals(baseServerURL)) {
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
    /// <summary>
    /// Delete the local user's profile on the server.
    /// </summary>
    IEnumerator DeleteUserProfileIEnum() {
        NameValueCollection queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);
        queryString.Add("id", LocalPlayer.Instance.playerId);

        string url = baseServerURL + "/Player?" + queryString.ToString();
        if (url.Equals(baseServerURL)) { Trace.LogWarning("Full URL not set!"); yield break; }

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
    /// <summary>
    /// Change the local user's password on the server.
    /// </summary>
    /// <param name="password"></param>
    /// <param name="newPassword"></param>
    IEnumerator ChangePasswordIEnum(string password, string newPassword) {
        #region Check the login credentials by trying to log in
        string url = baseServerURL + "/Player/Login";
        if (url.Equals(baseServerURL)) {
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
        #endregion

        if (WasRequestSuccesful(req)) {
            GameStates.SetLoginStatus("Log in successful!");
            GameStates.SetLoggedStatus(true);
            Trace.Log("Login successful!");

            #region Retrieve the user's email address
            string urlGetEmail = baseServerURL + "/Player/" + req.downloadHandler.text;
            if (urlGetEmail.Equals(baseServerURL)) {
                Trace.LogWarning("Full URL not set!"); yield break;
            }
            UnityWebRequest reqGetEmail = UnityWebRequest.Get($"{urlGetEmail}");
            yield return reqGetEmail.SendWebRequest();
            #endregion
            
            if (WasRequestSuccesful(reqGetEmail)) {
                JSONNode json = JSONNode.Parse(reqGetEmail.downloadHandler.text);

                #region Change password
                string urlChangePass = baseServerURL + "/Player"; ;
                if (urlChangePass.Equals(baseServerURL)) { Trace.LogWarning("Full URL not set!"); yield break; }

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
                #endregion
            }
            reqGetEmail.Dispose();
        }
        else {
            GameStates.SetLoginStatus("Log in failed.");
            Trace.LogWarning("Error logging in!");
        }

        req.Dispose();
    }
    /// <summary>
    /// Get leaderboard data from the server.
    /// </summary>
    IEnumerator GetLeaderboardProfilesIEnum() {
        // Get all multiplayer sessions from the server
        string url = baseServerURL + "/MultiplayerSession";
        if (url.Equals(baseServerURL)) { Trace.LogWarning("Full URL not set!"); yield break; }
        UnityWebRequest reqMPSessions = UnityWebRequest.Get($"{url}");
        yield return reqMPSessions.SendWebRequest();

        if (WasRequestSuccesful(reqMPSessions)) {
            // Get all players' data from the server
            string urlPlayers = baseServerURL + "/Player";
            if (urlPlayers.Equals(baseServerURL)) { Trace.LogWarning("Full URL not set!"); yield break; }
            UnityWebRequest reqPlayers = UnityWebRequest.Get($"{urlPlayers}");
            yield return reqPlayers.SendWebRequest();

            if (WasRequestSuccesful(reqPlayers)) {
                JSONNode jsonMPSessions = JSONNode.Parse(reqMPSessions.downloadHandler.text);
                JSONNode jsonPlayers = JSONNode.Parse(reqPlayers.downloadHandler.text);
                Trace.Log(reqMPSessions.downloadHandler.text);
                Trace.Log(reqPlayers.downloadHandler.text);

                // Get all the players' names from the server response json data and figure out
                // their best records from occured multiplayer games.

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
                }

                // Add highscores to the leaderboard
                foreach (var item in playersRecords) {
                    ScoreManager.Instance.AddScore(
                        new Score(playersNames[item.Key], item.Value));
                }
                reqPlayers.Dispose();
            }
        }
        reqMPSessions.Dispose();
    }
    /// <summary>
    /// Set the local player's id
    /// </summary>
    /// <param name="responseText"></param>
    /// <returns></returns>
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
