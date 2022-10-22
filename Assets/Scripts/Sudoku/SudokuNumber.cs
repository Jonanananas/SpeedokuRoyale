using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SudokuNumber {
    private int col = 0, row = 0, val = 0, answer = 0;
    public GameObject instance;
    private bool correct;

    public SudokuNumber(GameObject instance, int row, int col, int val, int answer) {
        this.instance = instance;
        this.row = row;
        this.col = col;
        this.val = val;
        this.instance.GetComponentInChildren<TextMeshProUGUI>().text = val.ToString();
        this.answer = answer;
        if (this.val == this.answer) {
            correct = true;
        } else {
            correct = false;
        }
    }

    public void SetAll(int row, int col, int val, int answer) {
        this.row = row;
        this.col = col;
        this.val = val;
        this.instance.GetComponentInChildren<TextMeshProUGUI>().text = val.ToString();
        this.answer = answer;
        if (this.val == this.answer) {
            correct = true;
        } else {
            correct = false;
        }
    }

    public bool Correct { get => this.correct;}

    public int Col { get => this.col; set => this.col = value; }

    public int Row { get => this.row; set => this.row = value; }

    public int Val { get => this.val; set => this.val = value; }

    public int Answer { get => this.answer; set => this.answer = value; }

    public void SetNumber(int x) {
        this.val = x;
        this.instance.GetComponentInChildren<TextMeshProUGUI>().text = x + "";
        if (this.val == this.answer) {
            correct = true;
        } else {
            correct = false;
        }
    }
    public void SetSelect() {
        this.instance.GetComponentInChildren<TextMeshProUGUI>().color = Color.magenta;
    }

    public void UnsetSelect() {
        this.instance.GetComponentInChildren<TextMeshProUGUI>().color = Color.cyan;
    }

    public void Disable() {
        this.instance.GetComponent<Image>().color = Color.black;
        this.instance.GetComponentInChildren<TextMeshProUGUI>().color = Color.gray;
        this.instance.GetComponent<Button>().enabled = false;
    }

    public void Enable() {
        this.instance.GetComponent<Image>().color = Color.white;
        this.instance.GetComponent<Button>().enabled = true;
        this.instance.GetComponentInChildren<TextMeshProUGUI>().color = Color.cyan;
    }

    public void DisableOnly() {
        this.instance.GetComponent<Button>().enabled = false;
    }

    public override string ToString() {
        return "Col: " + this.col + ", Row: " + this.row + ", Value: " 
        + this.val + ", Ans: " + this.answer;
    }
}
