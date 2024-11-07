using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using System.Collections;

public class EnemyManager : Singleton<EnemyManager>
{
    public List<EnemyMachine> enemies = new List<EnemyMachine>();
    public List<EnemyMachine> activeEnemies = new List<EnemyMachine>();
    public float minDistance;
    [SerializeField] private int quantityInMap;
    [SerializeField] private float radius;
    private PlayerData data;
    public List<PositonStartEnemy> listPosStartEnemy = new List<PositonStartEnemy>();
    public void PoolEnemies()
    {
        data = DataManager.Ins.playerData;
        ResetEnemies();
        GetDataStage(LevelManager.Ins.stage.listPosStartEnemy);
        if (listPosStartEnemy.Count > 0)
        {
            StartCoroutine(IE_CreateEnemyPos());
        }
    }
    public void GetDataStage(List<PositonStartEnemy> _listPosStartEnemy)
    {
        this.listPosStartEnemy = new List<PositonStartEnemy>(_listPosStartEnemy);
    }
    IEnumerator IE_CreateEnemyPos()
    {
        yield return new WaitForEndOfFrame();
        for (int i = 0; i < listPosStartEnemy.Count; i++)
        {
            EnemyMachine enemy = SimplePool.Spawn<EnemyMachine>(PoolType.Enemy_Machine, Vector3.zero, Quaternion.identity);
            enemy.transform.position = listPosStartEnemy[i].positonStart.position;
            yield return new WaitForEndOfFrame();
            int validLevelID = data.levelCurrent % LevelManager.Ins._levelData.levels.Count;
            enemy.GetDataLevel(LevelManager.Ins._levelData.GetDataWithID(validLevelID).checkPoints);
            enemy.SetDataRandom(data.lvScale, data.lvTime, data.lvEx);
            activeEnemies.Add(enemy);
            enemy.isCanMove = false;
            enemy.ChangeState(new IdleState());
        }
        yield return new WaitForSeconds(0.5f);
        LevelManager.Ins.stage.OnEnableNavMesh(true);
    }
    public void SpawmAfterDeath()
    {

    }

    IEnumerator IE_SpawnAfterDeath()
    {
        yield return new WaitForEndOfFrame();
        EnemyMachine enemy = SimplePool.Spawn<EnemyMachine>(PoolType.Enemy_Machine, Vector3.zero, Quaternion.identity);
        int ran = Random.Range(0, listPosStartEnemy.Count);
        enemy.transform.position = listPosStartEnemy[ran].positonStart.position;
        int validLevelID = data.levelCurrent % LevelManager.Ins._levelData.levels.Count;
        enemy.GetDataLevel(LevelManager.Ins._levelData.GetDataWithID(validLevelID).checkPoints);
        enemy.SetDataRandom(data.lvScale, data.lvTime, data.lvEx);
        activeEnemies.Add(enemy);
        enemy.isCanMove = true;
        enemy.ChangeState(new PatrolState());
    }
    public void DespawnEnemy()
    {
        foreach (EnemyMachine enemy in activeEnemies)
        {
            SimplePool.Despawn(enemy);
        }
        ResetEnemies();
    }
    public void ResetEnemies()
    {
        enemies.Clear();
        activeEnemies.Clear();
    }
    public void PlayPantrol()
    {
        StartCoroutine(IE_PlayPantrol());
    }
    IEnumerator IE_PlayPantrol()
    {
        yield return new WaitForEndOfFrame();
        foreach (EnemyMachine enemy in activeEnemies)
        {
            enemy.isCanMove = true;
            enemy.ChangeState(new PatrolState());
        }
    }
    public void PlayIdle()
    {
        StartCoroutine(IE_PlayIdle());
    }
    IEnumerator IE_PlayIdle()
    {
        yield return new WaitForSeconds(1);
        foreach (EnemyMachine enemy in activeEnemies)
        {
            enemy.isCanMove = false;
            enemy.ChangeState(new IdleState());
        }
    }
    public static Vector3 RandomNavSphere(Vector3 origin, float dist, string areaName)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;
        NavMeshHit navHit;
        NavMeshQueryFilter filter = new NavMeshQueryFilter();
        filter.areaMask = 1 << NavMesh.GetAreaFromName(areaName);
        bool foundPosition = NavMesh.SamplePosition(randDirection, out navHit, dist, filter.areaMask);
        return foundPosition ? navHit.position : origin;
    }
}
