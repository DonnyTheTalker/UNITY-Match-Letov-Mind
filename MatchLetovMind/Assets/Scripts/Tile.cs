using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

public class Tile : MonoBehaviour
{ 
    private Vector2 _firstTouchPos;
    private Vector2 _finalTouchPos;

    private float _swipeAngle = 0f;

    private void OnMouseDown()
    {
        _firstTouchPos = GetCameraPos();
    }

    private void OnMouseUp()
    {
        _finalTouchPos = GetCameraPos();
        CalculateSwipeAngle();
    }

    private Vector2 GetCameraPos()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void CalculateSwipeAngle()
    {
        _swipeAngle = Mathf.Atan2(_finalTouchPos.y - _firstTouchPos.y, _finalTouchPos.x - _firstTouchPos.x) * 180f / Mathf.PI;
        Debug.Log(_swipeAngle);
    }

}
