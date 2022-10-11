using System.Collections;
using System.Collections.Generic;
using System.Linq;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class Sudoku : MonoBehaviour {

    // sudokun koko & puuttuvat numerot & N neliöjuuri
    [SerializeField] protected int N = 9, K = 9, SRN;
    // sudoku correct answers
    protected static int[,] answers; 
    // sudoku incomplete answers
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

    void Update() {
        if (currentSudokuNumber != null) {
            for (int i = 0; i < keyCodes.Length; i++) {
                if (Input.GetKeyDown(keyCodes[i])) {
                    int numberPressed = i + 1;
                    if (currentSudokuNumber.Val != numberPressed) {
                        currentSudokuNumber.setNumber(numberPressed);
                        setOne(currentSudokuNumber.Row, currentSudokuNumber.Col, currentSudokuNumber.Val);
                    }
                }
            }
        }
    }

    public void initializeGame() {
        allGamePoints = 0;
        ManageGameSession.Instance.ClearCheckerText();
        answers = new int[N, N];
        values = new int[N, N];
        numbers = new SudokuNumber[N*N];
        SRN = (int)Mathf.Sqrt(N);
        Transform[] ts = SudokuGridPanel.GetComponentsInChildren<Transform>();
        grids = ts.Skip(1).ToArray();
        grids = ts.Skip(1).ToArray();

        generateSudoku();

        //Make fields
        setControls();
        setNumbers();
    }
    
    //Generaattori
    protected void generateSudoku() {
        // Fill the diagonal of SRN x SRN matrices : fillDiagonal
        for (int i = 0; i < N; i += SRN) { createMatrix(i, i); }

        // Fill remaining
        generateRemaining(0, SRN);

        // Remove K
        removeDigits();
    }

    // Fill a 3 x 3 matrix.
    protected void createMatrix(int row, int col) {
        int num;
        for (int i = 0; i < SRN; i++) {
            for (int j = 0; j < SRN; j++) {
                do { num = randGen(N); }
                while (!notInBox(row, col, num));
                answers[row + i, col + j] = num;
            }
        }
    }

    // Täyttää
    protected bool generateRemaining(int i, int j) {
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
                if (generateRemaining(i, j + 1)) return true;

                answers[i, j] = 0; //tä ei tapahdu lähes ikinä
            }
        }
        return false;
    }

    // Remove K num of digits
    public void removeDigits() {
        values = answers.Clone() as int[,];
        int count = K;

        while (count != 0) {
            int remFrom = randGen(N * N) - 1,
                i = (remFrom / N),
                j = remFrom % N;

            if (j != 0) j--;

            if (values[i, j] > 0) {
                count--; values[i, j] = 0;
            }
        }
    }

    protected void setControls() {
        controls = new ControlObj[N];
        for (int i = 1; i <= N; ++i){
            GameObject controlInstance = GameObject.Instantiate(NumContPrefab, NumContPanel.transform);
            ControlObj cobj = new ControlObj(controlInstance, i);
            cobj.instance.GetComponent<Button>().onClick.AddListener(() => onClick_Control(cobj));
            cobj.instance.GetComponentInChildren<TextMeshProUGUI>().color = Color.blue;
            controls[i-1] = cobj;
        }
    }
    
    protected void setNumbers() {
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
                int r = row - 1, c = col - 1, v = getOne(r, c, values), a = getOne(r, c, answers);
                SudokuNumber sn;
                if (numbers[numberId] == null) {
                    sn = new SudokuNumber(GameObject.Instantiate(FieldPrefab, curGrid), r, c, v, a);
                } else {
                    numbers[numberId].setAll(r, c, v, a);
                    sn = numbers[numberId];
                }

                if (v == 0) {
                    sn.instance.GetComponent<Button>().onClick.AddListener(() => onClick_Number(sn));
                    sn.enable();
                } else {
                    sn.disable();
                }
                numbers[numberId] = sn;
                numberId++;
            }
        }
    }

    protected void onClick_Number(SudokuNumber sn) {
        ManageGameSession.Instance.ClearCheckerText();
        
        Debug.Log($"clicked: " + sn.ToString());
        if (currentSudokuNumber != null) {currentSudokuNumber.unsetSelect();}
        currentSudokuNumber = sn;
        currentSudokuNumber.setSelect();
    }

    protected void onClick_Control(ControlObj cobj) {
        ManageGameSession.Instance.ClearCheckerText();
        if (currentControl != null) { currentControl.unsetSelect();}
        currentControl = cobj;
        currentControl.setSelect();

        if (currentSudokuNumber != null && currentSudokuNumber.Val != currentControl.Val) {
            currentSudokuNumber.setNumber(currentControl.Val);
            setOne(currentSudokuNumber.Row, currentSudokuNumber.Col, currentSudokuNumber.Val);
        }
    }

    // Check if safe to put in cell
    protected bool SafeCheck(int i, int j, int num) {
        return (notInRow(i, num) && notInCol(j, num) && notInBox(i - i % SRN, j - j % SRN, num));
    }


    // False if the 3*3 block contains number
    protected bool notInBox(int rowStart, int colStart, int num) {
        for (int i = 0; i < SRN; i++)
            for (int j = 0; j < SRN; j++)
                if (answers[rowStart + i, colStart + j] == num) return false;
        return true;
    }

    // False if the column contains number
    protected bool notInCol(int j, int num) {
        for (int i = 0; i < N; i++) if (answers[i, j] == num) return false;
        return true;
    }

    // False if the row contains number
    protected bool notInRow(int i, int num) {
        for (int j = 0; j < N; j++) if (answers[i, j] == num) return false;
        return true;
    }

    // Random gen
    protected int randGen(int num) { return Random.Range(1, (num + 1)); }

    protected int getOne(int i, int j, int[,] arr) {
        return arr[i, j];
    }

    protected SudokuNumber getSudokuNumber(int i, int j) {
        foreach (SudokuNumber sn in numbers) {
            if (sn.Row == i && sn.Col == j) {
                return sn;
            }
        }
        return null;
    }

    protected void setOne(int i, int j, int value) { values[i, j] = value; }

    public int getWrong() {
        int x = 0;
        foreach (SudokuNumber number in numbers) {
            if (!number.Correct){
                x++;
            }
        }
        return x;
    }

    public int getPoints() {
        return K - getWrong();
    }

    public virtual void sudokuEnd() {
        //stop
        foreach (SudokuNumber number in numbers) {
            number.disableOnly();
        }
        if (currentSudokuNumber != null) currentSudokuNumber.unsetSelect();
        NumContPanel.SetActive(false);

        //check result
        // if (getWrong() != 0) {
        //     ManageGameSession.Instance.LoseGame();
        // } else {
        //     ManageGameSession.Instance.WinGame();
        // }
        //points
        allGamePoints += getPoints();
        //set endscreen info & checker time
        ManageGameSession.Instance.SetTimeSpent(true);
        ManageGameSession.Instance.UpdateScore((ulong)allGamePoints);
        //give points
        // ManageGameSession.Instance.AddPoints((ulong)allGamePoints);
    }
}

