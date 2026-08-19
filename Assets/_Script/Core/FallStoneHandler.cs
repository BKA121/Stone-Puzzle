using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallStoneHandler : MonoBehaviour
{
    private float _fallSpeed = 6.5f;

    public int countStoneFall = 0;

    public void FallAllStone(List<MovePathOfStone> allMovePathOfStone)
    {
        foreach(var path in allMovePathOfStone)
        {
            countStoneFall++;
            StartCoroutine(FallStone(path.stone, path.movePath));
            
        }
    }

    public IEnumerator FallStone(Stone stone, List<(int row, int col)> movePath)
    {
        //countStoneFall++;
        foreach (var pos in movePath)
        {
            Vector3 target = new Vector3(pos.col, pos.row, 0);

            while(Vector3.Distance(stone.transform.localPosition, target) > 0.01f)
            {
                stone.transform.localPosition = Vector3.MoveTowards(stone.transform.localPosition, target, _fallSpeed * Time.deltaTime);
                yield return null;
            }

            stone.transform.localPosition = target;
        }
        countStoneFall--;
    }
}
