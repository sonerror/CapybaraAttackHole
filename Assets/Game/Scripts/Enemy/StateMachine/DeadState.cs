using System.Collections;
using System.Collections.Generic;
using Unity;
using UnityEngine;
using UnityEngine.UIElements;

public class DeadState : IState<EnemyMachine>
{
    float timer = 0f;
    float time;
    public void OnEnter(EnemyMachine t)
    {
        timer = 2f;
        t.OnDead();
    }
    public void OnExecute(EnemyMachine t)
    {
        time += Time.deltaTime;
        if (time > timer)
        {
            t.ChangeState(new IdleState());
            EnemyManager.Ins.enemies.Remove(t);
            LevelManager.Ins.characterList.Remove(t);
            EnemyManager.Ins.activeEnemies.Add(t);
        }
    }
    public void OnExit(EnemyMachine t)
    {

    }
}