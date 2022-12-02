using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using TMPro;

[RequireComponent(typeof(Button))]
public class LogoutOrLoginButton : MonoBehaviour, IPointerUpHandler, ButtonInterface {
    [SerializeField] TextMeshProUGUI buttonTMP;
    [SerializeField] GameObject loginMenu, mainMenu;
    Button btn;
    public void UpdateButtonText() {
        if (GameStates.isLoggedIn) {
            buttonTMP.text = "Log out";
            return;
        }
        buttonTMP.text = "Log in";
    }
    public void UpdateButtonText(string text) {
        buttonTMP.text = text;
    }
    public void OnPointerUp(PointerEventData eventData) {
        TryToPress();
    }
    void Start() {
        btn = gameObject.GetComponent<Button>();
    }
    void OnEnable() {
        UpdateButtonText();
    }
    public void TryToPress() {
        if (!btn.interactable) return;

        // Try to log out if the user is logged in
        if (GameStates.isLoggedIn) {
            LocalPlayer.Instance.LogOut();
            UpdateButtonText();
            return;
        }
        // Go to login screen if the user is not logged in
        loginMenu.SetActive(true);
        mainMenu.SetActive(false);
    }
}