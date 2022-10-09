using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using TMPro;

[RequireComponent(typeof(Button))]
public class GetHighscoresButton : MonoBehaviour, IPointerUpHandler {
    Button btn;
    void Start() {
        btn = gameObject.GetComponent<Button>();
    }
    public void OnPointerUp(PointerEventData eventData) {
        TryToPress();
    }
    void TryToPress() {
        if (!btn.interactable) return;
        if (GameData.highscoreProfiles != null && GameData.highscoreProfiles.Count != 0) return;

        StartCoroutine(ServerPlayerProfileReqs.Instance.GetLeaderboardProfiles());
    }
}