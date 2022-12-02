using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using TMPro;

[RequireComponent(typeof(Button))]
public class LoginButton : MonoBehaviour, IPointerUpHandler, ButtonInterface {
    [SerializeField] GameObject mainMenu, loginMenu;
    public void CloseLoginMenu() {
        // Go to main menu
        loginMenu.SetActive(false);
        mainMenu.SetActive(true);
    }
    [SerializeField] TMP_InputField usernameInField, passwordInField;
    [SerializeField] GameObject loginInfoMenuGO, loginMenuGO;
    int minimumInputLength = GlobalVariables.minimumInputLength;
    Button btn;
    void Start() {
        btn = gameObject.GetComponent<Button>();
    }
    public void OnPointerUp(PointerEventData eventData) {
        TryToPress();
    }
    public void TryToPress() {
        // if (!btn.interactable) return;
        // if (usernameInField.text.Length < minimumInputLength ||
        //     passwordInField.text.Length < minimumInputLength) {
        //     Trace.Log($"Your username and password must be atleast {minimumInputLength} characters long.");
        //     return;
        // }
        loginInfoMenuGO.SetActive(true);
        ServerPlayerProfiles.Instance.LogIn(usernameInField.text, passwordInField.text);
        loginMenuGO.SetActive(false);
    }
}