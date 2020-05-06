using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
using UnityEngine.UI;

public class NodePiece : MonoBehaviour
{

    public int Value;
    public Point Index;

    [HideInInspector]
    public Vector2 Pos;
    [HideInInspector]
    public RectTransform Rect;

    private Image _img;

    public void Initialize(int v, Point P, Sprite sprite)
    {
        _img = GetComponent<Image>();
        Rect = GetComponent<RectTransform>();

        Value = v;
        Index = P;
        _img.sprite = sprite;

        ResetPosition();
        UpdateName();

    }

    public void SetIndex(Point index)
    {
        Index = index;
        ResetPosition();
        UpdateName();
    }

    public void ResetPosition()
    {
        Pos = new Vector2(GameManager.XOffset + Index.X * GameManager.NodeSize, 
            GameManager.YOffset - Index.Y * GameManager.NodeSize);
        Rect.anchoredPosition = Pos;
    }
   
    public void UpdateName()
    {
        transform.name = "Node [" + Index.X + ", " + Index.Y + "]";
    }

}
