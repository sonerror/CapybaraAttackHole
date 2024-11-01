using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Boss : Character
{
    public Transform tfTarget;
    public bool isUpdate;
    public NavMeshAgent agent;

    public override void OnInit()
    {
        base.OnInit();
        isDead = false;
        isUpdate = false;
        agent.enabled = true;
        isAttack = true;
    }
    private void Update()
    {
        if (isUpdate)
        {
            if (point <= 0)
            {
                isDead = true;
                this.ChangeAnim(Const.ANIM_BOSS_DIE);
                agent.speed = 0;
                isUpdate = false;
            }
        }
    }
    public void OnMoveBoss()
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(LevelManager.Ins.player.transform.position, out hit, 1.0f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
            ChangeAnim(Const.ANIM_BOSS_RUN);
        }
    }
    public void CheckIfReachedTarget()
    {
        
        float distanceToTarget = Vector3.Distance(transform.position, LevelManager.Ins.player.transform.position);

        if(distanceToTarget <= 15f && isAttack)
        {
            agent.speed = 0;
        }
        if (distanceToTarget <= 10f && isAttack)
        {
            StartCoroutine(IE_Attack());
            isAttack = false;
        }
    }
    IEnumerator IE_Attack()
    {
        yield return new WaitForEndOfFrame();
        ChangeSpeedAnim(1);
        yield return new WaitForEndOfFrame();
        ChangeAnim(Const.ANIM_BOSS_ATTACK);
        yield return new WaitForSeconds(1f);
        LevelManager.Ins.player.point = 0;
        ChangeAnim(Const.ANIM_BOSS_IDLE);
    }
}
