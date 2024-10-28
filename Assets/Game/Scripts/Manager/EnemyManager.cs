using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using System.Collections;

public class EnemyManager : Singleton<EnemyManager>
{
    public List<EnemyMachine> enemies = new List<EnemyMachine>();
    public List<Character> enemyPool = new List<Character>();
    public List<EnemyMachine> activeEnemies = new List<EnemyMachine>();
    public float minDistance;
    [SerializeField] private int quantityInStack;
    [SerializeField] private int quantityInMap;
    [SerializeField] private float radius;
    private PlayerData data;
    public void Oninit(Player player)
    {
        data = DataManager.Ins.playerData;
        ResetEnemies();
        enemyPool.Add(player);
        StartCoroutine(IE_CreateEnemy());
    }
    IEnumerator IE_CreateEnemy()
    {
        yield return new WaitForEndOfFrame();
        for (int i = 0; i < quantityInStack; i++)
        {
            EnemyMachine enemy = SimplePool.Spawn<EnemyMachine>(PoolType.Enemy_Machine, Vector3.zero, Quaternion.identity);
            enemy.gameObject.SetActive(false);
            if (CheckRamdomPosition(enemy))
            {
                enemies.Add(enemy);
                enemyPool.Add(enemy);
            }
        }
    }
    public void SpawmIntoMap()
    {
        int count = System.Math.Min(quantityInMap, enemies.Count);
        for (int i = 0; i < count; i++)
        {
            StartCoroutine(IE_SpawmEnemyIntoMap(enemies[i]));
        }
    }
    IEnumerator IE_SpawmEnemyIntoMap(EnemyMachine enemy)
    {
        yield return new WaitForEndOfFrame();
        enemies.Remove(enemy);
        int validLevelID = data.levelCurrent % LevelManager.Ins._levelData.levels.Count;
        enemy.isCanMove = true;
        enemy.GetDataLevel(LevelManager.Ins._levelData.GetDataWithID(validLevelID).checkPoints);
        enemy.SetDataRandom(data.lvScale, data.lvTime, data.lvEx);
        enemy.gameObject.SetActive(true);
        activeEnemies.Add(enemy);
        LevelManager.Ins.characterList.Add(enemy);
        enemy.ChangeState(new PatrolState());
    }
    public void SpawmIntoMapAfterDeath()
    {
        if (enemies.Count > 0)
        {
            StartCoroutine(IE_SpawnNewEnemyAfterDeath(enemies[0]));
        }
    }
    IEnumerator IE_SpawnNewEnemyAfterDeath(EnemyMachine enemy)
    {
        Debug.Log("SPN");
        yield return new WaitForEndOfFrame();
        enemies.Remove(enemy);
        int validLevelID = data.levelCurrent % LevelManager.Ins._levelData.levels.Count;
        enemy.GetDataLevel(LevelManager.Ins._levelData.GetDataWithID(validLevelID).checkPoints);
        enemy.SetDataRandom(data.lvScale, data.lvTime, data.lvEx);
        enemy.gameObject.SetActive(true);
        activeEnemies.Add(enemy);
        LevelManager.Ins.characterList.Add(enemy);
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
        enemyPool.Clear();
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
    public bool CheckRamdomPosition(Character character)
    {
        bool validPosition = false;
        int maxAttempts = 10;
        int attempts = 0;

        while (!validPosition && attempts < maxAttempts)
        {
            Vector3 randomPosition = RandomNavSphere(character.transform.position, radius, "SpawnArea");

            character.transform.position = randomPosition;

            validPosition = true;

            foreach (Character otherCharacter in enemyPool)
            {
                if (Vector3.Distance(character.transform.position, otherCharacter.transform.position) < minDistance)
                {
                    validPosition = false;
                    break;
                }
            }
            attempts++;
        }
        return validPosition;
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
