using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    [SerializeField] private Player playerPrefabs;
    [SerializeField] private Transform tfPlayer;
    [SerializeField] private LevelDatas levelData;
    [SerializeField] private PointData PointDatas;
    public PointData pointData => PointDatas;

    [SerializeField] private LevelBonusData LevelTime;
    public LevelBonusData levelTime => LevelTime;
    [SerializeField] private LevelBonusData LevelEx;
    public LevelBonusData levelEx => LevelEx;
    public Player player;

    public void Oninit()
    {
        CameraManager.Ins.Oninit();
        //UIManager.Ins.GetUI<UIGamePlay>().ReLoadUIFollow();
    }
    public void OnLoadLevel()
    {
        if (playerPrefabs != null)
        {
            var data = DataManager.Ins.playerData;
            player = Instantiate(playerPrefabs, tfPlayer);
            player.transform.position = new Vector3(0, 0, 0);
            Oninit();
            player.GetDataLevel(levelData.GetDataWithID(DataManager.Ins.playerData.levelCurrent).checkPoints);
            player.SetData(data.lvScale,data.lvTime,data.lvEx);
        }
    }
}
[System.Serializable]
public class LevelBonusData
{
    public List<LevelBonusDataModel> levelBonusDataModels;
    public LevelBonusDataModel GetDataWithID(int id)
    {
        return levelBonusDataModels.Find(x => x.id == id);
    }
}

[System.Serializable]
public class LevelBonusDataModel
{
    public int id;
    public float bonus;
}
