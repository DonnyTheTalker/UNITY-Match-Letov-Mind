using System; 
using UnityEngine; 

public class GameManager : MonoBehaviour 
{
    public ArrayLayout BoardLayaout;
    [SerializeField] private int _height = 6;
    [SerializeField] private int _width = 10;
    public static int XOffset = 32;
    public static int YOffset = -32;
    public static int NodeSize = 64; 
    private Node[,] _board;

    private System.Random _random;

    public Sprite[] Pieces;
    public GameObject NodePiecePrefab;
    public RectTransform GameBoard;

    private void Start()
    {
        StartGame();
    }

    void StartGame()
    { 
        _random = new System.Random(GetRandomSeed().GetHashCode());

        InitializeBoard();
        VerifyBoard();
        InstantiateBoard();
    }

    void InitializeBoard()
    {
        _board = new Node[_width, _height];
    
        for (int y = 0; y < _height; y++) {
            for (int x = 0; x < _width; x++) {
                _board[x, y] = new Node(BoardLayaout.Rows[y].Row[x] ? -1 : FillPiece(), new Point(x, y), null);
            }
        }
    }

    void VerifyBoard()
    {
        for (int x = 0; x < _width; x++) {
            for (int y = 0; y < _height; y++)
                ChangeSameLine(new Point(x, y));
        }
    }

    void ChangeSameLine(Point center)
    {
        int left = GetValueAtPoint(Point.Add(center, Point.Left));
        int mid = GetValueAtPoint(center);
        int right = GetValueAtPoint(Point.Add(center, Point.Right));
        int up = GetValueAtPoint(Point.Add(center, Point.Up));
        int down = GetValueAtPoint(Point.Add(center, Point.Down));

        bool hasMultLine = true;

        while (hasMultLine) {
            hasMultLine = false;

            if (center.X > 0 && center.X < _width - 1)
                if (left > 0 && right > 0)
                    while (left == right && right == mid) {
                        hasMultLine = true;
                        SetValueAtPoint(center, FillPiece()); 
                        mid = GetValueAtPoint(center); 
                    }
            
            if (center.Y > 0 && center.Y < _height - 1)
                if (up > 0 && down > 0)
                    while (up == down && down == mid) {
                        hasMultLine = true;
                        SetValueAtPoint(center, FillPiece());
                        mid = GetValueAtPoint(center); 
                    }
            
        }

    }

    void InstantiateBoard()
    {
        for (int x = 0; x < _width; x++) {
            for (int y = 0; y < _height; y++) {

                int val = GetValueAtPoint(x, y);
                if (val <= 0) continue;

                GameObject node = Instantiate(NodePiecePrefab, GameBoard);
                NodePiece nodePiece = node.GetComponent<NodePiece>();  
                nodePiece.Initialize(val, new Point(x, y), Pieces[val - 1]);
                _board[x, y].piece = nodePiece;
            }
        }
    }

    public static Vector2 GetPositionFromPoint(Point p)
    {
        return new Vector2(XOffset + p.X * NodeSize,
            YOffset - p.Y * NodeSize);
    }

    int GetValueAtPoint(Point p)
    {
        if (p.X < 0 || p.X >= _width || p.Y < 0 || p.Y >= _height)
            return -1;
        return _board[p.X, p.Y].Value;
    }

    int GetValueAtPoint(int x, int y)
    {
        if (x < 0 || x >= _width || y < 0 || y >= _height)
            return -1;
        return _board[x, y].Value;
    }

    public void SwapPieces(Point first, Point second)
    {
        Node nodeFirst = GetNodeAtPoint(first);
        Node nodeSecond = GetNodeAtPoint(second);
        
        if (nodeFirst.Value <= 0) return;
        if (nodeSecond.Value <= 0) {
            nodeFirst.piece.ResetPiece();
            return;
        }

        Debug.Log(first.X + " " + first.Y + "   " + second.X + " " + second.Y);

        NodePiece pieceFirst = nodeFirst.piece;
        NodePiece pieceSecond = nodeSecond.piece;

        var temp = new Node(nodeFirst);
        nodeFirst = new Node(nodeSecond);
        nodeSecond = new Node(temp);

        nodeFirst.piece = pieceSecond;
        nodeSecond.piece = pieceFirst;

        _board[first.X, first.Y] = nodeFirst;
        _board[second.X, second.Y] = nodeSecond;

        ResetNodeAtPoint(first);
        ResetNodeAtPoint(second);

        var dir = new Vector2(nodeFirst.piece.Pos.x, nodeFirst.piece.Pos.y);
        nodeFirst.piece.Pos = new Vector2(nodeSecond.piece.Pos.x, nodeSecond.piece.Pos.y);
        nodeSecond.piece.Pos = dir;

        nodeFirst.piece.ResetPiece();
        nodeSecond.piece.ResetPiece();
        nodeFirst.piece.ResetPiece();
        nodeSecond.piece.ResetPiece();

    }

    Node GetNodeAtPoint(Point p)
    {
        return _board[p.X, p.Y];
    }

    public void ResetNodeAtPoint(Point p)
    {
        _board[p.X, p.Y].piece.Index = p; 
        _board[p.X, p.Y].Index = p;
    }

    void SetValueAtPoint(Point p, int value)
    {
        _board[p.X, p.Y].Value = value;
    }

    int FillPiece()
    {
        int value = 0;
        value = Math.Min(_random.Next(0, 100) / (100 / Pieces.Length) + 1, Pieces.Length);
        return value;
    }

    string GetRandomSeed()
    {
        string seed = "";
        string acceptableChars = "qwertyuiopasdfghjklzxcvbnm";

        System.Random tempRand = new System.Random();

        for (int i = 0; i < 20; i++)
            seed += acceptableChars[tempRand.Next(0, acceptableChars.Length)];

        return seed; 
    } 
}
