using UnityEngine;

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
