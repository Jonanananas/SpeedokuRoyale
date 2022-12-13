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
    /// <summary>
    /// Update the score in the UI and try to update the player's local high score
    /// </summary>
    /// <param name="score"> 
    /// The player's current score.
    /// </param>
    public void UpdateScore(ulong score) {
        currentScoreTMP.text = score.ToString();
        scoreTMP.text = score.ToString();
        LocalPlayer.Instance.UpdateLocalHighScore();
    }
    /// <summary>
    /// Set an UI player placement text with a proper suffix.
    /// </summary>
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
    /// <summary>
    /// Check if a singleplayer sudoku is solved correctly.
    /// </summary>
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
    /// <summary>
    /// Check if a multiplayer sudoku is solved correctly.
    /// </summary>
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
    /// <summary>
    /// Clear the UI text which indicates if a sudoku was correctly solved or not.
    /// </summary>
    public void ClearCheckerText() {
        checkerTMP.GetComponent<LocalizeStringEvent>().StringReference.TableEntryReference = "Empty";
    }
    /// <summary>
    /// Set the amount of time used on solving sudokus displayed in the UI. 
    /// Use <code><paramref name="solo"/>= true</code> to use with singleplayer and
    /// <code><paramref name="solo"/>= false</code> to use with multiplayer.
    /// </summary>
    /// <param name="solo"></param>
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
    /// <summary>
    /// Set an UI end screen text based on if the player won or not.
    /// Use <code><paramref name="win"/>= true</code> if the player won and
    /// <code><paramref name="win"/>= false</code> if the player lost.
    /// </summary>
    /// <param name="win"></param>
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
