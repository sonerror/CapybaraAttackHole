using UnityEngine;
public class DeadState : IState<EnemyMachine>
{
    float timer = 0f;
    float time;
    public void OnEnter(EnemyMachine t)
    {
        timer = 0.1f;
        t.OnDead();
    }
    public void OnExecute(EnemyMachine t)
    {
        time += Time.deltaTime;
        if (time > timer)
        {
            EnemyManager.Ins.enemies.Remove(t);
            EnemyManager.Ins.activeEnemies.Remove(t);
            SimplePool.Despawn(t);
        }
    }
    public void OnExit(EnemyMachine t)
    {

    }
}