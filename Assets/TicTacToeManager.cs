// TicTacToeManager.cs - VR Left/Right Trigger Version
using UnityEngine;

public class TicTacToeManager : MonoBehaviour
{
    public enum Player { None, X, O }

    [Header("Game State")]
    private Player[,] grid = new Player[3, 3];
    private bool gameEnded = false;

    [Header("Prefabs")]
    public GameObject XPrefab;
    public GameObject OPrefab;

    [Header("Board Settings")]
    public float pieceYOffset = 0.1f; // Height offset for pieces

    [Header("Turn Settings")]
    public bool allowBothPlayers = true; // If false, alternates turns. If true, allows either trigger anytime

    // Turn tracking (only used if allowBothPlayers is false)
    private Player currentPlayer = Player.X;

    void Start()
    {
        Debug.Log("VR Tic-Tac-Toe game started.");
        if (!allowBothPlayers)
        {
            Debug.Log("Turn-based mode. Current player: " + currentPlayer);
        }
        else
        {
            Debug.Log("Free-play mode. Left trigger = X, Right trigger = O");
        }
    }

    public void MakeMoveWithTrigger(int row, int col, Vector3 squarePosition, bool isLeftTrigger)
    {
        // Check if game has ended
        if (gameEnded)
        {
            Debug.Log("Game has already ended!");
            return;
        }

        // Check if square is already occupied
        if (grid[row, col] != Player.None)
        {
            Debug.Log($"Square [{row}, {col}] is already occupied by {grid[row, col]}");
            return;
        }

        // Determine which player based on trigger
        Player playerToPlace = isLeftTrigger ? Player.X : Player.O;

        // If turn-based mode is enabled, check if it's the right player's turn
        if (!allowBothPlayers && playerToPlace != currentPlayer)
        {
            Debug.Log($"It's not {playerToPlace}'s turn! Current turn: {currentPlayer}");
            return;
        }

        // Make the move
        grid[row, col] = playerToPlace;
        Debug.Log($"Player {playerToPlace} placed at [{row}, {col}] using {(isLeftTrigger ? "left" : "right")} trigger");

        // Spawn the appropriate prefab at the square's position
        Vector3 spawnPosition = new Vector3(squarePosition.x, squarePosition.y + pieceYOffset, squarePosition.z);
        GameObject prefabToSpawn = (playerToPlace == Player.X) ? XPrefab : OPrefab;

        if (prefabToSpawn != null)
        {
            GameObject spawnedPiece = Instantiate(prefabToSpawn, spawnPosition, Quaternion.Euler(-90, 0, 0));
            // Add tag for easy cleanup during reset
            spawnedPiece.tag = "GamePiece";
        }
        else
        {
            Debug.LogError($"Prefab for player {playerToPlace} is not assigned!");
        }

        // Check for win condition
        if (CheckWin(playerToPlace))
        {
            Debug.Log($"Player {playerToPlace} wins!");
            gameEnded = true;
            // TODO: Trigger win UI
            return;
        }

        // Check for draw
        if (IsBoardFull())
        {
            Debug.Log("It's a draw!");
            gameEnded = true;
            // TODO: Trigger draw UI
            return;
        }

        // Switch to next player (only in turn-based mode)
        if (!allowBothPlayers)
        {
            currentPlayer = (currentPlayer == Player.X) ? Player.O : Player.X;
            Debug.Log($"Current player is now: {currentPlayer}");
        }
    }

    private bool IsBoardFull()
    {
        foreach (var cell in grid)
        {
            if (cell == Player.None)
                return false;
        }
        return true;
    }

    private bool CheckWin(Player player)
    {
        // Check rows
        for (int i = 0; i < 3; i++)
        {
            if (grid[i, 0] == player && grid[i, 1] == player && grid[i, 2] == player)
                return true;
        }

        // Check columns
        for (int i = 0; i < 3; i++)
        {
            if (grid[0, i] == player && grid[1, i] == player && grid[2, i] == player)
                return true;
        }

        // Check diagonals
        if (grid[0, 0] == player && grid[1, 1] == player && grid[2, 2] == player)
            return true;
        if (grid[0, 2] == player && grid[1, 1] == player && grid[2, 0] == player)
            return true;

        return false;
    }

    [ContextMenu("Reset Game")]
    public void ResetGame()
    {
        // Clear the grid
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                grid[i, j] = Player.None;
            }
        }

        // Reset game state
        currentPlayer = Player.X;
        gameEnded = false;

        // Destroy all spawned pieces
        GameObject[] pieces = GameObject.FindGameObjectsWithTag("GamePiece");
        foreach (GameObject piece in pieces)
        {
            DestroyImmediate(piece);
        }

        Debug.Log("Game reset!");

        if (!allowBothPlayers)
        {
            Debug.Log("Current player: " + currentPlayer);
        }
    }

    // Public getter for UI or other systems
    public Player GetCurrentPlayer()
    {
        return currentPlayer;
    }

    public bool IsGameEnded()
    {
        return gameEnded;
    }
}