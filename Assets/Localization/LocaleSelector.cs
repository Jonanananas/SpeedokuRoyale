using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using TMPro;

public class LocaleSelector : MonoBehaviour {

    public TMP_Dropdown dropdown;

    private bool active = false;

    // void Awake() {
    //     dropdown.value = 0;
    //     ChangeLocale(0);
    // }

    void Start() {
        if (LocalizationSettings.SelectedLocale
            == LocalizationSettings.AvailableLocales.GetLocale("en")) {
            dropdown.value = 0;
        }
        else {
            dropdown.value = 1;
        }
    }

    public void ChangeLocale(int localeID) {
        if (active) {
            return;
        }
        StartCoroutine(SetLocale(localeID));
    }

    IEnumerator SetLocale(int localeID) {
        print("localeID: " + localeID);
        active = true;
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[localeID];
        GameStates.localeGot = true;
        active = false;
    }
}
