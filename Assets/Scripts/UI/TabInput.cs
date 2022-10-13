using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TabInput : MonoBehaviour {
    public TMP_InputField[] fields;
    public int selected;
    void Start() {
        selected = 0;
        fields = GetComponentsInChildren<TMP_InputField>();
    }

    public void Update() {
        if (Input.GetKeyDown(KeyCode.Tab)) {
            if (selected < (fields.Length-1)) {
                selected++;
            } else {
                selected = 0;
            }
            fields[selected].Select();
        }

    }

    public void selectMe(int num){
        selected = num;
    }

}
