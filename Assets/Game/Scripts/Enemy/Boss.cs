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
        HideAgent(true);
    }
    private void Update()
    {
        if (isUpdate)
        {
            if (point <= 0)
            {
                isDead = true;
                this.ChangeAnim(Const.ANIM_BOSS_DIE);
                //StopMove();
                HideAgent(false);
                isUpdate = false;
            }
        }
    }
    public void HideAgent(bool isActive)
    {
        agent.enabled = isActive;
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
    private void StopMove()
    {
        agent.speed = 0;
        agent.velocity = Vector3.zero;
    }
    public void CheckIfReachedTarget()
    {
        float distanceToTarget = Vector3.Distance(transform.position, LevelManager.Ins.player.transform.position);
        if (distanceToTarget <= 4f && isAttack)
        {
            isAttack = false;
            HideAgent(false);
            ChangeSpeedAnim(1);
            StartCoroutine(IE_Attack());
        }
    }
    IEnumerator IE_Attack()
    {
        ChangeAnim(Const.ANIM_BOSS_ATTACK);
        yield return new WaitForSeconds(GetTimeAnim() + .5f);
        LevelManager.Ins.player.OnDead();
        yield return new WaitForSeconds(1f);
        ChangeAnim(Const.ANIM_BOSS_IDLE);
    }
}
