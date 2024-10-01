using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/Data/LevelData", order = 1)]
public class LevelDatas : ScriptableObject
{
    public List<LevelData> levels;
    public LevelData GetDataWithID(int id)
    {
        return levels.Find(x => x.levelID == id);
    }
}
[System.Serializable]
public class LevelData
{
    public int levelID;
    public List<CheckPoint> checkPoints; 
}
[System.Serializable]
public class CheckPoint
{
    public int idCheckPoint;
    public float checkPoint;
}