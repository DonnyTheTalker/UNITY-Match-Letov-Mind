using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
class Node
{
    public int Value; // 0 - blank, 1:6 - suitable images, -1 - hole
    public Point Index;

    public Node(int v, Point i)
    {
        Value = v;
        Index = i;
    }

}