using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using TMPro;

[RequireComponent(typeof(Button))]
public class LoginButton : MonoBehaviour, ISubmitHandler, IPointerUpHandler, IPointerEnterHandler, ISelectHandler {
    [SerializeField] TMP_InputField usernameInputField, passwordInputField;
    Button btn;
    void Start() {
        btn = gameObject.GetComponent<Button>();
    }
    public void OnSubmit(BaseEventData eventData) {
        TrySetIsPressed();
    }
    public void OnPointerUp(PointerEventData eventData) {
        TrySetIsPressed();
    }
    public void OnSelect(BaseEventData eventData) {
        TrySetIsHighlighted();
    }
    public void OnPointerEnter(PointerEventData eventData) {
        TrySetIsHighlighted();
    }
    void TrySetIsPressed() {
        if (btn.interactable) {
            StartCoroutine(ServerUser.Instance.LogIn(usernameInputField.text, passwordInputField.text));
        }
    }
    void TrySetIsHighlighted() {
        if (btn.interactable) {
        }
    }
}