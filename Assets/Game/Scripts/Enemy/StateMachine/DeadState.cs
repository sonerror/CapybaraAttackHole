using UnityEngine;
public class DeadState : IState<EnemyMachine>
{
    public void OnEnter(EnemyMachine t)
    {
        t.OnDead();
        EnemyManager.Ins.enemies.Remove(t);
        EnemyManager.Ins.activeEnemies.Remove(t);
        SimplePool.Despawn(t);
    }
    public void OnExecute(EnemyMachine t)
    {
    }
    public void OnExit(EnemyMachine t)
    {

    }
}