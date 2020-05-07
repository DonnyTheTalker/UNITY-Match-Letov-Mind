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
        MoveTile(this);
    }

    public void MoveTile(Tile tile)
    {
        Vector2 dir = new Vector2(tile.Column, tile.Row) - new Vector2(tile.transform.position.x, tile.transform.position.y);

        if (Mathf.Abs(dir.magnitude) > 0.003f) {
            var tempPos = new Vector2(tile.Column, tile.Row);
            tile.transform.position = Vector2.Lerp(tile.transform.position, tempPos, tile.MovingSpeed * Time.deltaTime);
        } else {
            tile.transform.position = new Vector3(tile.Column, tile.Row, 0f);
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
        StartCoroutine(SwapTiles(finalX, finalY));

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

    private IEnumerator SwapTiles(int finalX, int finalY)
    {
        int tempX = Column, tempY = Row;

        if (!(finalY < 0 || finalX < 0 || finalX >= _board.Width || finalY >= _board.Height)) {

            if (_board.Indexes[finalX, finalY] == -1) {
                yield return null;
            } else {

                _board.Tiles[Column, Row] = _board.Tiles[finalX, finalY];
                _board.Tiles[finalX, finalY] = this.gameObject;

                int temp = _board.Indexes[Column, Row];
                _board.Indexes[Column, Row] = _board.Indexes[finalX, finalY];
                _board.Indexes[finalX, finalY] = temp;

                _board.Tiles[Column, Row].GetComponent<Tile>().SetPos(Row, Column);
                SetPos(finalY, finalX);

                yield return new WaitForSeconds(0.3f);

                if (!_board.CanFindMatch()) {
                    _board.Tiles[finalX, finalY] = _board.Tiles[tempX, tempY];
                    _board.Tiles[tempX, tempY] = this.gameObject;

                    temp = _board.Indexes[tempX, tempY];
                    _board.Indexes[tempX, tempY] = _board.Indexes[finalX, finalY];
                    _board.Indexes[finalX, finalY] = temp;

                    SetPos(tempY, tempX);
                    _board.Tiles[finalX, finalY].GetComponent<Tile>().SetPos(finalY, finalX);
                } else {
                    _board.DestroyAllMatches();
                    StartCoroutine(_board.FillEmptyTiles());
                }
            }
        } else {
            yield return null;
        }

    }

}
