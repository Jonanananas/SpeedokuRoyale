using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using TMPro;

[RequireComponent(typeof(Button))]
public class DeleteUserButton : MonoBehaviour, IPointerUpHandler {
    Button btn;
    void Start() {
        btn = gameObject.GetComponent<Button>();
    }
    public void OnPointerUp(PointerEventData eventData) {
        TryToPress();
    }
    void TryToPress() {
        if (!btn.interactable) return;

        if (!GameStates.isLoggedIn) {
            Trace.LogWarning("Can't delete a profile because the user is not logged in!");
            return;
        }

        ServerPlayerProfiles.Instance.DeleteUserProfile();
    }
}