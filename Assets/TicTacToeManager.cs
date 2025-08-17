using UnityEngine;

public class TicTacToeManager : MonoBehaviour
{
    public enum Player { None, X, O }
    private Player[,] grid = new Player[3, 3];
    private Player currentPlayer = Player.X;

    public GameObject XPrefab;
    public GameObject OPrefab;

    public void MakeMove(int row, int col)
    {
        if (grid[row, col] != Player.None) return;

        grid[row, col] = currentPlayer;

        //find world position of the square
        Vector3 pos = new Vector3(row, 0.1f, col); // adjust numbers to match board
        Instantiate(currentPlayer == Player.X ? XPrefab : OPrefab, pos, Quaternion.identity);

        if (CheckWin(currentPlayer))
        {
            Debug.Log($"Player {currentPlayer} wins!");
            // Todo: trigger win ui
        }
        else if (IsBoardFull())
        {
            Debug.Log("it's a draw");
            //todo trigger ui draw
        }
        else
        {
            currentPlayer = (currentPlayer == Player.X) ? Player.O : Player.X;
        }
    }
    
    private bool IsBoardFull()
    {
        foreach (var cell in grid)
            if (cell == Player.None) return false;
        return true;
    }

    private bool CheckWin(Player player)
    {
        for (int i = 0; i < 3; i++)
        {
            if (grid[i, 0] == player && grid[i, 1] == player && grid[i, 2] == player) return true;
            if (grid[0, i] == player && grid[1, i] == player && grid[2, i] == player) return true;
        }
        if (grid[0, 0] == player && grid[1, 1] == player && grid[2, 2] == player) return true;
        if (grid[0, 2] == player && grid[1, 1] == player && grid[2, 0] == player) return true;

        return false;
    }

}
