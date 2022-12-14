using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using TMPro;

[RequireComponent(typeof(Button))]
public class ChangePasswordButton : MonoBehaviour, IPointerUpHandler, ButtonInterface {
    public GameObject userSettingsGO, changePasswordGO;
    [SerializeField] TMP_InputField passwordField, passwordNewField, passwordNewFieldR;
    [SerializeField] Button btn;
    public static ChangePasswordButton Instance;
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
    public void OnPointerUp(PointerEventData eventData) {
        TryToPress();
    }
    public void TryToPress() {
        if (!btn.interactable) return;
        
        if (!passwordNewField.text.Equals(passwordNewFieldR.text)) {
            Trace.Log("Input passwords don't match.");
            return;
        }
        ServerPlayerProfiles.Instance.ChangePassword(passwordField.text, passwordNewField.text);
    }
}