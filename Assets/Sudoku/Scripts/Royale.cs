using System.Collections;
using System.Collections.Generic;
using System.Linq;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class Royale : Sudoku {
    private int oneGamePoints = 0;

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
        Debug.Log(oneGamePoints);
        oneGamePoints = 0;
        ManageGameSession.Instance.UpdateScore((ulong)allGamePoints);
        K += 2;
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
        ManageGameSession.Instance.UpdateScore((ulong)allGamePoints);
        //give points from last puzzle
        ManageGameSession.Instance.AddPoints((ulong)oneGamePoints);
        Debug.Log(oneGamePoints);
        oneGamePoints = 0;
    }
}