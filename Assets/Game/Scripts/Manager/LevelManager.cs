using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

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

    private float deltaScale;
    private float deltaDur;
    public EnemyData enemyDatas;
    public PointData pointData => PointDatas;
    public LevelBonusData levelTime => LevelTime;
    public LevelBonusData levelEx => LevelEx;

    public Player player;
    public Stage stage;
    public FloorBoss floorBoss;
    public Boss bossTimeUp;

    public bool isShoot;
    public List<int> historyMagnetics = new List<int>();
    public bool isCont;
    public bool isCountTime;
    public List<Character> characterList;


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
        stage.transform.position = Vector3.zero;
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
        int validLevelID = data.levelCurrent % levelData.levels.Count;
        player = Instantiate(playerPrefabs);
        characterList.Add(player);
        player.transform.position = levelData.GetDataWithID(validLevelID).positonStart;
        player.transform.rotation = Quaternion.Euler(0, 180, 0);
        CameraManager.Ins.SetData(player);
        player.GetDataLevel(levelData.GetDataWithID(validLevelID).checkPoints);
        player.SetData(data.lvScale, data.lvTime, data.lvEx);
        StartCoroutine(IE_OninitEnemy());
        SetEX();
    }
    IEnumerator IE_OninitEnemy()
    {
        yield return new WaitForEndOfFrame();
        EnemyManager.Ins.Oninit(player);
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
        characterList.Clear();
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
    public void DespawnEnemy()
    {
        StartCoroutine(IE_DespawnEnemy());
    }
    IEnumerator IE_DespawnEnemy()
    {
        yield return new WaitForSeconds(0.5f);
        EnemyManager.Ins.DespawnEnemy();
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
        player.transform.rotation = Quaternion.Euler(0, 0, 0);
        float distance = Vector3.Distance(Camera.main.transform.position, player.transform.position);
        Vector3 maxPosition = GetMaxVisiblePosition(Camera.main, distance);
        player.ChangeAnim(Const.ANIM_UP);
        DOVirtual.DelayedCall(0.1f, () =>
        {
            player.transform.DOMove(new Vector3(player.transform.position.x, maxPosition.y, player.transform.position.z), 0.6f)
             .SetEase(Ease.Linear)
             .OnComplete(() =>
             {
                 player.HideCollider(false);
                 player.transform.DOMove(new Vector3(tf.position.x, tf.position.y, tf.position.z), 0.2f)
                 .SetEase(Ease.Linear)
                 .OnComplete(() =>
                 {
                     player.HideCollider(true);
                     CameraManager.Ins.virtualCamera.enabled = true;
                     CameraManager.Ins.SetTransform();
                     StartCoroutine(IE_SetupBossFight());
                 });
             });
        });
    }
    IEnumerator IE_PlayAnim()
    {
        yield return new WaitForSeconds(1f);
        player.OnMove();
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
        if (player.lvCurrent <= 5)
        {
            deltaScale = 2;
            deltaDur = 60;
        }
        else
        {
            deltaScale = 4;
            deltaDur = 80;
        }
        Vector3 tf = CalculateNewPosition(Camera.main.transform.position, player.transform.position, deltaDur);
        bossTimeUp.transform.position = new Vector3(tf.x, 100f, tf.z);
        bossTimeUp.transform.rotation = Quaternion.Euler(0, 180, 0);
        bossTimeUp.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        bossTimeUp.point = bossData.pointBoss;
       
        bossTimeUp.transform.DOScale(new Vector3(deltaScale, deltaScale, deltaScale), 1)
            .SetEase(Ease.OutBounce)
            .OnComplete(() =>
            {
                UIManager.Ins.GetUI<UIGamePlay>().SetUIFloorBoss(player.GetDataScale(),player.point);
                bossTimeUp.isUpdate = true;
            });
    }
    Vector3 CalculateNewPosition(Vector3 from, Vector3 to, float distance)
    {
        Vector3 direction = to - new Vector3(from.x, to.y, from.z);
        direction.Normalize();
        Vector3 newPosition = from + direction * distance;
        return newPosition;
    }
    public void RemoveTarget(Character character)
    {
        for (int i = 0; i < characterList.Count; i++)
        {
            if (characterList[i].listTarget.Contains(character))
            {
                characterList[i].listTarget.Remove(character);
            }
        }
    }
    private IEnumerator IE_ShowUILose()
    {
        yield return new WaitForSeconds(5f);
        EnemyManager.Ins.DespawnEnemy();
        UIManager.Ins.OpenUI<PopupReward>().Oninit(false);
    }
    public void OnPlayDead()
    {
        EnemyManager.Ins.PlayIdle();
        stage.IsCountDown(false);
        StartCoroutine(IE_ShowUILose());
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
