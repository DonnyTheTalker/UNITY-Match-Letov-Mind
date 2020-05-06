using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
class Node
{
    public int Value; // 0 - blank, 1:6 - suitable images, -1 - hole
    public Point Index;
    public NodePiece piece;

    public Node(int val, Point point, NodePiece piece)
    {
        Value = val;
        Index = point;
        this.piece = piece;
    } 

    public Node(Node node)
    {
        Value = node.Value;
        Index = new Point(node.Index.X, node.Index.Y);
        piece = null;

    }

}