using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelData 
{
    public int row;
    public int column;
    public int moves;
    public List<(int x, int y)> positionIceList = new List<(int r, int c)>();
    public List<string> ruleList = new List<string>();
    public List<StoneType> normalStone = new List<StoneType>();
    public List<StoneType> specialStone = new List<StoneType>();
    public Dictionary<string, int> targetDict = new Dictionary<string, int>();
}
