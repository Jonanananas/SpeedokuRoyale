using System.Collections;
using System.Collections.Generic;
using System.Linq;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class Royale : Sudoku {
    //Royale luokka on Sudokun perivä moninpeli sovellutus
    private int oneGamePoints = 0;

    // newPuzzle() -metodilla nollataan ja asetetaan sudokuun uudet numerot
    public void newPuzzle() {
        currentSudokuNumber.disable();
        currentSudokuNumber = null;
        answers = new int[N, N];
        values = new int[N, N];
        SRN = (int)Mathf.Sqrt(N);

        generateSudoku();
        setNumbers();
    }
    // stop() -metodilla kytketään sudoku numerot "pois päältä" ManageGameSession -luokasta
    public void stop() {
        foreach (SudokuNumber number in numbers) {
            number.disableOnly();
        }
        if (currentSudokuNumber != null) currentSudokuNumber.unsetSelect();
        NumContPanel.SetActive(false);
    }
    // afterCorrectButtonValidate() -metodia kutsutaan ManageGameSession -luokasta validoinnin jälkeen
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
    //sudokuEnd() -metodi kutsutaan pelin lopussa ManageGameSession -luokasta
    public override void sudokuEnd() {
        // Aseta numerot
        oneGamePoints += getPoints();
        allGamePoints += oneGamePoints;
        oneGamePoints = 0;
        // Aseta checker aika & päivitä score
        ManageGameSession.Instance.SetTimeSpent(false);
        ManageGameSession.Instance.UpdateScore((ulong)allGamePoints);
        // Anna pisteitä viimeisestä sudoku taulukosta vaikka jäisi kesken
        ManageGameSession.Instance.AddPoints((ulong)oneGamePoints);
        oneGamePoints = 0;
    }
}