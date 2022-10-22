using System.Collections;
using System.Collections.Generic;
using System.Linq;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class Sudoku : MonoBehaviour {

    // N sudokun koko, K puuttuvat numerot, SRN neliöjuuri N:stä
    public int N = 9, K = 9, SRN;
    // sudoku oikeat vastaukset
    protected static int[,] answers; 
    // sudoku annetut vastaukset
    protected static int[,] values;

    protected static int allGamePoints;

    protected SudokuNumber currentSudokuNumber;
    protected ControlObj currentControl;
    protected SudokuNumber[] numbers;
    protected ControlObj[] controls;
    
    [SerializeField] protected Transform[] grids;
    protected KeyCode[] keyCodes = { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, 
                                    KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6, 
                                    KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9 };

    [SerializeField] protected GameObject SudokuGridPanel, FieldPrefab, 
                                    NumContPanel, NumContPrefab, ButPanel;
    
    // Update() -metodissa vaihdetaan sudoku numeroiden arvoja näppäimistöllä
    void Update() {
        if (currentSudokuNumber != null) {
            for (int i = 0; i < keyCodes.Length; i++) {
                if (Input.GetKeyDown(keyCodes[i])) {
                    int numberPressed = i + 1;
                    if (currentSudokuNumber.Val != numberPressed) {
                        currentSudokuNumber.SetNumber(numberPressed);
                        SetOne(currentSudokuNumber.Row, currentSudokuNumber.Col, currentSudokuNumber.Val);
                    }
                }
            }
        }
    }

    // InitializeGame() -metodilla alustetaan uusi sudoku taulu
    public void InitializeGame() {
        allGamePoints = 0;
        ManageGameSession.Instance.ClearCheckerText();
        answers = new int[N, N];
        values = new int[N, N];
        numbers = new SudokuNumber[N*N];
        SRN = (int)Mathf.Sqrt(N);
        Transform[] ts = SudokuGridPanel.GetComponentsInChildren<Transform>();
        grids = ts.Skip(1).ToArray();
        grids = ts.Skip(1).ToArray();

        GenerateSudoku();
        SetControls();
        SetNumbers();
    }
    
    // GenerateSudoku() -metodilla generoidaan sudoku numerot kutsumalla metodeja
    protected void GenerateSudoku() {
        // Fill the diagonal of SRN x SRN matrices : fillDiagonal
        for (int i = 0; i < N; i += SRN) { CreateMatrix(i, i); }

        // Fill remaining
        GenerateRemaining(0, SRN);

        // Remove K
        RemoveDigits();
    }

    // CreateMatrix(int row, int col) -metodi luo koon neliöjuuren verran numeroita matrixiin
    protected void CreateMatrix(int row, int col) {
        int num;
        for (int i = 0; i < SRN; i++) {
            for (int j = 0; j < SRN; j++) {
                do { num = RandGen(N); }
                while (!NotInBox(row, col, num));
                answers[row + i, col + j] = num;
            }
        }
    }

    // GenerateRemaining(int i, int j) -metodi luo loput numerot
    protected bool GenerateRemaining(int i, int j) {
        if (j >= N && i < N - 1) { i++; j = 0; }

        if (i >= N && j >= N) return true;

        if (i < SRN && j < SRN){
            j = SRN;
        } else if (i < N - SRN) {
            if (j == (int)(i / SRN) * SRN) j += SRN;
        } else {
            if (j == N - SRN) {
                i++; j = 0;
                if (i >= N) return true;
            }
        }

        for (int num = 1; num <= N; num++) {
            if (SafeCheck(i, j, num)) {
                answers[i, j] = num;
                if (GenerateRemaining(i, j + 1)) return true;

                answers[i, j] = 0;
            }
        }
        return false;
    }

    // RemoveDigits() -metodi poistaa K:n verran numeroita
    public void RemoveDigits() {
        values = answers.Clone() as int[,];
        int count = K;

        while (count != 0) {
            int remFrom = RandGen(N * N) - 1,
                i = (remFrom / N),
                j = remFrom % N;

            if (j != 0) j--;

            if (values[i, j] > 0) {
                count--; values[i, j] = 0;
            }
        }
    }

    // SetControls() -metodi asettaa kontrollit
    protected void SetControls() {
        controls = new ControlObj[N];
        for (int i = 1; i <= N; ++i){
            GameObject controlInstance = GameObject.Instantiate(NumContPrefab, NumContPanel.transform);
            ControlObj cobj = new ControlObj(controlInstance, i);
            cobj.instance.GetComponent<Button>().onClick.AddListener(() => OnClick_Control(cobj));
            cobj.instance.GetComponentInChildren<TextMeshProUGUI>().color = Color.blue;
            controls[i-1] = cobj;
        }
    }

    // SetNumbers() -metodi asettaa numerot tauluihin
    protected void SetNumbers() {
        Transform curGrid = grids[0];
        int numberId = 0;
        for (int row = 1; row <= N; row++) {
            for (int col = 1; col <= N; col++) {
                if (row <= 3) {
                    if (col <= 3)
                        curGrid = grids[0];
                    else if (4 <= col && col <= 6)
                        curGrid = grids[1];
                    else if (7 <= col && col <= 9)
                        curGrid = grids[2];
                }
                else if (row <= 6) {
                    if (col <= 3)
                        curGrid = grids[3];
                    else if (4 <= col && col <= 6)
                        curGrid = grids[4];
                    else if (7 <= col && col <= 9)
                        curGrid = grids[5];
                }
                else if (row <= 9) {
                    if (col <= 3)
                        curGrid = grids[6];
                    else if (4 <= col && col <= 6)
                        curGrid = grids[7];
                    else if (7 <= col && col <= 9)
                        curGrid = grids[8];
                }
                //row, col, value, answer for a number obj
                int r = row - 1, c = col - 1, v = GetOne(r, c, values), a = GetOne(r, c, answers);
                SudokuNumber sn;
                if (numbers[numberId] == null) {
                    sn = new SudokuNumber(GameObject.Instantiate(FieldPrefab, curGrid), r, c, v, a);
                } else {
                    numbers[numberId].SetAll(r, c, v, a);
                    sn = numbers[numberId];
                }

                if (v == 0) {
                    sn.instance.GetComponent<Button>().onClick.AddListener(() => OnClick_Number(sn));
                    sn.Enable();
                } else {
                    sn.Disable();
                }
                numbers[numberId] = sn;
                numberId++;
            }
        }
    }

    // OnClick_Number(SudokuNumber sn) -metodi toimii klikki kuuntelijana sudoku numeroille
    protected void OnClick_Number(SudokuNumber sn) {
        ManageGameSession.Instance.ClearCheckerText();
        Trace.Log($"clicked: " + sn.ToString());
        if (currentSudokuNumber != null) {currentSudokuNumber.UnsetSelect();}
        currentSudokuNumber = sn;
        currentSudokuNumber.SetSelect();
    }

    // OnClick_Control(ControlObj cobj) -metodi toimii klikki kuuntelijana kontrolli nappuloille
    protected void OnClick_Control(ControlObj cobj) {
        ManageGameSession.Instance.ClearCheckerText();
        if (currentControl != null) { currentControl.UnsetSelect();}
        currentControl = cobj;
        currentControl.SetSelect();

        if (currentSudokuNumber != null && currentSudokuNumber.Val != currentControl.Val) {
            currentSudokuNumber.SetNumber(currentControl.Val);
            SetOne(currentSudokuNumber.Row, currentSudokuNumber.Col, currentSudokuNumber.Val);
        }
    }

    // SafeCheck(int i, int j, int num) -boolean tarkistaa että numeron voi asettaa taulukkoon
    protected bool SafeCheck(int i, int j, int num) {
        return (NotInRow(i, num) && NotInCol(j, num) && NotInBox(i - i % SRN, j - j % SRN, num));
    }

    // NotInBox(int rowStart, int colStart, int num) -boolean tarkistaa että numero ei ole SRN*SRN taulukossa joka luotiin CreateMatrix()
    protected bool NotInBox(int rowStart, int colStart, int num) {
        for (int i = 0; i < SRN; i++)
            for (int j = 0; j < SRN; j++)
                if (answers[rowStart + i, colStart + j] == num) return false;
        return true;
    }

    // NotInCol(int j, int num) -boolean tarkistaa että numero ei ole kolumnissa
    protected bool NotInCol(int j, int num) {
        for (int i = 0; i < N; i++) if (answers[i, j] == num) return false;
        return true;
    }

    // NotInRow(int j, int num) -boolean tarkistaa että numero ei ole rivissä
    protected bool NotInRow(int i, int num) {
        for (int j = 0; j < N; j++) if (answers[i, j] == num) return false;
        return true;
    }

    // RandGen(int num) -integer luo satunnaisen numeron
    protected int RandGen(int num) { return Random.Range(1, (num + 1)); }

    // GetOne(int i, int j, int[,] arr) -integer palauttaa yhden arvon taulukosta
    protected int GetOne(int i, int j, int[,] arr) {
        return arr[i, j];
    }

    // GetSudokuNumber(int i, int j) -SudokuNumber palauttaa yhden olion taulukosta
    protected SudokuNumber GetSudokuNumber(int i, int j) {
        foreach (SudokuNumber sn in numbers) {
            if (sn.Row == i && sn.Col == j) {
                return sn;
            }
        }
        return null;
    }

    // SetOne(int i, int j, int value) -metodi asettaa yhden arvon taulukosta
    protected void SetOne(int i, int j, int value) { values[i, j] = value; }

    // SetOne(int i, int j, int value) -metodi asettaa yhden arvon taulukosta
    public int GetWrong() {
        int x = 0;
        foreach (SudokuNumber number in numbers) {
            if (!number.Correct){
                x++;
            }
        }
        return x;
    }

    //GetPoints() -integer palauttaa
    public int GetPoints() {
        return K - GetWrong();
    }

    //SudokuEnd() -metodi kutsutaan pelin lopuksi ManageGameSession -luokasta
    public virtual void SudokuEnd() {
        foreach (SudokuNumber number in numbers) {
            number.DisableOnly();
        }
        if (currentSudokuNumber != null) currentSudokuNumber.UnsetSelect();

        NumContPanel.SetActive(false);
        allGamePoints += GetPoints();
        // Aseta checker aika & päivitä score
        ManageGameSession.Instance.SetTimeSpent(true);
        ManageGameSession.Instance.UpdateScore((ulong)allGamePoints);
    }
}

