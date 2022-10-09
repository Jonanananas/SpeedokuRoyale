﻿using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using System;
using System.IO;
public class ServerGameroomReqs : MonoBehaviour {
    public static ServerGameroomReqs Instance;
    bool gameStarted, gameComplete;
    JSONNode serverJSON = new JSONObject();
    void Awake() {
        string serverSettingsPath = Application.dataPath + "/server-settings.json";

        if (File.Exists(serverSettingsPath)) {
            string fileContent = File.ReadAllText(serverSettingsPath);
            serverJSON = JSONNode.Parse(fileContent);
        }
        else {
            File.Create(serverSettingsPath).Dispose();
            serverJSON.Add("baseUrl", "http://127.0.0.1:8000");
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
    public IEnumerator GetAvailableGameRooms() {
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
            StartCoroutine(JoinGameRoom(json[0]["name"]));
        }

        req.Dispose();
    }
    public IEnumerator JoinGameRoom(string roomName) {
        print("roomName:" + roomName);
        string url = serverJSON["baseUrl"] + "/MultiplayerRuntime/" + roomName + "/Join?playerId=" + PlayerPrefs.GetString("playerId");
        if (url.Equals(serverJSON["baseUrl"])) { Trace.LogWarning("Full URL not set!"); yield break; }

        UnityWebRequest req = UnityWebRequest.Post($"{url}", "");

        yield return req.SendWebRequest();
        if (WasRequestSuccesful(req)) {
            print(req.downloadHandler.text);
            StartCoroutine(GetGameRoomStatus(roomName));
        }

        req.Dispose();
    }
    public IEnumerator GetGameRoomStatus(string roomName) {
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
