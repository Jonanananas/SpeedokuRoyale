using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class ServerOperations : MonoBehaviour {
    public static ServerOperations Instance;
    void Awake() {
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
    public void RegisterUser(string userName, string password) {
        StartCoroutine(ServerUser.Instance.RegisterUser(userName, password));
    }
}
