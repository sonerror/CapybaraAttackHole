using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class PatrolState : IState<EnemyMachine>
{
    float timer;
    float time;
    public void OnEnter(EnemyMachine t)
    {
        time = 0f;
        timer = 1.1f;
    }

    public void OnExecute(EnemyMachine t)
    {
        if (t.isDead)
        {
            t.ChangeState(new DeadState());
        }
        else
        {
            t.Moving();
            time += Time.deltaTime;
            if (t.listTarget.Count > 0 && time > timer)
            {
                t.ChangeState(new IdleState());
                time = 0f;
            }
        }
    }
    public void OnExit(EnemyMachine t)
    {

    }
}