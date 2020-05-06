using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private Vector2 _firstTouchPos;
    private Vector2 _finalTouchPos;
    private Board _board;

    public int Row;
    public int Column;
    public float MovingSpeed = 8f;

    private void Update()
    {
        Vector2 dir = new Vector2(Column, Row) - new Vector2(transform.position.x, transform.position.y);

        if (Mathf.Abs(dir.magnitude) > 0.003f) {
            var tempPos = new Vector2(Column, Row);
            transform.position = Vector2.Lerp(transform.position, tempPos, MovingSpeed * Time.deltaTime);
        } else {
            transform.position = new Vector3(Column, Row, 0f);
        }

    }

    private void Start()
    {
        _board = FindObjectOfType<Board>();
        Row = (int)transform.position.y;
        Column = (int)transform.position.x;
    }

    public void SetPos(int row, int col)
    {
        Row = row;
        Column = col;
    }

    private void OnMouseDown()
    {
        _firstTouchPos = GetCameraPos();
    }

    private void OnMouseUp()
    {
        _finalTouchPos = GetCameraPos();

        int finalX = (int)Mathf.Round(_finalTouchPos.x);
        int finalY = (int)Mathf.Round(_finalTouchPos.y);

        if (Column == finalX && Row == finalY)
            return;

        GetMovedPos(ref finalX, ref finalY);
        SwapTiles(finalX, finalY);

    }

    private Vector2 GetCameraPos()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void GetMovedPos(ref int finalX, ref int finalY)
    {
        if (Column < finalX) {
            finalX = Column + 1;
            finalY = Row;
        } else if (Column > finalX) {
            finalX = Column - 1;
            finalY = Row;
        } else if (Row < finalY) {
            finalY = Row + 1;
            finalX = Column;
        } else {
            finalY = Row - 1;
            finalX = Column;
        }
    }

    private void SwapTiles(int finalX, int finalY)
    {
        if (finalY < 0 || finalX < 0 || finalX >= _board.Width || finalY >= _board.Height)
            return;

        _board.Tiles[Column, Row] = _board.Tiles[finalX, finalY];
        _board.Tiles[finalX, finalY] = this.gameObject;

        _board.Tiles[Column, Row].GetComponent<Tile>().SetPos(Row, Column);
        SetPos(finalY, finalX);

    }

}
