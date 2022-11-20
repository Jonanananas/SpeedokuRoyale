using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class GivePoints : MonoBehaviour, IPointerUpHandler {
    Button btn;
    void Start() {
        btn = gameObject.GetComponent<Button>();
    }
    public void OnPointerUp(PointerEventData eventData) {
        TryToPress();
    }
    void TryToPress() {
        if (!btn.interactable) return;
        ManageGameSession.Instance.AddPoints(1);
        ManageGameSession.Instance.UpdateScore(LocalPlayer.Instance.GetScore());
    }
}
