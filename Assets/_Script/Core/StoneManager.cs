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

public class StoneManager : MonoBehaviour
{
    private int _row;
    private int _column;

    public Stone[,] boardStone;
    public StonePoolManager stonePoolManager;
    public StoneConfig stoneConfig;

    public void InitLevel(LevelData levelData)
    {
        _row = levelData.row;
        _column = levelData.column;
        boardStone = new Stone[_row, _column];
        SpawnStoneForNewGame(levelData.positionIceList, levelData.normalStone);
    }

    public void SpawnStoneForNewGame(List<(int r, int c)> positionIceList, List<StoneType> normalStone)
    {
        foreach (var pos in positionIceList)
        {
            GameObject ice = GameObject.Instantiate(stoneConfig.GetStoneByType(StoneType.Ice), this.transform);
            ice.transform.localPosition = new Vector2(pos.c, pos.r);
            Stone stone = ice.GetComponent<Stone>();
            stone.c = pos.c;
            stone.r = pos.r;
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
                stoneScript.r = r; stoneScript.c = c; stoneScript.type = type;
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
}
