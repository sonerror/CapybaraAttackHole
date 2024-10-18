using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    [SerializeField] private Player playerPrefabs;
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
  
    public void OnInit()
    {
        isShoot = true;
        isCont = true;
        isCountTime = false;
    }

    public void OnLoadStage()
    {
        int totalLevels = levelData.levels.Count;
        int validLevelID = DataManager.Ins.playerData.levelCurrent % totalLevels;
        stage = Instantiate(levelData.GetDataWithID(validLevelID).stage);
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
        player = Instantiate(playerPrefabs);
        player.transform.position = new Vector3(-29.4421997f, 0.00171999994f, 10.6892004f);
        player.transform.rotation = Quaternion.Euler(0, 180, 0);
        CameraManager.Ins.SetData(player);
        int validLevelID = data.levelCurrent % levelData.levels.Count;
        player.GetDataLevel(levelData.GetDataWithID(validLevelID).checkPoints);
        player.SetData(data.lvScale, data.lvTime, data.lvEx);
        SetEX();
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
        var data = DataManager.Ins.playerData;
        if (data.lvEx < levelEx.levelBonusDataModels.Count - 1)
        {
            data.lvEx++;
            player.lvEx = data.lvEx;
            player.bonusGlod = levelEx.levelBonusDataModels.Find(x => x.id == data.lvEx).bonus;
        }
    }
    public void SetEX()
    {
        player.SetDataBonusGold();
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
            playerLevel++;
        }
    }

    public void ReLoad()
    {
        player.checkPoints.Clear();
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
            StopAllCoroutines();
            UIManager.Ins.GetUI<UIGamePlay>().SetActiveJoystick(false);
            SpawnFloorBoss();
        }
        else
        {
            UIManager.Ins.OpenUI<PopupLose>();
        }
        StartCoroutine(IE_DespawnEnemy());
    }
    IEnumerator IE_DespawnEnemy()
    {
        yield return new WaitForSeconds(1f);
        EnemyManager.Ins.DespawnEnemy();
        EnemyManager.Ins.ResetEnemes();
    }
    private void SpawnFloorBoss()
    {
        Vector3 targetPosition = new Vector3(0, 100, 0);
        Quaternion rotation = Quaternion.Euler(0, 45, 0);
        floorBoss = Instantiate(floorBossPrefabs, targetPosition, rotation);
        StartCoroutine(IE_MovePlayerToBoss(floorBoss.targetPlayerMove));
    }
    IEnumerator IE_MovePlayerToBoss(Transform tf)
    {
        yield return new WaitForSeconds(0.2f);
        CameraManager.Ins.virtualCamera.enabled = false;
        player.transform.rotation = Quaternion.identity;
        float distance = Vector3.Distance(Camera.main.transform.position, player.transform.position);
        Vector3 maxPosition = GetMaxVisiblePosition(Camera.main, distance);
        player.transform.DOMove(new Vector3(player.transform.position.x, maxPosition.y, player.transform.position.z), 0.6f)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                player.HideColliderPlayer(false);
                player.transform.DOMove(new Vector3(tf.position.x, tf.position.y + player.transform.localScale.y * 5f, tf.position.z), 0.2f)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    player.HideColliderPlayer(true);
                    CameraManager.Ins.virtualCamera.enabled = true;
                    CameraManager.Ins.SetTransform();
                    StartCoroutine(IE_SetupBossFight());
                });
            });
    }
    public Vector3 GetMaxVisiblePosition(Camera camera, float distance)
    {
        float aspect = camera.aspect;
        float verticalFOV = camera.fieldOfView * Mathf.Deg2Rad;
        float frustumHeight = 2f * distance * Mathf.Tan(verticalFOV / 2f);
        float frustumWidth = frustumHeight * aspect;
        return new Vector3(frustumWidth / 2f, frustumHeight / 2f, distance);
    }

    IEnumerator IE_SetupBossFight()
    {
        yield return new WaitForSeconds(1f);
        Destroy(stage.gameObject);
        GameManager.Ins.ChangeState(GameState.Finish);
        int totalLevels = levelData.levels.Count;
        int validLevelID = DataManager.Ins.playerData.levelCurrent % totalLevels;
        var boss = levelData.GetDataWithID(validLevelID);
        SpawnBoss(boss);
    }

    private void SpawnBoss(LevelData bossData)
    {
        bossTimeUp = Instantiate(bossData.boss);
        Vector3 tf = CalculateNewPosition(Camera.main.transform.position, player.transform.position, 15);
        bossTimeUp.transform.position = new Vector3(tf.x, 100f, tf.z);
        bossTimeUp.transform.rotation = Quaternion.Euler(0, 180, 0);
        bossTimeUp.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        bossTimeUp.point = bossData.pointBoss;
        bossTimeUp.transform.DOScale(new Vector3(2, 2, 2), 1)
            .SetEase(Ease.OutBounce)
            .OnComplete(() =>
            {
                UIManager.Ins.GetUI<UIGamePlay>().SetUIFloorBoss();
            });
    }
    Vector3 CalculateNewPosition(Vector3 from, Vector3 to, float distance)
    {
        Vector3 direction = to - from;
        direction.Normalize();
        Vector3 newPosition = from + direction * distance;
        return newPosition;
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
