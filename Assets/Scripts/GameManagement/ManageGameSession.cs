using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using TMPro;

public class ManageGameSession : MonoBehaviour {
    public static ManageGameSession Instance;
    public GameObject gameEndMenu;
    [SerializeField]
    private TextMeshProUGUI
    checkerTMP, victoryOrDefeatTMP, placementTMP,
    scoreTMP, userTMP, currentScoreTMP;
    [SerializeField] Sudoku sudoku;

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
        sudoku.gameObject.SetActive(true);
        LocalPlayer.Instance.ResetScore();
        Timer.Instance.StartTimer();
        sudoku.initializeGame();
    }
    public void LoseGame() {
        EndGame();
        setWinText(false);
    }
    public void WinGame() {
        EndGame();
        LocalPlayer.Instance.IncrementVictories();
        setWinText(true);
    }
    public void EndGame() {
        Timer.Instance.StopTimer();
        sudoku.sudokuEnd();
        string scene = ManageScenes.Instance.GetSceneName();
        if (scene == "Singleplayer") {
            if (sudoku.getWrong() > 0) {
                setWinText(false);
            }
            else {
                setWinText(true);
            }
        }
        gameEndMenu.SetActive(true);
    }

    public void AddPoints(ulong pointsToAdd) {
        LocalPlayer.Instance.AddPoints(pointsToAdd);
    }
    public void AddPoint() {
        LocalPlayer.Instance.AddPoints(1);
    }
    public void UpdateScore(ulong score) {
        currentScoreTMP.text = "Score: " + score.ToString();
        scoreTMP.text = "Score: " + score.ToString();
        LocalPlayer.Instance.UpdateLocalHighScore();
    }
    public void SetPlacingText(int placing) {
        string placementSuffix = "th";
        int modulo = placing % 10;
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
        if (placing % 100 >= 11 && placing % 100 <= 20) {
            placementSuffix = "th";
        }
        placementTMP.text = $"You placed {placing}{placementSuffix}";
    }
    public void ButtonValidateClassic() {
        if (sudoku.getWrong() != 0) {
            checkerTMP.color = Color.magenta;
            checkerTMP.text = "Incorrect!";
        }
        else {
            checkerTMP.color = Color.cyan;
            checkerTMP.text = "Correct!";
            EndGame();
        }
    }
    public void ButtonValidateRoyale() {
        Royale royale = (Royale)sudoku;
        if (royale.getWrong() != 0) {
            checkerTMP.color = Color.magenta;
            checkerTMP.text = "Incorrect!";
        }
        else {
            checkerTMP.color = Color.cyan;
            checkerTMP.text = "Correct!";
            royale.afterCorrectButtonValidate();
        }
    }
    public void ClearCheckerText() {
        checkerTMP.text = "";
    }
    public void SetTimeSpent(bool solo) {
        if (solo) {
            checkerTMP.text += "\tTime spent: " + Timer.Instance.GetElapsedTime();
            placementTMP.text = "Time spent (m:s.f): " + Timer.Instance.GetElapsedTime();
        }
        else {
            checkerTMP.text += "\tTime spent: " + Timer.Instance.GetElapsedTime();
        }
    }

    private void setWinText(bool win) {
        if (win) {
            checkerTMP.color = Color.cyan;
            victoryOrDefeatTMP.text = "Victory Royale!";
            victoryOrDefeatTMP.color = new Color32(132, 250, 255, 255);
        }
        else {
            checkerTMP.color = Color.red;
            victoryOrDefeatTMP.text = "Defeat Royale";
            victoryOrDefeatTMP.color = Color.red;
        }
    }
}
