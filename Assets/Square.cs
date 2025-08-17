using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Square : MonoBehaviour 
{
    public int row;
    public int col;
    public TicTacToeManager manager;

    public void Init(TicTacToeManager gameManager, int r, int c)
    {
        manager = gameManager;
        row = r;
        col = c;
    }

    public void OnSelected(SelectEnterEvent args)
    {
        manager.MakeMove(row, col);
    }

    //public void OnMouseDown()
    //{
    //    OnSelected();
    //}
}
