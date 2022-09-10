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
            Debug.LogWarning("More than one instance of " + this.GetType().Name + " found!");
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

            int bestScore;
            int victories;

            if (Int32.TryParse(json["victories"].Value, out victories) &&
                Int32.TryParse(json["bestScore"].Value, out bestScore)
            ) {
                LocalPlayer.Instance.SetLocalPlayerProfile(new PlayerProfile(
                    json["username"].Value,
                    json["password"].Value,
                    bestScore,
                    victories
                ));
                Debug.Log("Login successful!");
            }
            else {
                Debug.Log("Failed to parse user profile int data!");
            }
        }
        else {
            Debug.Log("Error logging in!");
        }
    }
    public IEnumerator LogOut(string username) {
        string url = "";
        UnityWebRequest req = new UnityWebRequest($"{url}?username={username}", "PATCH");
        req.downloadHandler = new DownloadHandlerBuffer();

        yield return req.SendWebRequest();
        if (WasRequestSuccesful(req)) {
            Debug.Log("Logout successful!");
            // TODO: Log the user out
        }
        else {
            Debug.Log("Error logging out!");
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
    public IEnumerator UpdateBestScore(string username, int score) {
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
    public IEnumerator UpdateVictories(string username, int victories) {
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
            Debug.Log(req.error);
            Debug.Log(req.downloadHandler.text);
            return false;
        }
        else {
            Debug.Log("Request complete: " + req.downloadHandler.text);
            return true;
        }
    }
}
