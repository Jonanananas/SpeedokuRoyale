using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class ServerGameOperations : MonoBehaviour {
    public static ServerGameOperations Instance;
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
    public void GetAvailableGameRooms() {
        StartCoroutine(ServerGameroomReqs.Instance.GetAvailableGameRooms());
    }
}
