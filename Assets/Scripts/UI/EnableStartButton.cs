using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class EnableStartButton : MonoBehaviour {
    [SerializeField] GameObject startButton;

    void OnEnable() {
        startButton.SetActive(true);
    }
}
