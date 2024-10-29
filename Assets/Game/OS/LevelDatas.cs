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
    public Stage stage;
    public Enemy boss;
    public float pointBoss;
    public Vector3 positonStart;

}
[System.Serializable]
public class CheckPoint
{
    public int id;
    public float checkPoint;
    public float scale;
    public float speedMove;
    public int price;
    public float cameraDistance;
}