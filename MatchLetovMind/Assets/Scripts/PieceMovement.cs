using System.Collections;
using System.Collections.Generic; 
using UnityEngine;

public class PieceMovement : MonoBehaviour
{
    public static PieceMovement Instance;
    private GameManager _gameManager;

    private NodePiece _movingPiece;
    private Point _newIndex;
    private Vector2 _mouseStart;

    private Point add;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _gameManager = GetComponent<GameManager>();
    }

    private void Update()
    {
        if (_movingPiece != null) {
            Vector2 dir = (Vector2)Input.mousePosition - _mouseStart;
            Vector2 dirNormalized = dir.normalized;
            Vector2 absDir = new Vector2(Mathf.Abs(dir.x), Mathf.Abs(dir.y));

            _newIndex = Point.Clone(_movingPiece.Index);
            add = Point.Zero;

            if (dir.magnitude > GameManager.XOffset) {

                if (absDir.x > absDir.y)
                    add = new Point((dirNormalized.x > 0) ? 1 : -1, 0);
                else
                    add = new Point(0, (dirNormalized.y) > 0 ? -1 : 1);

            }

            _newIndex = Point.Add(_newIndex, add);
            Vector2 newPos = GameManager.GetPositionFromPoint(_movingPiece.Index);

            if (!Point.Equals(_newIndex, _movingPiece.Index)) {
                newPos += new Vector2(add.X, -add.Y) * 10;
            }

            _movingPiece.MovePositionTo(newPos);

        }
    }

    public void MovePiece(NodePiece piece)
    {
        if (_movingPiece != null) return;
        _movingPiece = piece;
        _mouseStart = Input.mousePosition;
    }

    public void DropPiece()
    {
        if (_movingPiece == null) return;

        if (!Point.Equals(_newIndex, _movingPiece.Index)) {
            _movingPiece.MovePosition(GameManager.GetPositionFromPoint(_movingPiece.Index) + new Vector2(add.X, -add.Y) * 10);
            _gameManager.SwapPieces(_movingPiece.Index, _newIndex); 
            _movingPiece = null;
            return;
        }

        _movingPiece.ResetPiece();
        _movingPiece = null;
    }
    
}
