using System.Collections;
using System.Collections.Generic;

using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Components;
using UnityEngine;
using TMPro;

public class ManageGameSession : MonoBehaviour {
    public static ManageGameSession Instance;
    public GameObject gameEndMenu;
    [SerializeField]
    private TextMeshProUGUI checkerTMP, endHeaderTMP, 
    placementTMP, scoreTMP, userTMP, currentScoreTMP;
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
        sudoku.InitializeGame();
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
        sudoku.SudokuEnd();

        if (!GameStates.isOnlineMode) {
            if (sudoku.GetWrong() > 0) {
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
        currentScoreTMP.text = score.ToString();
        scoreTMP.text = score.ToString();
        LocalPlayer.Instance.UpdateLocalHighScore();
    }
    public void SetPlacingText(int placing) {
        string placementSuffix = "";
        if (LocalizationSettings.SelectedLocale == LocalizationSettings.AvailableLocales.GetLocale("en")) {
            placementSuffix = "th";
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
            placementTMP.text = $"{placing}{placementSuffix}";
        }
        placementTMP.text = $"{placing}{placementSuffix}";
    }
    public void ButtonValidateClassic() {
        if (sudoku.GetWrong() != 0) {
            checkerTMP.color = Color.magenta;
            checkerTMP.GetComponent<LocalizeStringEvent>().StringReference.TableEntryReference = "Incorrect";
        }
        else {
            checkerTMP.color = Color.cyan;
            checkerTMP.GetComponent<LocalizeStringEvent>().StringReference.TableEntryReference = "Correct";
            EndGame();
        }
    }
    public void ButtonValidateRoyale() {
        Royale royale = (Royale)sudoku;
        if (royale.GetWrong() != 0) {
            checkerTMP.color = Color.magenta;
            checkerTMP.GetComponent<LocalizeStringEvent>().StringReference.TableEntryReference = "Incorrect";
        }
        else {
            checkerTMP.color = Color.cyan;
            checkerTMP.GetComponent<LocalizeStringEvent>().StringReference.TableEntryReference = "Correct";
            royale.AfterCorrectButtonValidate();
        }
    }
    public void ClearCheckerText() {
        checkerTMP.GetComponent<LocalizeStringEvent>().StringReference.TableEntryReference = "Empty";
    }
    public void SetTimeSpent(bool solo) {
        ClearCheckerText();
        if (solo) {
            checkerTMP.text += Timer.Instance.GetElapsedTime();
            placementTMP.text = Timer.Instance.GetElapsedTime();
        }
        else {
            checkerTMP.text += Timer.Instance.GetElapsedTime();
        }
    }

    private void setWinText(bool win) {
        if (win) {
            checkerTMP.color = Color.cyan;
            endHeaderTMP.GetComponent<LocalizeStringEvent>().StringReference.TableEntryReference = "Victory";
            endHeaderTMP.color = new Color32(132, 250, 255, 255);
        }
        else {
            checkerTMP.color = Color.red;
            endHeaderTMP.GetComponent<LocalizeStringEvent>().StringReference.TableEntryReference = "Defeat";
            endHeaderTMP.color = Color.red;
        }
    }
}
