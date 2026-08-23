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
    RedMatchTorL, GreenMatchTorL, BlueMatchTorL, PurpleMatchTorL, YellowMatchTorL, 
    StoneMatch5
}

public enum StoneVFXType
{
    BaseExplosion, Match4Explosion, LightVertical, LightHorizontal, MatchTorLExplosion
}

public enum StateBoard
{
    none, isSwapping
}

public enum MatchType
{
    none, match2, match3, match4, match5, matchTorL
}

public class StoneManager : MonoBehaviour
{
    public int row;
    public int column;
    private List<StoneType> _normalStone;
    private MatchFinder _matchFinder;
    private MatchProcesser _matchProcesser;
    private PathCaculator _pathCaculator;
    private StoneExplosionManager _stoneExplosionManager;
    private StoneVFXManager _stoneVFXManager;
    private int countMatch;
    private MatchGroup _match2;

    public FallStoneHandler fallStoneHandler;
    public StateBoard curState;
    public static StoneManager Instance { get; private set; }
    public Stone[,] boardStone;
    public StonePoolManager stonePoolManager;
    public StoneConfig stoneConfig;
    public Transform StoneVFX;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this.gameObject);
        else Instance = this;
    }

    public void InitLevel(LevelData levelData)
    {
        curState = StateBoard.none;
        row = levelData.row;
        column = levelData.column;
        boardStone = new Stone[row, column];
        _normalStone = levelData.normalStone;
        SpawnStoneForNewGame(levelData.positionIceList);

        _matchFinder = new MatchFinder(row, column, boardStone, this);
        _matchProcesser = new MatchProcesser(stonePoolManager, boardStone);
        _pathCaculator = new PathCaculator(row, column, boardStone);
        _stoneVFXManager = new StoneVFXManager(stonePoolManager, StoneVFX, row, column);
        _stoneExplosionManager = new StoneExplosionManager(stonePoolManager, boardStone, _stoneVFXManager);
    }

    // Spawn stone
    public void SpawnStoneForNewGame(List<(int r, int c)> positionIceList)
    {
        foreach (var pos in positionIceList)
        {
            GameObject ice = GameObject.Instantiate(stoneConfig.GetStoneByType(StoneType.Ice), this.transform);
            ice.transform.localPosition = new Vector2(pos.c, pos.r);
            Stone stone = ice.GetComponent<Stone>();
            stone.r = pos.r;
            stone.c = pos.c;
            stone.type = StoneType.Ice;
            boardStone[pos.r, pos.c] = stone;
        }

        for (int r = 0; r < row; r++)
        {
            for (int c = 0; c < column; c++)
            {
                if (boardStone[r, c] != null) continue;
                List<StoneType> availableStone = new List<StoneType>(_normalStone);
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

                boardStone[r, c] = stonePoolManager.GetStoneByType(type, r, c);
            }
        }
    }
    public void SpawnStoneRefillBoard()
    {
        int count = 0;
        for(int j=0; j<column; j++)
        {
            count = 0;
            for(int i=row-1; i>=0; i--)
            {
                if (boardStone[i, j] != null) break;
                count++;
            }

            for(int i=row-count; i<row; i++)
            {
                List<StoneType> availableStone = new List<StoneType>(_normalStone);
                StoneType type = StoneType.Red;
                while (availableStone.Count > 0)
                {
                    int index = UnityEngine.Random.Range(0, availableStone.Count);
                    type = availableStone[index];
                    if (!IsPositionValidCol(i, j, type))
                    {
                        availableStone.RemoveAt(index);
                    }
                    else break;
                }

                boardStone[i, j] = stonePoolManager.GetStoneByType(type, i, j);
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
    public bool IsPositionValidCol(int r, int c, StoneType type)
    {
        if (boardStone[r - 1, c].type == type && boardStone[r - 2, c].type == type) return false;
        return true;
    }

    public StoneType GetNormalStoneType(StoneType type)
    {
        return type switch
        {
            StoneType.RedMatch4 or StoneType.RedMatchTorL => StoneType.Red,
            
            StoneType.GreenMatch4 or StoneType.GreenMatchTorL => StoneType.Green,
            
            StoneType.BlueMatch4 or StoneType.BlueMatchTorL => StoneType.Blue,
            
            StoneType.PurpleMatch4 or StoneType.PurpleMatchTorL => StoneType.Purple,
            
            StoneType.YellowMatch4 or StoneType.YellowMatchTorL => StoneType.Yellow,
            
            _ => type 
        };
    }

    public bool IsSameNormalType(StoneType type1, StoneType type2)
    {
        return GetNormalStoneType(type1) == GetNormalStoneType(type2);
    }

    // Swap stone
    public IEnumerator SwapStone(int ra, int ca, int rb, int cb)
    {
        curState = StateBoard.isSwapping;
        Stone stoneA = boardStone[ra, ca];
        Stone stoneB = boardStone[rb, cb];

        boardStone[ra, ca] = stoneB;
        stoneB.r = ra; stoneB.c = ca;
        
        boardStone[rb, cb] = stoneA;
        stoneA.r = rb; stoneA.c = cb;

        yield return StartCoroutine(SmoothMove(stoneA, stoneB, 0.25f));

        if(CheckMatch2(stoneA, stoneB))
        {
            _match2 = new MatchGroup(new List<Stone>{stoneA, stoneB}, MatchType.match2);
            StartCoroutine(HandleCore());
        }
        else if (!CheckStoneAfterSwap(ra, ca, boardStone[ra, ca].type) && 
                 !CheckStoneAfterSwap(rb, cb, boardStone[rb, cb].type))
        {
            boardStone[ra, ca] = stoneA;
            stoneA.r = ra; stoneA.c = ca;

            boardStone[rb, cb] = stoneB;
            stoneB.r = rb; stoneB.c = cb;

            yield return StartCoroutine(SmoothMove(stoneB, stoneA, 0.25f));
        }
        else
        {
            StartCoroutine(HandleCore());
        }
    }
    private bool CheckMatch2(Stone stoneA, Stone stoneB)
    {
        if (stoneA.type == StoneType.StoneMatch5 || stoneB.type == StoneType.StoneMatch5) return true;

        if (stoneA.type >= StoneType.RedMatch4 && stoneB.type >= StoneType.RedMatch4) return true;

        return false;
    }

    public IEnumerator SmoothMove(Stone s1, Stone s2, float duration)
    {
        Vector3 start1 = s1.transform.localPosition;
        Vector3 start2 = s2.transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float p = elapsed / duration;
            float curve = p * p * (3f - 2f * p); 

            s1.transform.localPosition = Vector3.Lerp(start1, start2, curve);
            s2.transform.localPosition = Vector3.Lerp(start2, start1, curve);

            yield return null;
        }
        s1.transform.localPosition = start2;
        s2.transform.localPosition = start1;
        curState = StateBoard.none;
    }
    public bool CheckStoneBeforeSwap(int r, int c)
    {
        if (r < 0 || r >= row/2 || c < 0 || c >= column) return false;
        if (boardStone[r, c] == null) return false;
        if (boardStone[r, c].type == StoneType.Ice) return false;
        return true;
    }
    public bool CheckStoneAfterSwap(int r, int c, StoneType type)
    {
        int countVertical = 1; 

        for (int i = r + 1; i < row/2; i++)
        {
            if (boardStone[i, c] != null && IsSameNormalType(boardStone[i, c].type, type)) countVertical++;
            else break; 
        }
        for (int i = r - 1; i >= 0; i--)
        {
            if (boardStone[i, c] != null && IsSameNormalType(boardStone[i, c].type, type)) countVertical++;
            else break;
        }
        if (countVertical >= 3) return true;

        int countHorizontal = 1;

        for (int i = c + 1; i < column; i++)
        {
            if (boardStone[r, i] != null && IsSameNormalType(boardStone[r, i].type, type)) countHorizontal++;
            else break;
        }
        for (int i = c - 1; i >= 0; i--)
        {
            if (boardStone[r, i] != null && IsSameNormalType(boardStone[r, i].type, type)) countHorizontal++;
            else break;
        }
        if (countHorizontal >= 3) return true;

        return false;
    }

    // Core game
    public IEnumerator HandleCore()
    {
        do
        {
            // Find match
            List<MatchGroup> allMatches = new List<MatchGroup>();
            if(_match2 == null)
            {
                allMatches = _matchFinder.FindAllMatches();
            }
            else
            {
                allMatches.Add(_match2);
                _match2 = null;
            }
            
            countMatch = allMatches.Count;
            if (countMatch == 0) break;

            yield return null;

            // Process match and update target stone
            _matchProcesser.ProcessMatch(allMatches, _stoneExplosionManager);
            yield return new WaitForSeconds(0.35f);

            // Fall stone
            List<MovePathOfStone> allMovePathOfStone = _pathCaculator.GetMovePathOfStones();
            fallStoneHandler.FallAllStone(allMovePathOfStone);
            while(fallStoneHandler.countStoneFall != 0)
            {
                yield return null;
            }

            // Refill board
            SpawnStoneRefillBoard();
            yield return null;
        } while (countMatch > 0);
    }
}
