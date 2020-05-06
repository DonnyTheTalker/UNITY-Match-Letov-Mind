using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class NodePiece : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    public float MovingSpeed = 8f;
    public int Value;
    public Point Index;
    public bool IsGrabbed = false;

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
        Pos = GameManager.GetPositionFromPoint(Index);
        Rect.anchoredPosition = Pos;
    }

    public void ResetPiece()
    {
        StartCoroutine(MovePositionBack());
    }

    public IEnumerator MovePositionBack()
    {
        Vector2 dest = Pos;
         
        while (Mathf.Abs(Vector2.Distance(dest, Rect.anchoredPosition)) > 0.003) {
            MovePositionTo(dest);
            yield return null;
        }

        Rect.anchoredPosition = dest; 
    }

    public void MovePositionTo(Vector2 pos)
    {
        Rect.anchoredPosition = Vector2.Lerp(Rect.anchoredPosition, pos, Time.deltaTime * MovingSpeed);
    }

    public void MovePosition(Vector2 pos)
    {
        Rect.anchoredPosition = pos;
    }

    public void UpdateName()
    {
        transform.name = "Node [" + Index.X + ", " + Index.Y + "]";
    }

    public bool UpdatePiece()
    {
        return true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (IsGrabbed) return;
        PieceMovement.Instance.MovePiece(this);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        PieceMovement.Instance.DropPiece();
        IsGrabbed = false;
    }

}
