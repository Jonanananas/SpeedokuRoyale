using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Localization.Components;
using TMPro;

public class DisplayProfileData : MonoBehaviour {
    [SerializeField] TextMeshProUGUI usernameTMP, victoriesValueTMP, personalBestValueTMP;
    [SerializeField] LocalizeStringEvent usernameLSE, victoriesLSE, personalBestLSE;

    void OnEnable() {
        if (LocalPlayer.Instance.profile == null) {
            usernameLSE.StringReference.TableEntryReference = "Login2C";
            victoriesLSE.StringReference.TableEntryReference = "Empty";
            personalBestLSE.StringReference.TableEntryReference = "Empty";
            victoriesValueTMP.text = "";
            personalBestValueTMP.text = "";
            return;
        }
        victoriesLSE.StringReference.TableEntryReference = "GamesWon";
        personalBestLSE.StringReference.TableEntryReference = "BestScore";

        usernameTMP.text = LocalPlayer.Instance.profile.username;
        victoriesValueTMP.text = LocalPlayer.Instance.profile.victories.ToString();
        personalBestValueTMP.text = LocalPlayer.Instance.profile.bestScore.ToString();
    }
}
