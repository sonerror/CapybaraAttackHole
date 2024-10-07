using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    [SerializeField] private Player playerPrefabs;
    [SerializeField] private Transform tfPlayer;
    [SerializeField] private Transform tfStage;
    [SerializeField] private Transform tfBoss;
    [SerializeField] private LevelDatas levelData;
    public LevelDatas _levelData => levelData;
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

    public bool isShoot;
    public List<int> historyMagnetics = new List<int>();
    public bool isCont;
    public bool isCountTime;
    private void Awake()
    {
        OnInit();
    }

    public void OnInit()
    {
        CameraManager.Ins.Oninit();
        isShoot = true;
        isCont = true;
        isCountTime = false;
    }

    public void OnLoadStage()
    {
        int totalLevels = levelData.levels.Count;
        int validLevelID = DataManager.Ins.playerData.levelCurrent % totalLevels;
        stage = Instantiate(levelData.GetDataWithID(validLevelID).stage, tfStage);
        SetTimeCount();
    }

    public void OnLoadLevel()
    {
        if (playerPrefabs != null)
        {
            historyMagnetics.Clear();
            OnLoadStage();
            InstantiatePlayer();
            OnInit();
        }
    }

    private void InstantiatePlayer()
    {
        var data = DataManager.Ins.playerData;
        player = Instantiate(playerPrefabs, tfPlayer);
        player.transform.position = Vector3.zero;
        int validLevelID = data.levelCurrent % levelData.levels.Count;
        player.GetDataLevel(levelData.GetDataWithID(validLevelID).checkPoints);
        player.SetData(data.lvScale, data.lvTime, data.lvEx);
    }

    public LevelBonusDataModel GetDataTimeCountWithId(int id)
    {
        return levelTime.levelBonusDataModels?.Find(x => x.id == id);
    }

    public void UpDataScale()
    {
        var data = DataManager.Ins.playerData;
        if (data.lvScale < player.checkPoints.Count - 1)
        {
            data.lvScale++;
            player.lvCurrent = data.lvScale;
            player.SetScale(player.lvCurrent);
        }
    }

    public void SetTimeCount()
    {
        stage.SetTimeData((int)GetDataTimeCountWithId(DataManager.Ins.playerData.lvTime).bonus);
    }

    public void UpLVBonusExp()
    {
        UpdateLevelBonus(ref DataManager.Ins.playerData.lvEx, player.lvEx, levelEx);
    }

    public void UpLVBonusTime()
    {
        UpdateLevelBonus(ref DataManager.Ins.playerData.lvTime, player.lvTime, levelTime);
        SetTimeCount();
    }

    private void UpdateLevelBonus(ref int levelData, int playerLevel, LevelBonusData bonusData)
    {
        if (levelData < bonusData.levelBonusDataModels.Count - 1)
        {
            levelData++;
            playerLevel = levelData;
        }
    }

    public void ReLoad()
    {
        Destroy(floorBoss?.gameObject);
        Destroy(bossTimeUp?.gameObject);
        player.checkPoints.Clear();
        Destroy(player.gameObject);
        if (stage != null)
        {
            Destroy(stage?.gameObject);
        }
    }

    public void ReloadScene()
    {
        ReLoad();
        DOVirtual.DelayedCall(0.1f, () =>
        {
            SceneController.Ins.ChangeScene(Const.GAMEPLAY_SCENE, () =>
            {
                UIManager.Ins.OpenUI<UIHome>();
                OnLoadLevel();
                GameManager.Ins.ChangeState(GameState.MainMenu);
            });
        });
    }

    public void OnTimeUP()
    {
        if (historyMagnetics.Count > 0)
        {
            UIManager.Ins.GetUI<UIGamePlay>().SetActiveJoystick(false);
            SpawnFloorBoss();
        }
        else
        {
            UIManager.Ins.OpenUI<PopupLose>();
        }
    }

    private void SpawnFloorBoss()
    {
        Vector3 targetPosition = new Vector3(0, 100, 0);
        Quaternion rotation = Quaternion.Euler(0, 45, 0);
        floorBoss = Instantiate(floorBossPrefabs, targetPosition, rotation);
        MovePlayerToBoss();
    }

    private void MovePlayerToBoss()
    {
        player.transform.DOMove(floorBoss.targetPlayerMove.position, 0.2f)
            .SetEase(Ease.InOutQuad)
            .OnComplete(SetupBossFight);
    }

    private void SetupBossFight()
    {
        player.transform.rotation = Quaternion.identity;
        Destroy(stage.gameObject);
        GameManager.Ins.ChangeState(GameState.Finish);
        UIManager.Ins.GetUI<UIGamePlay>().SetAtiveBtnShot();
        int totalLevels = levelData.levels.Count;
        int validLevelID = DataManager.Ins.playerData.levelCurrent % totalLevels;
        var boss = levelData.GetDataWithID(validLevelID);
        SpawnBoss(boss);
    }

    private void SpawnBoss(LevelData bossData)
    {
        bossTimeUp = Instantiate(bossData.boss, tfBoss);
        bossTimeUp.transform.position = floorBoss.tfBoss.position;
        bossTimeUp.transform.rotation = Quaternion.Euler(0, 180, 0);
        bossTimeUp.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        bossTimeUp.point = bossData.pointBoss;
        bossTimeUp.transform.DOScale(new Vector3(2, 2, 2), 1)
            .SetEase(Ease.OutBounce)
            .OnComplete(() =>
            {
                UIManager.Ins.GetUI<UIGamePlay>().OninitHPBoss();
            });
    }
}

[System.Serializable]
public class LevelBonusData
{
    public List<LevelBonusDataModel> levelBonusDataModels;

    public LevelBonusDataModel GetDataWithID(int id)
    {
        return levelBonusDataModels?.Find(x => x.id == id);
    }
}

[System.Serializable]
public class LevelBonusDataModel
{
    public int id;
    public float bonus;
    public int price;
}

[System.Serializable]
public class EnemyData
{
    public List<EnemyDataModel> enemyDataModels;

    public EnemyDataModel GetDataWithID(int id)
    {
        return enemyDataModels?.Find(x => (int)x.poolType == id);
    }
}

[System.Serializable]
public class EnemyDataModel
{
    public PoolType poolType;
    public Enemy enemy;
}
