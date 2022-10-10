using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SessionManager : MonoBehaviour {
    public static SessionManager Instance;
    [SerializeField] private Sudoku sudoku;

    void Awake() {
        #region Singleton
        if (Instance != null) {
            Trace.LogError("More than one instance of " + this.GetType().Name + " found!");
            return;
        }
        Instance = this;
        #endregion
    }

    public void StartGame() {
        Timer2.Instance.StartTimer();
        sudoku.initializeGame();
    }

    public void EndGame() {
        sudoku.validate(false);
    }

    public void EliminateOnePlayer() {
        Royale royale = (Royale) sudoku;
        royale.eliminatePlayer();
    }
}

