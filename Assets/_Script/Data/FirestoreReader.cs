using Firebase.Extensions;
using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class FirestoreReader : MonoBehaviour
{
    private FirebaseFirestore db;
    private void Awake()
    {
        db = FirebaseFirestore.DefaultInstance;
    }
    public async Task<LevelData> LoadLevelData(string idLevel)
    {
        var docRef = db.Collection("levels").Document(idLevel);
        DocumentSnapshot s = await docRef.GetSnapshotAsync();
        LevelData levelData = new LevelData();
        if (s.Exists)
        {
            Dictionary<string, object> data = s.ToDictionary();

            // Doc cac thong so
            levelData.row = Convert.ToInt32(data["row"]);
            levelData.column = Convert.ToInt32(data["column"]);
            levelData.moves = Convert.ToInt32(data["moves"]);

            // Doc vi tri sinh ice
            var posIceList = data["position_ice"] as List<object>;
            foreach (object pos in posIceList)
            {
                var position = pos as Dictionary<string, object>;
                int r = Convert.ToInt32(position["r"]);
                int c = Convert.ToInt32(position["c"]);
                levelData.positionIceList.Add((r, c));
            }

            // Doc danh sach rule
            var ruleList = data["rules"] as List<object>;
            foreach (object ruleObj in ruleList)
            {
                string rule = ruleObj.ToString();
                levelData.ruleList.Add(rule);
            }

            // Doc target
            if (data.ContainsKey("target") && data["target"] is Dictionary<string, object> targetData)
            {
                foreach (var entry in targetData)
                {
                    levelData.targetDict.Add(entry.Key, Convert.ToInt32(entry.Value));
                }
            }

            // Doc danh sach sinh stone normal va special
            docRef = db.Collection("rules").Document(levelData.ruleList[0]);
            s = await docRef.GetSnapshotAsync();
            if (s.Exists && s.ContainsField("spawnNormal"))
            {
                Dictionary<string, object> ruleData = s.ToDictionary();
                var stoneList = ruleData["spawnNormal"] as List<object>;
                foreach (object a in stoneList)
                {
                    string nameStone = a.ToString();
                    StoneType type = Enum.Parse<StoneType>(nameStone, true);
                    levelData.normalStone.Add(type);
                }
            }

            if (s.Exists && s.ContainsField("spawnSpecial"))
            {
                Dictionary<string, object> ruleData = s.ToDictionary();
                var stoneList = ruleData["spawnSpecial"] as List<object>;
                foreach (object a in stoneList)
                {
                    string nameStone = a.ToString();
                    StoneType type = Enum.Parse<StoneType>(nameStone, true);
                    levelData.specialStone.Add(type);
                }
            }
        }
        return levelData;
    }
}