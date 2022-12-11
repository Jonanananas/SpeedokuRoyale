using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using TMPro;

[RequireComponent(typeof(Button))]
public class QuitButton : MonoBehaviour, IPointerUpHandler, ButtonInterface {
    [SerializeField] GameObject quitConfirmationBox;
    Button btn;
    void Start() {
        btn = gameObject.GetComponent<Button>();
    }
    public void OnPointerUp(PointerEventData eventData) {
        TryToPress();
    }
    public void TryToPress() {
        if (!btn.interactable) return;

        if (Timer.Instance.isRunning) {
            quitConfirmationBox.SetActive(true);
            return;
        }
        ManageScenes.Instance.LoadMainMenu();
    }
}