using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class LevelManager : Singleton<LevelManager>
{
    [SerializeField] private Player playerPrefabs;
    [SerializeField] private Transform tfPlayer;
    [SerializeField] private Transform tfStage;
    [SerializeField] private Transform tfBoss;
    [SerializeField] private LevelDatas levelData;
    [SerializeField] private PointData PointDatas;
    [SerializeField] private LevelBonusData LevelEx;
    [SerializeField] private LevelBonusData LevelTime;
    [SerializeField] private FloorBoss floorBossPrefabs;
    [SerializeField] private Vector3 cameraPos;
    [SerializeField] private Vector3 cameraRotate;
    public EnemyData enemyDatas;
    public PointData pointData => PointDatas;
    public LevelBonusData levelTime => LevelTime;
    public LevelBonusData levelEx => LevelEx;
    public Player player;
    public Stage stage;
    public FloorBoss floorBoss;
    public Enemy bossTimeUp;

    public List<int> historyMagnetics = new List<int>();
    public void Oninit()
    {
        CameraManager.Ins.Oninit();
    }
    public void OnLoadStage()
    {
        stage = Instantiate(levelData.GetDataWithID(DataManager.Ins.playerData.levelCurrent).stage, tfStage);
        stage.SetTimeData(levelData.GetDataWithID(DataManager.Ins.playerData.levelCurrent).timer);
    }
    public void OnLoadLevel()
    {
        if (playerPrefabs != null)
        {
            historyMagnetics.Clear();
            OnLoadStage();
            var data = DataManager.Ins.playerData;
            player = Instantiate(playerPrefabs, tfPlayer);
            player.transform.position = new Vector3(0, 0, 0);
            player.GetDataLevel(levelData.GetDataWithID(DataManager.Ins.playerData.levelCurrent).checkPoints);
            Oninit();
            player.SetData(data.lvScale, data.lvTime, data.lvEx);
        }
    }
    public void UpDataScale()
    {
        var data = DataManager.Ins.playerData;
        if (data.lvScale < player.checkPoints.Count - 1)
        {
            data.lvScale += 1;
            player.lvCurrent = data.lvScale;
            player.SetScale(player.lvCurrent);
        }
    }
    public void UpLVBonusExp()
    {
        var data = DataManager.Ins.playerData;
        if (data.lvEx < levelEx.levelBonusDataModels.Count - 1)
        {
            data.lvEx += 1;
            player.lvEx = data.lvEx;
        }
    }

    public void OnTimeUP()
    {
        UIManager.Ins.GetUI<UIGamePlay>().SetActiveJoystick(false);
        Vector3 targetPosition = new Vector3(0, 100, 0);
        Quaternion rotation = Quaternion.Euler(0, 45, 0);
        floorBoss = Instantiate(floorBossPrefabs, targetPosition, rotation);
        bossTimeUp = Instantiate(levelData.GetDataWithID(DataManager.Ins.playerData.levelCurrent).boss, tfBoss);
        bossTimeUp.transform.position = floorBoss.tfBoss.position;
        bossTimeUp.transform.localScale = new Vector3(2,2,2);
        player.transform.DOMove(floorBoss.targetPlayerMove.position, 0.2f).SetEase(Ease.InOutQuad).OnComplete(()=>{

            stage.gameObject.SetActive(false);
            GameManager.Ins.ChangeState(GameState.Finish);
            CameraManager.Ins.SetTfCamera(cameraPos,cameraRotate);
            UIManager.Ins.GetUI<UIGamePlay>().SetAtiveBtnShot();
        });
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

[System.Serializable]
public class EnemyData
{
    public List<EnemyDataModel> enemyDataModels;
    public EnemyDataModel GetDataWithID(int id)
    {
        return enemyDataModels.Find(x => x.id == id);
    }
}

[System.Serializable]
public class EnemyDataModel
{
    public int id;
    public Enemy enemy;
}
