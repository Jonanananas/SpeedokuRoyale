using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using System;

public class ServerUser : MonoBehaviour {
    public static ServerUser Instance;
    void Awake() {
        #region Singleton
        if (Instance != null) {
            Trace.LogError("More than one instance of " + this.GetType().Name + " found!");
            return;
        }
        Instance = this;
        #endregion
    }
    public IEnumerator LogIn(string username, string password) {

        Trace.Log("username: " + username + "password: " + password);

        string url = "";
        if (url.Equals("")) { Trace.LogWarning("URL not set!"); yield break; }

        byte[] passwordBytes = HashPassword.Hash(password);

        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddBinaryData("password", passwordBytes);

        UnityWebRequest req = UnityWebRequest.Put($"{url}", form.data);

        foreach (var header in form.headers) {
            req.SetRequestHeader(header.Key, header.Value);
        }

        yield return req.SendWebRequest();

        if (WasRequestSuccesful(req)) {
            JSONNode json = JSONNode.Parse(req.downloadHandler.text);

            ulong bestScore;
            ulong victories;

            if (UInt64.TryParse(json["victories"].Value, out victories) &&
                UInt64.TryParse(json["bestScore"].Value, out bestScore)
            ) {
                LocalPlayer.Instance.SetLocalPlayerProfile(new PlayerProfile(
                    json["username"].Value,
                    bestScore,
                    victories
                ));
                Trace.Log("Login successful!");
            }
            else {
                Trace.LogError("Failed to parse numerical user profile data!");
            }
        }
        else {
            Trace.LogError("Error logging in!");
        }

        req.Dispose();
    }
    public IEnumerator LogOut(string username) {
        string url = "";
        if (url.Equals("")) { Trace.LogWarning("URL not set!"); yield break; }

        UnityWebRequest req = new UnityWebRequest($"{url}?username={username}", "PUT");
        req.downloadHandler = new DownloadHandlerBuffer();

        yield return req.SendWebRequest();
        if (WasRequestSuccesful(req)) {
            Trace.Log("Logout successful!");
            // TODO: Log the user out
        }
        else {
            Trace.LogError("Error logging out!");
        }

        req.Dispose();
    }
    public IEnumerator CreateUser(string username, string password) {
        string url = "";
        if (url.Equals("")) { Trace.LogWarning("URL not set!"); yield break; }

        byte[] passwordBytes = HashPassword.Hash(password);

        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddBinaryData("password", passwordBytes);

        UnityWebRequest req = UnityWebRequest.Post($"{url}", form);

        yield return req.SendWebRequest();
        WasRequestSuccesful(req);

        req.Dispose();
    }
    public IEnumerator UpdateBestScore(string username, ulong score) {
        string url = "";
        if (url.Equals("")) { Trace.LogWarning("URL not set!"); yield break; }

        JSONObject json = new JSONObject();
        json.Add("bestScore", score);

        UnityWebRequest req = UnityWebRequest.Put($"{url}?username={username}", json);
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();
        WasRequestSuccesful(req);

        req.Dispose();
    }
    public IEnumerator UpdateVictories(string username, ulong victories) {
        string url = "";
        if (url.Equals("")) { Trace.LogWarning("URL not set!"); yield break; }

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
        string url = "";
        if (url.Equals("")) { Trace.LogWarning("URL not set!"); yield break; }

        JSONObject json = new JSONObject();
        json.Add("currentScore", score);

        UnityWebRequest req = UnityWebRequest.Put($"{url}?username={username}", json);
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();
        WasRequestSuccesful(req);

        req.Dispose();
    }
    public IEnumerator ChangePassword(string username, string password, string newPassword) {
        string url = "";
        if (url.Equals("")) { Trace.LogWarning("URL not set!"); yield break; }

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
        string url = "";
        if (url.Equals("")) { Trace.LogWarning("URL not set!"); yield break; }

        UnityWebRequest req = UnityWebRequest.Get($"{url}");

        yield return req.SendWebRequest();

        JSONNode json = JSONNode.Parse(req.downloadHandler.text);

        Dictionary<string, ulong> bestScores = new Dictionary<string, ulong>();
        foreach (var profile in json) {
            ulong highscore;
            if (!UInt64.TryParse(profile.Value["score"], out highscore))
                Trace.LogError("Error parsing score data!");
            bestScores.Add(profile.Value["name"], highscore);
        }

        GameData.SetBestScores(bestScores);

        WasRequestSuccesful(req);

        req.Dispose();
    }
    bool WasRequestSuccesful(UnityWebRequest req) {
        if (req.isNetworkError || req.isHttpError) {
            Trace.LogError(req.error);
            Trace.LogError(req.downloadHandler.text);
            return false;
        }
        else {
            Trace.Log("Request complete: " + req.downloadHandler.text);
            return true;
        }
    }
}
