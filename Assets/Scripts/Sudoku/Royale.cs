using System.Collections;
using System.Collections.Generic;
using System.Linq;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Royale luokka on Sudokun perivä moninpelisovellutus.
/// </summary>
public class Royale : Sudoku {
    private int oneGamePoints = 0;
    /// <summary>
    /// NewPuzzle() -metodilla nollataan ja asetetaan sudokuun uudet numerot.
    /// </summary>
    public void NewPuzzle() {
        currentSudokuNumber.Disable();
        currentSudokuNumber = null;
        answers = new int[N, N];
        values = new int[N, N];
        SRN = (int)Mathf.Sqrt(N);

        GenerateSudoku();
        SetNumbers();
    }
    /// <summary>
    /// Stop() -metodilla kytketään sudoku numerot "pois päältä" ManageGameSession -luokasta.
    /// </summary>
    public void Stop() {
        foreach (SudokuNumber number in numbers) {
            number.DisableOnly();
        }
        if (currentSudokuNumber != null) currentSudokuNumber.UnsetSelect();
        NumContPanel.SetActive(false);
    }
    /// <summary>
    /// AfterCorrectButtonValidate() -metodia kutsutaan ManageGameSession -luokasta validoinnin jälkeen.
    /// </summary>
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
    /// <summary>
    /// SudokuEnd() -metodi kutsutaan pelin lopussa ManageGameSession -luokasta.
    /// </summary>
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