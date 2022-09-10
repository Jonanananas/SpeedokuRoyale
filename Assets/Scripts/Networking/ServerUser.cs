using System.Collections;

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
        string url = "";
        UnityWebRequest req = new UnityWebRequest($"{url}?username={username}&password={password}", "PATCH");
        req.downloadHandler = new DownloadHandlerBuffer();

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
                    json["password"].Value,
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
    }
    public IEnumerator LogOut(string username) {
        string url = "";
        UnityWebRequest req = new UnityWebRequest($"{url}?username={username}", "PATCH");
        req.downloadHandler = new DownloadHandlerBuffer();

        yield return req.SendWebRequest();
        if (WasRequestSuccesful(req)) {
            Trace.Log("Logout successful!");
            // TODO: Log the user out
        }
        else {
            Trace.LogError("Error logging out!");
        }
    }
    public IEnumerator CreateUser(string username, string password) {
        string url = "";
        JSONObject json = new JSONObject();
        json.Add("username", username);
        json.Add("password", password);

        UnityWebRequest req = new UnityWebRequest($"{url}", "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json.ToString());
        req.uploadHandler = new UploadHandlerRaw(jsonToSend);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();
        WasRequestSuccesful(req);
    }
    public IEnumerator UpdateBestScore(string username, ulong score) {
        string url = "";
        JSONObject json = new JSONObject();
        json.Add("bestScore", score);

        UnityWebRequest req = new UnityWebRequest($"{url}?username={username}", "PATCH");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json.ToString());
        req.uploadHandler = new UploadHandlerRaw(jsonToSend);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();
        WasRequestSuccesful(req);
    }
    public IEnumerator UpdateVictories(string username, ulong victories) {
        string url = "";
        JSONObject json = new JSONObject();
        json.Add("victories", victories);

        UnityWebRequest req = new UnityWebRequest($"{url}?username={username}", "PATCH");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json.ToString());
        req.uploadHandler = new UploadHandlerRaw(jsonToSend);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();
        WasRequestSuccesful(req);
    }
    public IEnumerator UpdateCurrentScore(string username, ulong score) {
        string url = "";
        JSONObject json = new JSONObject();
        json.Add("currentScore", score);

        UnityWebRequest req = new UnityWebRequest($"{url}?username={username}", "PATCH");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json.ToString());
        req.uploadHandler = new UploadHandlerRaw(jsonToSend);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();
        WasRequestSuccesful(req);
    }
    public IEnumerator ChangePassword(string username, string password, string newPassword) {
        string url = "";
        JSONObject json = new JSONObject();
        json.Add("newPassword", newPassword);

        UnityWebRequest req = new UnityWebRequest($"{url}?username={username}&password={password}", "PATCH");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json.ToString());
        req.uploadHandler = new UploadHandlerRaw(jsonToSend);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();
        WasRequestSuccesful(req);
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
