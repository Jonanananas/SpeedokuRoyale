using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using TMPro;

public class DisplayProfileData : MonoBehaviour {
    [SerializeField] TextMeshProUGUI usernameTMP, victoriesTMP, personalBestTMP;

    void OnEnable() {
        if (LocalPlayer.Instance.profile == null) return;
        usernameTMP.text = LocalPlayer.Instance.profile.username;
        victoriesTMP.text = LocalPlayer.Instance.profile.victories.ToString();
        personalBestTMP.text = LocalPlayer.Instance.profile.bestScore.ToString();
    }
}
