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
                _board[x, y] = new Node(BoardLayaout.Rows[y].Row[x] ? -1 : FillPiece(), new Point(x, y));
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
