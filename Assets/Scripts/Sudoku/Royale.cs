using System.Collections;
using System.Collections.Generic;
using System.Linq;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class Royale : Sudoku {
    //Royale luokka on Sudokun perivä moninpeli sovellutus
    private int oneGamePoints = 0;

    // NewPuzzle() -metodilla nollataan ja asetetaan sudokuun uudet numerot
    public void NewPuzzle() {
        currentSudokuNumber.Disable();
        currentSudokuNumber = null;
        answers = new int[N, N];
        values = new int[N, N];
        SRN = (int)Mathf.Sqrt(N);

        GenerateSudoku();
        SetNumbers();
    }
    // Stop() -metodilla kytketään sudoku numerot "pois päältä" ManageGameSession -luokasta
    public void Stop() {
        foreach (SudokuNumber number in numbers) {
            number.DisableOnly();
        }
        if (currentSudokuNumber != null) currentSudokuNumber.UnsetSelect();
        NumContPanel.SetActive(false);
    }
    // AfterCorrectButtonValidate() -metodia kutsutaan ManageGameSession -luokasta validoinnin jälkeen
    public void AfterCorrectButtonValidate() {
        oneGamePoints += GetPoints();
        ManageGameSession.Instance.AddPoints((ulong)oneGamePoints);
        allGamePoints += oneGamePoints;
        Debug.Log(oneGamePoints);
        oneGamePoints = 0;
        ManageGameSession.Instance.UpdateScore((ulong)allGamePoints);
        K += 2;
        NewPuzzle();
    }
    //SudokuEnd() -metodi kutsutaan pelin lopussa ManageGameSession -luokasta
    public override void SudokuEnd() {
        // Aseta numerot
        oneGamePoints += GetPoints();
        allGamePoints += oneGamePoints;
        // oneGamePoints = 0;
        // Aseta checker aika & päivitä score
        ManageGameSession.Instance.SetTimeSpent(false);
        // ManageGameSession.Instance.UpdateScore((ulong)allGamePoints);
        // Anna pisteitä viimeisestä sudoku taulukosta vaikka jäisi kesken
        // ManageGameSession.Instance.AddPoints((ulong)oneGamePoints);
        oneGamePoints = 0;
    }
}