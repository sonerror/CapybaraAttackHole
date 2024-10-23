using UnityEngine;

public class IdleState : IState<EnemyMachine>
{
    float timer = 0f;
    float idleDuration;

    public void OnEnter(EnemyMachine t)
    {
        idleDuration = Random.Range(1f, 2f); 
        timer = 0f;
    }

    public void OnExecute(EnemyMachine bot)
    {
        if (bot.isDead)
        {
            bot.ChangeState(new DeadState());
        }
        else
        {
            timer += Time.deltaTime;
            if (timer >= 2)
            {
                bot.ChangeState(new PatrolState());
            }
        }
    }

    public void OnExit(EnemyMachine bot)
    {
        timer = 0f;
    }
}
