using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyManager : Singleton<EnemyManager>
{
    public List<EnemyMachine> Enemies = new List<EnemyMachine>();
    public float minDistance;
    public void SpawmEnemy()
    {
        if (Enemies.Count < 1)
        {
            EnemyMachine enemy = SimplePool.Spawn<EnemyMachine>(PoolType.Enemy, Vector3.zero, Quaternion.identity);
            Vector3 spawnPos = GetPos();
            enemy.transform.position = spawnPos;
            enemy.HideCollider(true);
            Enemies.Add(enemy);
            enemy.SetScale(0.6f);
            enemy.ChangeState(new PatrolState());
            enemy.isCanMove = true;
            /*DOVirtual.DelayedCall(0.5f, () =>
            {
                UIManager.Ins.OpenUI<UIIndicator>();
            });*/
        }
    }

    public Vector3 GetSpawnPosition()
    {
        Vector3 playerPosition = LevelManager.Ins.player.transform.position;
        Vector3 randomDirection = Random.onUnitSphere * 10f;
        randomDirection.y = playerPosition.y;
        Vector3 spawnPosition = playerPosition + randomDirection;
        NavMeshHit navHit;
        if (NavMesh.SamplePosition(spawnPosition, out navHit, 10f, NavMesh.AllAreas))
        {
            return navHit.position;
        }
        return playerPosition;
    }
    public Vector3 GetPos()
    {
        Vector3 playerPosition = new Vector3(0, LevelManager.Ins.player.transform.position.y, 0);
        Vector3 randomDirection = Random.onUnitSphere * 10f;
        Vector3 spawnPosition = playerPosition + randomDirection;
        NavMeshHit navHit;
        if (NavMesh.SamplePosition(spawnPosition, out navHit, 10f, NavMesh.AllAreas))
        {
            return navHit.position;
        }
        return playerPosition;
    }
    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;

        NavMeshHit navHit;
        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }
    public void DespawnEnemy()
    {
        foreach (EnemyMachine enemy in Enemies)
        {
            SimplePool.Despawn(enemy);
        }
    }
    public void ResetEnemes()
    {
        Enemies.Clear();
    }
}
