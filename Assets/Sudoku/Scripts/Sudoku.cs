using System.Collections;
using System.Collections.Generic;
using System.Linq;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class Sudoku : MonoBehaviour {

    // sudokun koko & puuttuvat numerot & N neliöjuuri
    [SerializeField] protected  int N = 9, K = 9;
    private int SRN;
    // sudoku correct answers
    protected static int[,] answers; 
    // sudoku incomplete answers
    protected static int[,] values;

    protected static int allGamePoints;

    protected SudokuNumber currentSudokuNumber;
    protected ControlObj currentControl;
    protected SudokuNumber[] numbers;
    protected ControlObj[] controls;
    [SerializeField] protected GameObject EndScreen;
    [SerializeField] protected TextMeshProUGUI endTitleTMP, placementTMP, 
                                                scoreTMP, user;
    
    
    [SerializeField] protected Transform[] grids;
    protected KeyCode[] keyCodes = { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, 
                                    KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6, 
                                    KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9 };

    [SerializeField] protected GameObject SudokuGridPanel, FieldPrefab, 
                                    NumContPanel, NumContPrefab, ButPanel;

    [SerializeField] protected TextMeshProUGUI checkerTMP;

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
        //LocalPlayer.Instance.ResetScore();
        checkerTMP.text = "";
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

    public void newPuzzle() {
        currentSudokuNumber.disable();
        currentSudokuNumber = null;
        //checkerTMP.text = "";
        answers = new int[N, N];
        values = new int[N, N];
        SRN = (int)Mathf.Sqrt(N);

        generateSudoku();

        //Set numbers again
        setNumbers();
    }
    
    //Generaattori
    private void generateSudoku() {
        // Fill the diagonal of SRN x SRN matrices : fillDiagonal
        for (int i = 0; i < N; i += SRN) { createMatrix(i, i); }

        // Fill remaining
        generateRemaining(0, SRN);

        // Remove K
        removeDigits();
    }

    // Fill a 3 x 3 matrix.
    void createMatrix(int row, int col) {
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
    bool generateRemaining(int i, int j) {
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

    private void setControls() {
        controls = new ControlObj[N];
        for (int i = 1; i <= N; ++i){
            GameObject controlInstance = GameObject.Instantiate(NumContPrefab, NumContPanel.transform);
            ControlObj cobj = new ControlObj(controlInstance, i);
            cobj.instance.GetComponent<Button>().onClick.AddListener(() => onClick_Control(cobj));
            cobj.instance.GetComponentInChildren<TextMeshProUGUI>().color = Color.blue;
            controls[i-1] = cobj;
        }
    }
    
    private void setNumbers() {
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

    private void onClick_Number(SudokuNumber sn) {
        checkerTMP.text = "";
        
        Debug.Log($"clicked: " + sn.ToString());
        if (currentSudokuNumber != null) {currentSudokuNumber.unsetSelect();}
        currentSudokuNumber = sn;
        currentSudokuNumber.setSelect();
    }

    private void onClick_Control(ControlObj cobj) {
        checkerTMP.text = "";
        if (currentControl != null) { currentControl.unsetSelect();}
        currentControl = cobj;
        currentControl.setSelect();

        if (currentSudokuNumber != null && currentSudokuNumber.Val != currentControl.Val) {
            currentSudokuNumber.setNumber(currentControl.Val);
            setOne(currentSudokuNumber.Row, currentSudokuNumber.Col, currentSudokuNumber.Val);
        }
    }

    // Check if safe to put in cell
    bool SafeCheck(int i, int j, int num) {
        return (notInRow(i, num) && notInCol(j, num) && notInBox(i - i % SRN, j - j % SRN, num));
    }


    // False if the 3*3 block contains number
    bool notInBox(int rowStart, int colStart, int num) {
        for (int i = 0; i < SRN; i++)
            for (int j = 0; j < SRN; j++)
                if (answers[rowStart + i, colStart + j] == num) return false;
        return true;
    }

    // False if the column contains number
    bool notInCol(int j, int num) {
        for (int i = 0; i < N; i++) if (answers[i, j] == num) return false;
        return true;
    }

    // False if the row contains number
    bool notInRow(int i, int num) {
        for (int j = 0; j < N; j++) if (answers[i, j] == num) return false;
        return true;
    }

    // Random gen
    int randGen(int num) { return Random.Range(1, (num + 1)); }

    private int getOne(int i, int j, int[,] arr) {
        return arr[i, j];
    }

    private SudokuNumber getSudokuNumber(int i, int j) {
        foreach (SudokuNumber sn in numbers) {
            if (sn.Row == i && sn.Col == j) {
                return sn;
            }
        }
        return null;
    }

    private void setOne(int i, int j, int value) { values[i, j] = value; }

    private string arrtoS(int[,] a) {
        string s = "";
        for (int i = 0; i < N; i++) {
            for (int j = 0; j < N; j++) {
                s += a[i, j].ToString();
            }
        }
        return s;
    }

    public int wrong() {
        int x = 0;
        foreach (SudokuNumber number in numbers) {
            if (!number.Correct){
                x++;
            }
        }
        return x;
    }

    public virtual void validate(bool button) {
        allGamePoints = K - wrong();
        string result = "";
        if (wrong() != 0) {
            checkerTMP.color = Color.magenta;
            result = "Incorrect!";
        } else {
            checkerTMP.color = Color.cyan;
            result = "Correct!";
        }
        checkerTMP.text = result;
        //if stopping case
        if (wrong() == 0 || (!button && wrong() != 0)){
            //stop the game
            Timer2.Instance.StopTimer();
            foreach (SudokuNumber number in numbers) {
                number.disableOnly();
            }
            if (currentSudokuNumber != null) currentSudokuNumber.unsetSelect();
            NumContPanel.SetActive(false);
            //set checker time, color & endscreen title color
            checkerTMP.text += "\tTime spent: " + Timer2.Instance.getEnd();
            if (wrong() != 0) {
                checkerTMP.color = Color.magenta;
                endTitleTMP.color = Color.magenta;
            } else {
                //LocalPlayer.Instance.IncrementVictories();
                checkerTMP.color = Color.cyan;
                endTitleTMP.color = Color.cyan;//new Color32(132, 250, 255, 255);
            }
            //set endscreen info
            endTitleTMP.text = result;
            placementTMP.text = "Time spent (m:s:ms): " + Timer2.Instance.getEnd();
            scoreTMP.text = "Solved: " + allGamePoints + " numbers";
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
}

