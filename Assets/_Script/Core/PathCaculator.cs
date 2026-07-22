using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathCaculator
{
    private int _row;
    private int _column;
    private Stone[,] _boardStone;

    public PathCaculator(int row, int column, Stone[,] boardStone)
    {
        _row = row;
        _column = column;
        _boardStone = boardStone;
    }

    public List<MovePathOfStone> GetMovePathOfStones()
    {
        Dictionary<Stone, MovePathOfStone> stonePathMap = new Dictionary<Stone, MovePathOfStone>();
        for (int j = 0; j < _column; j++)
        {
            for (int i = 0; i < _row; i++)
            {
                Stone stone = _boardStone[i, j];
                if (stone == null || stone.type == StoneType.Ice) continue;

                if (!stonePathMap.TryGetValue(stone, out MovePathOfStone movePath))
                {
                    movePath = new MovePathOfStone(stone);
                    stonePathMap.Add(stone, movePath);
                }
                int curRow = stone.r;
                int curCol = stone.c;
                int fallDistance = 0;
                for (int r = curRow - 1; r >= 0; r--)
                {
                    if (_boardStone[r, curCol] == null) fallDistance++;
                    else break;
                }

                if (fallDistance > 0)
                {
                    _boardStone[curRow, curCol] = null;
                    curRow -= fallDistance;
                    _boardStone[curRow, curCol] = stone;
                    movePath.movePath.Add((curRow, curCol));
                }
                bool canMove = true;
                while (canMove)
                {
                    canMove = false;

                    if (curRow - 1 >= 0 && curCol - 1 >= 0 &&
                        _boardStone[curRow - 1, curCol] != null &&
                        _boardStone[curRow - 1, curCol - 1] == null &&
                        _boardStone[curRow, curCol - 1] != null &&
                        _boardStone[curRow, curCol - 1].type == StoneType.Ice)
                    {
                        _boardStone[curRow, curCol] = null;
                        curRow--; curCol--;
                        _boardStone[curRow, curCol] = stone;
                        movePath.movePath.Add((curRow, curCol));
                        canMove = true;
                        continue;
                    }

                    if (curRow - 1 >= 0 && curCol + 1 < _column &&
                        _boardStone[curRow - 1, curCol] != null &&
                        _boardStone[curRow - 1, curCol + 1] == null &&
                        _boardStone[curRow, curCol + 1] != null &&
                        _boardStone[curRow, curCol + 1].type == StoneType.Ice)
                    {
                        _boardStone[curRow, curCol] = null;
                        curRow--; curCol++;
                        _boardStone[curRow, curCol] = stone;

                        movePath.movePath.Add((curRow, curCol));
                        canMove = true;
                        continue;
                    }

                    int extraFall = 0;
                    for (int r = curRow - 1; r >= 0; r--)
                    {
                        if (_boardStone[r, curCol] == null) extraFall++;
                        else break;
                    }

                    if (extraFall > 0)
                    {
                        _boardStone[curRow, curCol] = null;
                        curRow -= extraFall;
                        _boardStone[curRow, curCol] = stone;

                        movePath.movePath.Add((curRow, curCol));
                        canMove = true;
                    }
                }
                stone.r = curRow; stone.c = curCol;
            }
        }

        List<MovePathOfStone> result = new List<MovePathOfStone>();
        foreach (var kv in stonePathMap)
        {
            if (kv.Value.movePath.Count > 0)
                result.Add(kv.Value);
        }

        return result;
    }
}