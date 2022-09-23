using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using TMPro;

[RequireComponent(typeof(Button))]
public class LoginButton : MonoBehaviour, IPointerUpHandler {
    [SerializeField] TMP_InputField usernameInField, passwordInField;
    int minimumInputLength = GlobalVariables.minimumInputLength;
    Button btn;
    void Start() {
        btn = gameObject.GetComponent<Button>();
    }
    public void OnPointerUp(PointerEventData eventData) {
        TryToPress();
    }
    void TryToPress() {
        if (!btn.interactable) return;
        if (usernameInField.text.Length < minimumInputLength ||
            passwordInField.text.Length < minimumInputLength) {
            Trace.Log($"Your username and password must be atleast {minimumInputLength} characters long.");
            return;
        }

        StartCoroutine(ServerUser.Instance.LogIn(usernameInField.text, passwordInField.text));
    }
}