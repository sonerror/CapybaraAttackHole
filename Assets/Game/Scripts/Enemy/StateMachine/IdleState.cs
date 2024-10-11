using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class IdleState : IState<EnemyMachine>
{
    float timer;
    float time = 0f;
    float durationTimeAttack = 1.1f;
    public void OnEnter(EnemyMachine t)
    {
       // t.ChangeAnim();
    }

    public void OnExecute(EnemyMachine t)
    {
        if (t.isDead)
        {
            t.ChangeState(new DeadState());
        }
        else
        {
            timer = Random.Range(2f, 4f);
            if (t.isCanMove)
            {
                if (time > timer && t.listTarget.Count <= 0)
                {
                    t.ChangeState(new PatrolState());
                    time = 0f;
                }
                time += Time.deltaTime;

            }
        }
    }

    public void OnExit(EnemyMachine t)
    {

    }

}