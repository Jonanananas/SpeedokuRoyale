using UnityEngine;

/// <summary>
/// Attach this to a gameobject to make it non-destroybale on load.
/// </summary>
public class DontDestroyOnLoad : MonoBehaviour {
    public static DontDestroyOnLoad Instance;
    void Awake() {
        // Destroy an existing DontDestroyOnLoad in the loaded scene if one is already loaded
        if (Instance == null)
            Instance = this;
        else {
            Destroy(gameObject);
            return;
        }

        // Don't destroy this gameobject on load
        DontDestroyOnLoad(gameObject);
    }
}
