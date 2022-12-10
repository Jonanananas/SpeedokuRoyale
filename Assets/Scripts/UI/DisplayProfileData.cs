using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Localization.Components;
using TMPro;

public class DisplayProfileData : MonoBehaviour {
    [SerializeField] TextMeshProUGUI usernameTMP, victoriesTMP, personalBestTMP;

    void OnEnable() {
        if (LocalPlayer.Instance.profile == null) {
            usernameTMP.gameObject.GetComponent<LocalizeStringEvent>().StringReference.TableEntryReference = "Login2C";
            victoriesTMP.gameObject.GetComponent<LocalizeStringEvent>().StringReference.TableEntryReference = "Empty";
            personalBestTMP.gameObject.GetComponent<LocalizeStringEvent>().StringReference.TableEntryReference = "Empty";
            return;
        }
        victoriesTMP.gameObject.GetComponent<LocalizeStringEvent>().StringReference.TableEntryReference = "GamesWon";
        personalBestTMP.gameObject.GetComponent<LocalizeStringEvent>().StringReference.TableEntryReference = "BestScore";

        usernameTMP.text = LocalPlayer.Instance.profile.username;
        victoriesTMP.text = LocalPlayer.Instance.profile.victories.ToString();
        personalBestTMP.text = LocalPlayer.Instance.profile.bestScore.ToString();
    }
}
