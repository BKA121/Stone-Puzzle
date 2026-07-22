using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : MonoBehaviour
{
    private Vector2 _startPos;
    private Vector2 _endPos;

    public StoneType type;
    public int r;
    public int c;

    // Tinh toan swap stone
    void OnMouseDown()
    {
        _startPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
    void OnMouseUp()
    {
        if (StoneManager.Instance.curState != StateBoard.none) return; 
        _endPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Vector2.Distance(_startPos, _endPos) < 0.5f) return;

        Vector2 dir = _endPos - _startPos;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        HandleSwipeDirection(angle);
    }
    private void HandleSwipeDirection(float angle)
    {
        int dc = 0,dr = 0;
        int r = (int)this.transform.localPosition.y;
        int c = (int)this.transform.localPosition.x;

        if (angle >= -45 && angle <= 45) dc = 1;
        else if (angle > 45 && angle < 135) dr = 1;
        else if (angle >= 135 || angle <= -135) dc = -1;
        else if (angle > -135 && angle < -45) dr = -1;

        if(StoneManager.Instance.CheckStoneBeforeSwap(r, c) && 
           StoneManager.Instance.CheckStoneBeforeSwap(r + dr, c + dc))
        {
            StartCoroutine(StoneManager.Instance.SwapStone(r, c, r + dr, c + dc));
        }
    }

    public void Explode(StonePoolManager stonePoolManager, Stone[,] boardStone)
    {
        stonePoolManager.ReturnStoneByType(type, this.gameObject);
        boardStone[r, c] = null;
    }
}
