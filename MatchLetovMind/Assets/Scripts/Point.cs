using System;
using UnityEngine;

[System.Serializable]
public class Point
{ 
    public int X;
    public int Y;

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }

    public static Point FromVector(Vector2 v)
    {
        return new Point((int)v.x, (int)v.y);
    }

    public static Point FromVector(Vector3 v)
    {
        return new Point((int)v.x, (int)v.y);
    }

    public Vector2 ToVector()
    {
        return new Vector2(X, Y);
    }

    public static bool Equals(Point lhs, Point rhs)
    {
        return lhs.X == rhs.X && lhs.Y == rhs.Y;
    }

    public static Point Mult(Point p, int m)
    {
        return new Point(p.X * m, p.Y * m);
    }

    public static Point Clone(Point node)
    {
        return new Point(node.X, node.Y);
    } 

    public static Point Add(Point lhs, Point rhs)
    {
        return new Point(lhs.X + rhs.X, lhs.Y + rhs.Y);
    }

    public static Point Up
    {
        get { return new Point(0, 1); }
    }

    public static Point Down
    {
        get { return new Point(0, -1); }
    }

    public static Point Right
    {
        get { return new Point(1, 0); }
    }

    public static Point Left
    {
        get { return new Point(-1, 0); }
    }

    public static Point Zero
    {
        get { return new Point(0, 0); }
    }

    public static Point One
    {
        get { return new Point(1, 1); }
    }

}
