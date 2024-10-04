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
        int totalLevels = levelData.levels.Count;
        int validLevelID = DataManager.Ins.playerData.levelCurrent % totalLevels;
        stage = Instantiate(levelData.GetDataWithID(validLevelID).stage, tfStage);
        stage.SetTimeData(levelData.GetDataWithID(validLevelID).timer);
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
            int totalLevels = levelData.levels.Count;
            int validLevelID = DataManager.Ins.playerData.levelCurrent % totalLevels;
            player.GetDataLevel(levelData.GetDataWithID(validLevelID).checkPoints);
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
    public void ReLoad()
    {
        Destroy(floorBoss.gameObject);
        Destroy(bossTimeUp.gameObject);
        player.checkPoints.Clear();
        Destroy(player.gameObject);
    }
    public void ReloadScene()
    {
        ReLoad();
        DOVirtual.DelayedCall(0.1f, () =>
        {
            SceneController.Ins.ChangeScene(Const.GAMEPLAY_SCENE, () =>
            {
                UIManager.Ins.OpenUI<UIHome>();
                LevelManager.Ins.OnLoadLevel();
                GameManager.Ins.ChangeState(GameState.MainMenu);
            });
        });
    }
    public void OnTimeUP()
    {
        UIManager.Ins.GetUI<UIGamePlay>().SetActiveJoystick(false);
        Vector3 targetPosition = new Vector3(0, 100, 0);
        Quaternion rotation = Quaternion.Euler(0, 45, 0);
        floorBoss = Instantiate(floorBossPrefabs, targetPosition, rotation);
        player.transform.DOMove(floorBoss.targetPlayerMove.position, 0.2f).SetEase(Ease.InOutQuad).OnComplete(() =>
        {
            player.transform.rotation = Quaternion.Euler(0, 0, 0);
            Destroy(stage.gameObject);
            GameManager.Ins.ChangeState(GameState.Finish);
            CameraManager.Ins.SetTfCamera(cameraPos, cameraRotate);
            UIManager.Ins.GetUI<UIGamePlay>().SetAtiveBtnShot();
            int totalLevels = levelData.levels.Count;
            int validLevelID = DataManager.Ins.playerData.levelCurrent % totalLevels;
            var boss = levelData.GetDataWithID(validLevelID);
            bossTimeUp = Instantiate(boss.boss, tfBoss);
            bossTimeUp.transform.position = floorBoss.tfBoss.position;
            bossTimeUp.transform.rotation = Quaternion.Euler(0, 180, 0);
            bossTimeUp.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            bossTimeUp.transform.DOScale(new Vector3(2, 2, 2), 1).SetEase(Ease.OutBounce).OnComplete(() =>
            {
                bossTimeUp.point = boss.pointBoss;

            });
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
        return enemyDataModels.Find(x => (int)x.poolType == id);
    }
}

[System.Serializable]
public class EnemyDataModel
{
    public PoolType poolType;
    public Enemy enemy;
}
