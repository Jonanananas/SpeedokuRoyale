using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using TMPro;

[RequireComponent(typeof(Button))]
public class RegisterButton : MonoBehaviour, IPointerUpHandler {
    [SerializeField] TMP_InputField usernameInField, passwordInField, passwordRepeatInField;
    [SerializeField] GameObject registerMenuGO, registerInfoMenuGO;
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
        // Check for proper login length
        // if (usernameInField.text.Length < minimumInputLength ||
        //     passwordInField.text.Length < minimumInputLength ||
        //     passwordRepeatInField.text.Length < minimumInputLength) {
        //     Trace.Log($"Your username and password must be atleast {minimumInputLength} characters long.");
        //     return;
        // }
        if (!passwordInField.text.Equals(passwordRepeatInField.text)) {
            Trace.Log("Input passwords don't match.");
            return;
        }
        registerInfoMenuGO.SetActive(true);
        ServerPlayerProfiles.Instance.RegisterUser(usernameInField.text, passwordInField.text);
        registerMenuGO.SetActive(false);
    }
}