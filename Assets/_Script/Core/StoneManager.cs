using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public enum StoneType
{
    Red, Green, Blue, Purple, Yellow, Ice,
    RedMatch4, GreenMatch4, BlueMatch4, PurpleMatch4, YellowMatch4,
    RedMatchTorL, GreenMatchTorL, BlueMatchTorL, PurpleMatchTorL, YellowMatchTorL, StoneMatch5
}

public enum StateBoard
{
    none, isSwapping
}

public class StoneManager : MonoBehaviour
{
    private int _row;
    private int _column;

    public StateBoard curState;
    public static StoneManager Instance { get; private set; }
    public Stone[,] boardStone;
    public StonePoolManager stonePoolManager;
    public StoneConfig stoneConfig;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this.gameObject);
        else Instance = this;
    }

    public void InitLevel(LevelData levelData)
    {
        curState = StateBoard.none;
        _row = levelData.row;
        _column = levelData.column;
        boardStone = new Stone[_row, _column];
        SpawnStoneForNewGame(levelData.positionIceList, levelData.normalStone);
    }

    // Spawn stone
    public void SpawnStoneForNewGame(List<(int r, int c)> positionIceList, List<StoneType> normalStone)
    {
        foreach (var pos in positionIceList)
        {
            GameObject ice = GameObject.Instantiate(stoneConfig.GetStoneByType(StoneType.Ice), this.transform);
            ice.transform.localPosition = new Vector2(pos.c, pos.r);
            Stone stone = ice.GetComponent<Stone>();
            stone.type = StoneType.Ice;
            boardStone[pos.r, pos.c] = stone;
        }

        for (int r = 0; r < _row; r++)
        {
            for (int c = 0; c < _column; c++)
            {
                if (boardStone[r, c] != null) continue;
                List<StoneType> availableStone = new List<StoneType>(normalStone);
                StoneType type = StoneType.Red;
                while (availableStone.Count > 0)
                {
                    int index = UnityEngine.Random.Range(0, availableStone.Count);
                    type = availableStone[index];
                    if (!IsPositionValid(r, c, type))
                    {
                        availableStone.RemoveAt(index);
                    }
                    else break;
                }
                GameObject stone = stonePoolManager.GetStoneByType(type);
                stone.transform.SetParent(this.transform);
                stone.transform.localPosition = new Vector2(c, r);
                Stone stoneScript = stone.GetComponent<Stone>();
                stoneScript.type = type;
                boardStone[r, c] = stoneScript;
            }
        }
    }
    public bool IsPositionValid(int r, int c, StoneType type)
    {
        if (r < 2 && c < 2) return true;
        if (c >= 2 && boardStone[r, c - 1].type == type
                   && boardStone[r, c - 2].type == type) return false;
        if (r >= 2 && boardStone[r - 1, c].type == type
                   && boardStone[r - 2, c].type == type) return false;
        return true;
    }

    // Swap stone
    public IEnumerator SwapStone(int ra, int ca, int rb, int cb)
    {
        curState = StateBoard.isSwapping;
        Stone stoneA = boardStone[ra, ca];
        Stone stoneB = boardStone[rb, cb];

        boardStone[ra, ca] = stoneB;
        boardStone[rb, cb] = stoneA;

        yield return StartCoroutine(SmoothMove(stoneA, stoneB, 0.25f));

        bool matchA = CheckStoneAfterSwap(ra, ca, boardStone[ra, ca].type);
        bool matchB = CheckStoneAfterSwap(rb, cb, boardStone[rb, cb].type);

        if (!matchA && !matchB)
        {
            boardStone[ra, ca] = stoneA;
            boardStone[rb, cb] = stoneB;

            yield return StartCoroutine(SmoothMove(stoneB, stoneA, 0.25f));
        }
        else
        {
            Debug.Log("Match Found!");
        }

        curState = StateBoard.none;
    }
    public IEnumerator SmoothMove(Stone s1, Stone s2, float duration)
    {
        Vector3 start1 = s1.transform.position;
        Vector3 start2 = s2.transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float p = elapsed / duration;
            float curve = p * p * (3f - 2f * p); 

            s1.transform.position = Vector3.Lerp(start1, start2, curve);
            s2.transform.position = Vector3.Lerp(start2, start1, curve);

            yield return null;
        }
        s1.transform.position = start2;
        s2.transform.position = start1;
        curState = StateBoard.none;
    }
    public bool CheckStoneBeforeSwap(int r, int c)
    {
        if (r < 0 || r >= _row/2 || c < 0 || c >= _column) return false;
        if (boardStone[r, c] == null) return false;
        if (boardStone[r, c].type == StoneType.Ice) return false;
        return true;
    }
    public bool CheckStoneAfterSwap(int r, int c, StoneType type)
    {
        int countVertical = 1; 

        for (int i = r + 1; i < _row/2; i++)
        {
            if (boardStone[i, c] != null && boardStone[i, c].type == type) countVertical++;
            else break; 
        }
        for (int i = r - 1; i >= 0; i--)
        {
            if (boardStone[i, c] != null && boardStone[i, c].type == type) countVertical++;
            else break;
        }
        if (countVertical >= 3) return true;

        int countHorizontal = 1;

        for (int i = c + 1; i < _column; i++)
        {
            if (boardStone[r, i] != null && boardStone[r, i].type == type) countHorizontal++;
            else break;
        }
        for (int i = c - 1; i >= 0; i--)
        {
            if (boardStone[r, i] != null && boardStone[r, i].type == type) countHorizontal++;
            else break;
        }
        if (countHorizontal >= 3) return true;

        return false;
    }
}
