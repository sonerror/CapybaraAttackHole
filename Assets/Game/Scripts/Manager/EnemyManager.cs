using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class EnemyManager : Singleton<EnemyManager>
{
    public List<EnemyMachine> enemies = new List<EnemyMachine>();
    public List<EnemyMachine> activeEnemies = new List<EnemyMachine>();
    public List<EnemyMachine> enemyPool = new List<EnemyMachine>();
    public float minDistance;
    [SerializeField] private int quantityInStack;
    [SerializeField] private int quantityInMap;
    [SerializeField] private float radius;
    private PlayerData data;
    public void Oninit()
    {
        data = DataManager.Ins.playerData;
        ResetEnemies();
        for (int i = 0; i < quantityInStack; i++)
        {
            EnemyMachine enemy = SimplePool.Spawn<EnemyMachine>(PoolType.Enemy_Machine, Vector3.zero, Quaternion.identity);
            enemies.Add(enemy);
            enemy.gameObject.SetActive(false);
        }
        SpawmIntoMap(quantityInMap);
    }
    private void SpawmIntoMap(int real)
    {
        for (int i = 0; i < real; i++)
        {
            SwamEnemy();
        }
    }
    private void SwamEnemy()
    {
        EnemyMachine enemy = GetBotFormPool();
        enemy.OnInit();
        enemy.isCanMove = false;
        enemy.SetData(data.lvScale, data.lvTime, data.lvEx);
        if (CheckRamdomPosition(enemy))
        {
            enemy.gameObject.SetActive(true);
            activeEnemies.Add(enemy);
            LevelManager.Ins.characterList.Add(enemy);
        }
    }

    public EnemyMachine GetBotFormPool()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            if (!enemies[i].gameObject.activeInHierarchy)
            {
                return enemies[i];
            }
        }
        EnemyMachine enemy = SimplePool.Spawn<EnemyMachine>(PoolType.Enemy_Machine, Vector3.zero, Quaternion.identity);
        enemy.gameObject.SetActive(false);
        enemies.Add(enemy);
        return enemy;
    }
    public void DespawnEnemy()
    {
        foreach (EnemyMachine enemy in activeEnemies)
        {
            SimplePool.Despawn(enemy);
        }
        activeEnemies.Clear();
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
        yield return new WaitForSeconds(1);
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
        while (!validPosition)
        {
            character.transform.position = RandomNavSphere(character.transform.position, radius, -1);
            validPosition = true;
            foreach (Character otherCharacter in LevelManager.Ins.characterList)
            {
                if (Vector3.Distance(character.transform.position, otherCharacter.transform.position) < minDistance)
                {
                    validPosition = false;
                    break;
                }
            }
        }
        return validPosition;
    }
    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);
        return navHit.position;
    }

}
