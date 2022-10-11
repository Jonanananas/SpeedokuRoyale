using System.Collections;
using System.Collections.Generic;
using System.Linq;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class Royale : Sudoku {
    public int playersLeft = 1;
    private int oneGamePoints = 0;
    public void eliminatePlayer() {
        if (playersLeft > 1) {
            playersLeft--;
        }
    }

    public void newPuzzle() {
        currentSudokuNumber.disable();
        currentSudokuNumber = null;
        answers = new int[N, N];
        values = new int[N, N];
        SRN = (int)Mathf.Sqrt(N);

        generateSudoku();

        //Set numbers again
        setNumbers();
    }

    public void stop() {
        //stop
        foreach (SudokuNumber number in numbers) {
            number.disableOnly();
        }
        if (currentSudokuNumber != null) currentSudokuNumber.unsetSelect();
        NumContPanel.SetActive(false);
    }

    public void afterCorrectButtonValidate() {
        oneGamePoints += getPoints();
        ManageGameSession.Instance.AddPoints((ulong)oneGamePoints);
        allGamePoints += oneGamePoints;
        oneGamePoints = 0;
        ManageGameSession.Instance.UpdateScore((ulong)allGamePoints);
        newPuzzle();
    }

    public override void sudokuEnd() {
        // if (playersLeft < 2) {
        //     ManageGameSession.Instance.WinGame();
        // }
        // else {
        //     ManageGameSession.Instance.LoseGame();
        // }
        //points
        oneGamePoints += getPoints();
        allGamePoints += oneGamePoints;
        oneGamePoints = 0;
        //set endscreen info & checker time
        ManageGameSession.Instance.SetTimeSpent(false);
        ManageGameSession.Instance.SetPlacingText(playersLeft);
        ManageGameSession.Instance.UpdateScore((ulong)allGamePoints);
        //give points
        // ManageGameSession.Instance.AddPoints((ulong)allGamePoints);
    }
}