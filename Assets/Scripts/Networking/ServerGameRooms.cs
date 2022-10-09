using System.Collections;
using System.Collections.Specialized;
using System.IO;

using UnityEngine;
using UnityEngine.Networking;

using SimpleJSON;

public class ServerGameRooms : MonoBehaviour {
    public static ServerGameRooms Instance;
    JSONNode serverJSON = new JSONObject();
    string currentRoomName;

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
    bool gameStarted, gameComplete;
    IEnumerator UpdateCurrentScoreIEnum(string username, ulong score) {
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
    IEnumerator AddScoreIEnum(ulong score) {
        NameValueCollection queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);

        queryString.Add("playerId", PlayerPrefs.GetString("playerId"));
        queryString.Add("scores", score.ToString());

        string url = serverJSON["baseUrl"] + "/MultiplayerRuntime/" + currentRoomName + "/AddScore?" + queryString.ToString();
        if (url.Equals(serverJSON["baseUrl"])) { Trace.LogWarning("Full URL not set!"); yield break; }

        UnityWebRequest req = UnityWebRequest.Post($"{url}", "");

        yield return req.SendWebRequest();
        WasRequestSuccesful(req);

        req.Dispose();
    }
    IEnumerator GetAvailableGameRoomsIEnum() {
        string url = serverJSON["baseUrl"] + "/MultiplayerRuntime/AvaliableRooms";
        if (url.Equals(serverJSON["baseUrl"])) {
            StartGameButton.Instance.StartGame();
            Trace.LogWarning("Full URL not set!"); yield break;
        }

        UnityWebRequest req = UnityWebRequest.Get(url);

        yield return req.SendWebRequest();

        JSONNode json = JSONNode.Parse(req.downloadHandler.text);

        // print(json);
        // print(json[0]["name"]);

        // StartCoroutine(JoinGameRoom());

        if (WasRequestSuccesful(req)) {
            StartCoroutine(JoinGameRoomIEnum(json[0]["name"]));
        }

        req.Dispose();
    }
    IEnumerator JoinGameRoomIEnum(string roomName) {
        print("roomName:" + roomName);
        string url = serverJSON["baseUrl"] + "/MultiplayerRuntime/" + roomName + "/Join?playerId=" + PlayerPrefs.GetString("playerId");
        if (url.Equals(serverJSON["baseUrl"])) { Trace.LogWarning("Full URL not set!"); yield break; }

        UnityWebRequest req = UnityWebRequest.Post($"{url}", "");

        yield return req.SendWebRequest();
        if (WasRequestSuccesful(req)) {
            print(req.downloadHandler.text);
            currentRoomName = roomName;
            StartCoroutine(GetGameRoomStatusIEnum(roomName));
        }
        else {
            currentRoomName = "";
        }

        req.Dispose();
    }
    IEnumerator GetGameRoomStatusIEnum(string roomName) {
        string url = serverJSON["baseUrl"] + "/MultiplayerRuntime/" + roomName + "/Status";
        if (url.Equals(serverJSON["baseUrl"])) {
            StartGameButton.Instance.StartGame();
            Trace.LogWarning("Full URL not set!"); yield break;
        }

        UnityWebRequest req = new UnityWebRequest();

        while (!gameComplete) {
            req = UnityWebRequest.Get($"{url}");

            yield return req.SendWebRequest();

            if (WasRequestSuccesful(req)) {

                // JSONNode json = JSONNode.Parse(req.downloadHandler.text);

                // if (json["gameRoomStatus"] == "startGame") gameStarted = true;
                print("game room status: " + req.downloadHandler.text);
                string roomStatus = req.downloadHandler.text;
                if (roomStatus == "1" && !gameStarted) {
                    StartGameButton.Instance.StartGame();
                    gameStarted = true;
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

    // public class ForceAcceptAll : CertificateHandler {
    //     protected override bool ValidateCertificate(byte[] certificateData) {
    //         return true;
    //     }
    // }
}
