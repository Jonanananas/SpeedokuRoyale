using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using TMPro;

public class ManageGameSession : MonoBehaviour {
    public static ManageGameSession Instance;
    public GameObject gameEndMenu;
    public TextMeshProUGUI victoryOrDefeatTMP, placementTMP, scoreTMP, userTMP, currentScoreTMP;
    [SerializeField] Sudoku sudoku;
    int playersLeft = 2;
    void Awake() {
        #region Singleton
        if (Instance != null) {
            Trace.LogError("More than one instance of " + this.GetType().Name + " found!");
            return;
        }
        Instance = this;
        #endregion
        playersLeft = 2;
    }
    public void StartGame() {
        LocalPlayer.Instance.ResetScore();
        Timer.Instance.StartTimer();
        sudoku.initializeGame();
    }
    public void LoseGame() {
        EndGame();
        Timer.Instance.StopTimer();
        victoryOrDefeatTMP.text = "Defeat Royale";
        victoryOrDefeatTMP.color = Color.red;
        SetPlacingText();
    }
    public void WinGame() {
        EndGame();
        LocalPlayer.Instance.IncrementVictories();
        victoryOrDefeatTMP.text = "Victory Royale!";
        victoryOrDefeatTMP.color = new Color32(132, 250, 255, 255);
        placementTMP.text = $"You placed 1st";
    }
    void EndGame() {
        gameEndMenu.SetActive(true);
        scoreTMP.text = $"Score: {LocalPlayer.Instance.GetScore()}";
        LocalPlayer.Instance.UpdateLocalHighScore();
    }
    public void EliminateOnePlayer() {
        playersLeft--;
    }
    public void AddPoints(ulong pointsToAdd) {
        LocalPlayer.Instance.AddPoints(pointsToAdd);
    }
    public void AddPoint() {
        LocalPlayer.Instance.AddPoints(1);
    }
    public void UpdateScore(ulong score) {
        currentScoreTMP.text = score.ToString();
    }
    void SetPlacingText() {
        string placementSuffix = "th";
        int modulo = playersLeft % 10;
        switch (modulo) {
            case 1:
                placementSuffix = "st";
                break;
            case 2:
                placementSuffix = "nd";
                break;
            case 3:
                placementSuffix = "rd";
                break;
        }
        if (playersLeft % 100 >= 11 && playersLeft % 100 <= 20) {
            placementSuffix = "th";
        }
        placementTMP.text = $"You placed {playersLeft}{placementSuffix}";
    }
}
