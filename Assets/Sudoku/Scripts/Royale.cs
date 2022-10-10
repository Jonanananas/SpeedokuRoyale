using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Royale : Sudoku {
    public int playersLeft = 1;

    public void eliminatePlayer() {
        if (playersLeft > 1) {
            playersLeft--;
        }
    }

    public override void validate(bool button) {
        if (button) {
            if (wrong() != 0) {
                checkerTMP.color = Color.magenta;
                checkerTMP.text = "Incorrect!";
            } else {
                checkerTMP.color = Color.cyan;
                checkerTMP.text = "Correct!\t+" + K + " points";
                allGamePoints += K;
                newPuzzle();
            }
        } else { //stopping case
            //stop the game
            Timer2.Instance.StopTimer();
            foreach (SudokuNumber number in numbers) {
                number.disableOnly();
            }
            if (currentSudokuNumber != null) currentSudokuNumber.unsetSelect();
            NumContPanel.SetActive(false);
            //set checker time
            checkerTMP.text += "\tTime spent: " + Timer2.Instance.getEnd();
            //set endscreen info
            //if win
            Debug.Log(playersLeft);
            if (playersLeft < 2) {
                endTitleTMP.color = Color.cyan;//new Color32(132, 250, 255, 255);
                endTitleTMP.text = "Victory Royale!";
                LocalPlayer.Instance.IncrementVictories();
            } else { //if lose
                endTitleTMP.color = Color.magenta;
                endTitleTMP.text = "Defeated";
            }
            //placementTMP.text = "Time spent: (m:s:ms): " + Timer2.Instance.getEnd();
            placementTMP.text = getPlacement();
            allGamePoints += K - wrong();
            scoreTMP.text = "Solved: " + allGamePoints + " numbers\nTime spent: (m:s:ms): " + Timer2.Instance.getEnd();
            //give points
            LocalPlayer.Instance.AddPoints((ulong)allGamePoints);
            //show endscreen in 1 second
            StartCoroutine(waiter(1f));
            IEnumerator waiter(float s){
                yield return new WaitForSeconds(s);
                EndScreen.SetActive(true);
            }
        } 
    }
    
    private string getPlacement() {
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
        return $"You placed {playersLeft}{placementSuffix}";
    }
}