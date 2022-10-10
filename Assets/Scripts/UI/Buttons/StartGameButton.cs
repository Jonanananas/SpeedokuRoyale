using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using TMPro;

[RequireComponent(typeof(Button))]
public class StartGameButton : MonoBehaviour, IPointerUpHandler {
    public static StartGameButton Instance;
    void Awake() {
        #region Singleton
        if (Instance != null) {
            Destroy(gameObject);
            Trace.LogWarning("More than one instance of " + this.GetType().Name + " found!");
            return;
        }
        Instance = this;
        #endregion
    }
    [SerializeField] GameObject startButtonGO, loseGameTestButtonGO;
    public void StartGame() {
        startButtonGO.SetActive(false);
        loseGameTestButtonGO.SetActive(true);
        ManageGameSession.Instance.StartGame();
    }
    Button btn;
    void Start() {
        btn = gameObject.GetComponent<Button>();
    }
    public void OnPointerUp(PointerEventData eventData) {
        TryToPress();
    }
    void TryToPress() {
        if (!btn.interactable) return;

        if (GameStates.isOnlineMode) {
            StartCoroutine(ServerUser.Instance.GetGameRoomStatus());
        }
        else {
           StartGame(); 
        }
    }
}