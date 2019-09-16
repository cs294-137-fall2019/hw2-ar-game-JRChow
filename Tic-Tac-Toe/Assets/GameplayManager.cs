using System;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
using UnityEngine.UI;
using Random = System.Random;

public class GameplayManager : MonoBehaviour
{
    public Text messageText;
    public GameObject gameBoardObj;
    public Button playButton;
    
    private char[][] board = new char[3][];
    
    private bool gameStarted = false;

    private enum GameStateEnum
    {
        InProgress,
        Win,
        Lose,
        Draw,
    }

    private GameStateEnum GameState
    {
        get
        {
            var verticalState = CheckVertical();
            if (verticalState != GameStateEnum.Draw) return verticalState;
        
            var horizontalState = CheckHorizontal();
            if (horizontalState != GameStateEnum.Draw) return horizontalState;

            var diagonalState = CheckDiagonal();
            if (diagonalState != GameStateEnum.Draw) return diagonalState;

            return HasEmptySlot() ? GameStateEnum.InProgress : GameStateEnum.Draw;
        }
    }

    private Random rand = new Random();
    
    public void Initialize()
    {
        for (var i = 0; i < 3; i++)
        {
            board[i] = new char[3];
            for (var j = 0; j < 3; j++)
            {
                board[i][j] = '.';
                SetActiveNoughtObject(i, j, false);
                SetActiveCrossObject(i, j, false);
            }
        }

        gameStarted = true;
        DisplayText("Game Starts!");
    }

    void Update()
    {
        playButton.GetComponentInChildren<Text>().text = gameStarted ? "Restart" : "Play";
    }

    public bool GameStarted()
    {
        return gameStarted;
    }

    public void PutCross(int crossID)
    {
        if (GameState != GameStateEnum.InProgress)
        {
            gameStarted = false;
            return;
        }
        
        var row = crossID / 3;
        var col = crossID % 3;
        if (PosOccupied(row, col)) 
            return;
        board[row][col] = 'X';
        SetActiveCrossObject(row, col, true);
        DisplayGameState();
        PutNought();
    }

    public bool PosOccupied(int r, int c)
    {
        return board[r][c] != '.';
    }

    private void PutNought()
    {
        if (GameState != GameStateEnum.InProgress)
        {
            gameStarted = false;
            return;
        }
            
        var row = rand.Next(0, 3);
        var col = rand.Next(0, 3);
        while (PosOccupied(row, col))
        {
            row = rand.Next(0, 3);
            col = rand.Next(0, 3);
        }

        board[row][col] = 'O';
        SetActiveNoughtObject(row, col, true);
        DisplayGameState();
    }

    private void SetActiveNoughtObject(int row, int col, bool isActive)
    {
        var noughtID = row * 3 + col;
        var noughtName = "Nought " + noughtID;
        var noughtObj = gameBoardObj.transform.Find(noughtName);
        noughtObj.gameObject.SetActive(isActive);
    }

    private void SetActiveCrossObject(int row, int col, bool isActive)
    {
        var crossID = row * 3 + col;
        var crossName = "Cross " + crossID;
        var crossScript = gameBoardObj.transform.Find(crossName).GetComponent<Cross>();
        crossScript.SetCrossActive(isActive);
    }

    /// <summary>
    /// Display prompt text if the game ends
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private void DisplayGameState()
    {
        switch (GameState)
        {
            case GameStateEnum.InProgress:
                // DisplayText("[DEBUG] IN-PROGRESS");
                break;
            case GameStateEnum.Win:
                DisplayText("You Won!");
                break;
            case GameStateEnum.Lose:
                DisplayText("You Lost...");
                break;
            case GameStateEnum.Draw:
                DisplayText("It's a Draw");
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void DisplayText(string txt)
    {
        messageText.gameObject.SetActive(true);
        messageText.text = txt;
    }

    /// <summary>
    /// If there's any '.' in the board, then the game is in progress.
    /// </summary>
    /// <returns></returns>
    private bool HasEmptySlot()
    {
        for (var r = 0; r < 3; r++)
        {
            for (var c = 0; c < 3; c++)
                if (board[r][c] == '.') return true;
        }

        return false;
    }

    private static GameStateEnum CheckLine(char c1, char c2, char c3)
    {
        if (c1 == c2 && c2 == c3 && c3 != '.')
            return c1 == 'X' ? GameStateEnum.Win : GameStateEnum.Lose;
        return GameStateEnum.Draw;
    }

    private GameStateEnum CheckVertical()
    {
        for (var c = 0; c < 3; c++)
        {
            var colState = CheckLine(board[0][c], board[1][c], board[2][c]);
            if (colState != GameStateEnum.Draw) return colState;
        }

        return GameStateEnum.Draw;
    }

    private GameStateEnum CheckHorizontal()
    {
        for (var r = 0; r < 3; r++)
        {
            var rowState = CheckLine(board[r][0], board[r][1], board[r][2]);
            if (rowState != GameStateEnum.Draw) return rowState;
        }

        return GameStateEnum.Draw;
    }

    private GameStateEnum CheckDiagonal()
    {
        var UL2DR = CheckLine(board[0][0], board[1][1], board[2][2]);
        if (UL2DR != GameStateEnum.Draw) return UL2DR;
        
        var DL2UR = CheckLine(board[2][0], board[1][1], board[0][2]);
        return DL2UR != GameStateEnum.Draw ? DL2UR : GameStateEnum.Draw;
    }
}
