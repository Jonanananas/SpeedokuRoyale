using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using TMPro;

[RequireComponent(typeof(Button))]
public class StartGameButton : MonoBehaviour, IPointerUpHandler, ButtonInterface {
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
    [SerializeField] GameObject startButtonGO, loseGameTestButtonGO, addScoreTestButton, thisGameObject;
    public void StartGame() {
        startButtonGO.SetActive(false);
        if (loseGameTestButtonGO != null)
            loseGameTestButtonGO.SetActive(true);
        if (addScoreTestButton != null)
            addScoreTestButton.SetActive(true);
        ManageGameSession.Instance.StartGame();
    }
    Button btn;
    void Start() {
        btn = gameObject.GetComponent<Button>();
    }
    public void OnPointerUp(PointerEventData eventData) {
        TryToPress();
    }
    public void TryToPress() {
        if (!btn.interactable) return;

        if (GameStates.isOnlineMode) {
            ServerGameRooms.Instance.GetAvailableGameRooms();
            thisGameObject.SetActive(false);
        }
        else {
            StartGame();
        }
    }
}