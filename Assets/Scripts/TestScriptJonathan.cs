using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class TestScriptJonathan : MonoBehaviour {
    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        Timer.Instance.StartTimer();
        print(Timer.Instance.text);
    }
}
