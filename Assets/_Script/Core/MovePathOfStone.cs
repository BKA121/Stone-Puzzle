using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePathOfStone
{
    public Stone stone;
    public List<(int row, int col)> movePath;

    public MovePathOfStone(Stone stone)
    {
        this.stone = stone;
        movePath = new List<(int row, int col)>();
    }
}
