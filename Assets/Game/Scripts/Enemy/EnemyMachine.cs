using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMachine : Character
{
    [SerializeField] private Renderer meshRenderer;
    [SerializeField] private float wanderTimer;
    [SerializeField] private Vector3 nextPoint;

    public NavMeshAgent agent;
    public float timer;
    public IState<EnemyMachine> currentState;
    public Transform tfTarget;
    public bool isCanMove;
    public bool isChangeStateDead;
    public float wanderRadius;
    public override void OnInit()
    {
        base.OnInit();
        timer = 0;
        isDead = false;
        isChangeStateDead = false;
        isMagnetic = true;
        isAttack = true;
        wanderTimer = 1;
        //SetDataBonusGold();
        SetBonusEnemy();
    }

    public void SetBonusEnemy()
    {
        bonusGlod = GetBonusEXP()+5;
    }
    #region
    void Update()
    {
        if (!isDead)
        {
            if (GameManager.Ins.gameState != GameState.GamePlay || !isCanMove) return;
            if (currentState != null)
            {
                currentState.OnExecute(this);
            }
        }
        else
        {
            if (!isChangeStateDead)
            {
                Debug.Log("dead");
                ChangeState(new DeadState());
                isChangeStateDead = true;
            }
        }

    }
    public void ChangeState(IState<EnemyMachine> state)
    {
        if (currentState != null)
        {
            currentState.OnExit(this);
        }
        currentState = state;
        if (currentState != null)
        {
            currentState.OnEnter(this);
        }
    }
    public void Moving()
    {
        timer += Time.deltaTime;
        if (timer >= wanderTimer)
        {
            Vector3 newPos = GetValidNavMeshPosition(transform.position, wanderRadius, Const.AREA_ENEMY_MOVE);
            nextPoint = newPos;
            agent.SetDestination(newPos);
            timer = 0;
            wanderTimer = Random.Range(Const.RANMIN, Const.RANMAX);
        }
    }
    public override void CheckPointUpLevel()
    {
        base.CheckPointUpLevel();
        agent.speed = moveSpeed;
    }
    /*  public static Vector3 RandomNavSphere(Vector3 origin, float dist, string areaName)
      {
          Vector3 randDirection = Random.insideUnitSphere * dist;
          randDirection += origin;
          NavMeshHit navHit;
          NavMeshQueryFilter filter = new NavMeshQueryFilter();
          filter.areaMask = 1 << NavMesh.GetAreaFromName(areaName);
          bool foundPosition = NavMesh.SamplePosition(randDirection, out navHit, dist, filter.areaMask);
          return foundPosition ? navHit.position : origin;
      }*/
    public static Vector3 GetValidNavMeshPosition(Vector3 origin, float initialRadius, string areaName)
    {
        NavMeshQueryFilter filter = new NavMeshQueryFilter
        {
            areaMask = 1 << NavMesh.GetAreaFromName(areaName)
        };

        Vector3 randDirection = Random.insideUnitSphere * initialRadius + origin;
        if (NavMesh.SamplePosition(randDirection, out NavMeshHit navHit, initialRadius, filter.areaMask))
        {
            return navHit.position;
        }

        float reducedRadius = initialRadius * 0.5f;
        randDirection = Random.insideUnitSphere * reducedRadius + origin;
        if (NavMesh.SamplePosition(randDirection, out navHit, reducedRadius, filter.areaMask))
        {
            return navHit.position;
        }

        reducedRadius *= 0.5f;
        randDirection = Random.insideUnitSphere * reducedRadius + origin;
        if (NavMesh.SamplePosition(randDirection, out navHit, reducedRadius, filter.areaMask))
        {
            return navHit.position;
        }

        return LevelManager.Ins.player.transform.position;
    }

    public bool IsDestination() => Vector3.Distance(transform.position, nextPoint) - Mathf.Abs(transform.position.y - nextPoint.y) < 0.1f;


    #endregion
    public override void SetData(int _Lv, int _lvTime, int _lvEx)
    {
        base.SetData(_Lv, _lvTime, _lvEx);
        OnInit();
        SetScale(lvCurrent);
        agent.speed = moveSpeed;
    }
    public void SetDataRandom(int _Lv, int _lvTime, int _lvEx)
    {
        this.lvTime = _lvTime;
        this.lvEx = _lvEx;
        int rd = Random.Range(0, 2);
        if (rd == 0)
        {
            lvCurrent = _Lv;
        }
        else
        {
            if (_Lv < checkPoints.Count - 1)
            {
                lvCurrent = _Lv + 2;
            }
            else
            {
                lvCurrent = _Lv;
            }
        }
        OnInit();
        SetScale(lvCurrent);
        agent.speed = moveSpeed;
    }
    public override void SetScale(int _lvScale)
    {
        base.SetScale(_lvScale);
    }
    public void SetPoint(int lv)
    {
        point = LevelManager.Ins.pointData.GetDataWithID(lv).point;
    }

    public void AddMat(Material mat)
    {
        if (meshRenderer != null && mat != null)
        {
            List<Material> materials = new List<Material>(meshRenderer.materials);
            materials.Add(mat);
            meshRenderer.materials = materials.ToArray();
        }
    }

    public void RemoveLastMat()
    {
        if (meshRenderer != null)
        {
            List<Material> materials = new List<Material>(meshRenderer.materials);
            if (materials.Count > 0)
            {
                materials.RemoveAt(materials.Count - 1);
                meshRenderer.materials = materials.ToArray();
            }
        }
    }
    public void StopMove()
    {
        agent.speed = 0;
    }
    public override void OnDead()
    {
        base.OnDead();
        StopMove();
        LevelManager.Ins.RemoveTarget(this);
    }
}
