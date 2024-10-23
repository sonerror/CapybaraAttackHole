using System;
using System.Diagnostics;

public class PatrolState : IState<EnemyMachine>
{
    public void OnEnter(EnemyMachine t)
    {
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
        }
    }

    public void OnExit(EnemyMachine t)
    {

    }
}
