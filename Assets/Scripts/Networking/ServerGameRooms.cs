using System.Collections;
using System.Collections.Specialized;
using System.IO;

using UnityEngine;
using UnityEngine.Networking;

using SimpleJSON;

public class ServerGameRooms : MonoBehaviour {
    public static ServerGameRooms Instance;
    string baseServerURL = ServerSettings.baseURL;
    string currentRoomName, inGameStatus;
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
    }
    public void UpdateCurrentScore(string userName, ulong score) {
        StartCoroutine(UpdateCurrentScoreIEnum(userName, score));
    }
    public void AddScore(ulong score) {
        StartCoroutine(AddScoreIEnum(score));
    }
    public void GetAvailableGameRooms() {
        StartCoroutine(GetAvailableGameRoomsIEnum());
    }
    public void JoinGameRoom(string roomName) {
        StartCoroutine(JoinGameRoomIEnum(roomName));
    }
    public void GetGameRoomStatus(string roomName) {
        StartCoroutine(GetGameRoomStatusIEnum(roomName));
    }
    public void DropLastPlayer() {
        JSONNode json = JSONNode.Parse(inGameStatus);

        int numberOfPlayers = json["players"].Count;
        ManageGameSession.Instance.SetPlacingText(numberOfPlayers);
        if (numberOfPlayers <= 1) return;

        // Drop a player with lowest score
        ulong lowestScore = 0;
        if (System.UInt64.TryParse(json["players"][0]["score"].Value, out lowestScore)) {
            Trace.Log("Score parse successful!");
        }
        else {
            Trace.LogError("Score parse failed!");
        }

        foreach (var player in json["players"]) {
            ulong playerScore;
            System.UInt64.TryParse(player.Value["score"], out playerScore);
            if (playerScore < lowestScore) {
                lowestScore = playerScore;
            }
        }
        if (lowestScore == LocalPlayer.Instance.GetScore()) {
            Timer.Instance.StopTimer();
            ManageGameSession.Instance.LoseGame();
            StartCoroutine(DropLastPlayerIEnum());
        }
    }
    bool gameStarted, gameComplete;
    IEnumerator UpdateCurrentScoreIEnum(string username, ulong score) {
        string url = baseServerURL;
        if (url.Equals(baseServerURL)) { Trace.LogWarning("Full URL not set!"); yield break; }

        JSONObject json = new JSONObject();
        json.Add("currentScore", score);

        UnityWebRequest req = UnityWebRequest.Put($"{url}?username={username}", json);
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();
        WasRequestSuccesful(req);

        req.Dispose();
    }
    IEnumerator AddScoreIEnum(ulong score) {
        NameValueCollection queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);

        queryString.Add("playerId", LocalPlayer.Instance.playerId);
        queryString.Add("scores", score.ToString());

        string url = baseServerURL + "/MultiplayerRuntime/" + currentRoomName + "/AddScore?" + queryString.ToString();
        if (url.Equals(baseServerURL)) { Trace.LogWarning("Full URL not set!"); yield break; }

        UnityWebRequest req = UnityWebRequest.Post($"{url}", "");

        yield return req.SendWebRequest();
        WasRequestSuccesful(req);

        req.Dispose();
    }
    IEnumerator GetAvailableGameRoomsIEnum() {
        string url = baseServerURL + "/MultiplayerRuntime/AvaliableRooms";
        if (url.Equals(baseServerURL)) {
            StartGameButton.Instance.StartGame();
            Trace.LogWarning("Full URL not set!"); yield break;
        }

        UnityWebRequest req = UnityWebRequest.Get(url);

        yield return req.SendWebRequest();

        JSONNode json = JSONNode.Parse(req.downloadHandler.text);

        if (WasRequestSuccesful(req)) {
            StartCoroutine(JoinGameRoomIEnum(json[0]["name"]));
        }

        req.Dispose();
    }
    IEnumerator JoinGameRoomIEnum(string roomName) {
        Trace.Log("roomName:" + roomName);
        string url = baseServerURL + "/MultiplayerRuntime/" + roomName + "/Join?playerId=" + LocalPlayer.Instance.playerId;
        if (url.Equals(baseServerURL)) { Trace.LogWarning("Full URL not set!"); yield break; }

        UnityWebRequest req = UnityWebRequest.Post($"{url}", "");

        yield return req.SendWebRequest();
        if (WasRequestSuccesful(req)) {
            Trace.Log(req.downloadHandler.text);
            currentRoomName = roomName;
            StartCoroutine(GetGameRoomStatusIEnum(roomName));
        }
        else {
            currentRoomName = "";
        }
        req.Dispose();
    }
    IEnumerator GetGameRoomStatusIEnum(string roomName) {
        string statusCodeURL = baseServerURL + "/MultiplayerRuntime/" + roomName + "/Status";
        string statusURL = baseServerURL + "/MultiplayerRuntime/" + roomName + "/InGameStatus";
        if (statusCodeURL.Equals(baseServerURL)) {
            StartGameButton.Instance.StartGame();
            Trace.LogWarning("Full URL not set!"); yield break;
        }
        gameComplete = false;
        gameStarted = false;

        UnityWebRequest statusCodeReq = new UnityWebRequest();
        UnityWebRequest statusReq = new UnityWebRequest();

        while (!gameComplete) {
            statusCodeReq = UnityWebRequest.Get($"{statusCodeURL}");
            statusReq = UnityWebRequest.Get($"{statusURL}");

            yield return statusCodeReq.SendWebRequest();

            if (WasRequestSuccesful(statusCodeReq)) {
                Trace.Log("game room status code: " + statusCodeReq.downloadHandler.text);
                string roomStatus = statusCodeReq.downloadHandler.text;
                if (roomStatus == "1" && !gameStarted) {
                    StartGameButton.Instance.StartGame();
                    Timer.Instance.StartCheckingToDropPlayers();
                    gameStarted = true;
                }
                if (roomStatus == "1" && gameStarted) {
                    yield return statusReq.SendWebRequest();
                    if (WasRequestSuccesful(statusReq)) {
                        Trace.Log("game room status: " + statusReq.downloadHandler.text);
                        inGameStatus = statusReq.downloadHandler.text;
                    }
                }
                else if (roomStatus == "3") {
                    gameComplete = true;
                }
            }
            else {
                Trace.LogError("Error starting game!");
                break;
            }
            yield return new WaitForSeconds(1);
        }

        // Update leaderboard
        ScoreManager.Instance.ClearScores();
        ServerPlayerProfiles.Instance.GetLeaderboardProfiles();

        statusCodeReq.Dispose();
        statusReq.Dispose();
    }
    IEnumerator DropLastPlayerIEnum() {
        NameValueCollection queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);
        queryString.Add("playerId", LocalPlayer.Instance.playerId);
        string url = baseServerURL + "/MultiplayerRuntime/" + currentRoomName + "/Kill?" + queryString.ToString();

        if (url.Equals(baseServerURL)) {
            Trace.LogWarning("Full URL not set!"); yield break;
        }

        UnityWebRequest req = UnityWebRequest.Post(url, "");

        yield return req.SendWebRequest();

        WasRequestSuccesful(req);

        if (req.downloadHandler.text == "true") {
            Trace.Log("Local player eliminated from game room " + currentRoomName);
        }
        else {
            Trace.LogWarning("Player elimination failed");
        }

        req.Dispose();
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
