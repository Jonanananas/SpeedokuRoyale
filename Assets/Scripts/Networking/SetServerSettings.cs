// using System.IO;

// using UnityEngine;

// using SimpleJSON;

// public class SetServerSettings : MonoBehaviour {
//     static SetServerSettings Instance;
//     JSONNode serverJSON = new JSONObject();
//     void Awake() {

//         #region Singleton
//         if (Instance != null) {
//             Destroy(gameObject);
//             // Trace.LogWarning("More than one instance of " + this.GetType().Name + " found!");
//             return;
//         }
//         Instance = this;
//         // Don't destroy this gameobject on load
//         DontDestroyOnLoad(gameObject);
//         #endregion

//         string serverSettingsPath = Application.dataPath + "/server-settings.json";

//         if (File.Exists(serverSettingsPath)) {
//             string fileContent = File.ReadAllText(serverSettingsPath);
//         }
//         else {
//             File.Create(serverSettingsPath).Dispose();
//             serverJSON.Add("baseUrl", "http://10.114.32.14:8000");
//             string fileContent = serverJSON.ToString();
//             File.WriteAllText(serverSettingsPath, fileContent);
//         }
//     }
// }
