using System.Collections;

using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;

public class ServerConnection : MonoBehaviour {
    public static ServerConnection Instance;
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

            LocalPlayer.Instance.SetLocalPlayerProfile(new PlayerProfile(
                json["username"].Value,
                json["password"].Value,
                json["bestScore"].Value
            ));
            Debug.Log("Login successful!");
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
    public IEnumerator UpdateUserBestScore(string username, int score) {
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
    public IEnumerator ChangeUserPassword(string username, string password, string newPassword) {
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
