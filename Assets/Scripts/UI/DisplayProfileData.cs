using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using TMPro;

public class DisplayProfileData : MonoBehaviour {
    [SerializeField] TextMeshProUGUI usernameTMP, victoriesTMP, personalBestTMP;

    void OnEnable() {
        if (LocalPlayer.Instance.profile == null) {
            usernameTMP.text = "Log in to see profile data!";
            victoriesTMP.text = "";
            personalBestTMP.text = "";
            return;
        }
        usernameTMP.text = LocalPlayer.Instance.profile.username;
        victoriesTMP.text = "Games Won: " + LocalPlayer.Instance.profile.victories.ToString();
        personalBestTMP.text = "Best Score: " + LocalPlayer.Instance.profile.bestScore.ToString();
    }
}
