using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private LevelData _levelData;

    public FirestoreReader firestoreReader;
    public StonePoolManager stonePoolManager;
    public StoneManager stoneManager;

    private async void Start() 
    {
        await HandleLevelSelection("1");
    }

    public async Task HandleLevelSelection(string idLevel)
    {
        _levelData = await firestoreReader.LoadLevelData(idLevel);
        InitializeLevel();
    }

    public void InitializeLevel()
    {
        stonePoolManager.InitializePools(_levelData.normalStone, _levelData.specialStone);
        stoneManager.InitLevel(_levelData);
    }
}
