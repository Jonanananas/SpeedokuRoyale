using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ControlObj {
    private int val = 0;
    public GameObject instance;

    public ControlObj(GameObject instance, int val) {
        this.instance = instance;
        this.val = val;
        this.instance.GetComponentInChildren<TextMeshProUGUI>().text = val.ToString();
    }

    public int Val { get => this.val; set => this.val = value; }

    public void setNumber(int value) {
        this.val = value;
        this.instance.GetComponentInChildren<TextMeshProUGUI>().text = value.ToString();
    }

    public void setSelect() {
        this.instance.GetComponentInChildren<TextMeshProUGUI>().color = new Color32(255, 220, 98, 255);
    }

    public void unsetSelect() {
        this.instance.GetComponentInChildren<TextMeshProUGUI>().color = Color.blue;
    }

    public override string ToString() {
        return this.val + "";
    }
}
