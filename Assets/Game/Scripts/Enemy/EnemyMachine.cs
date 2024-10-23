using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMachine : Character
{
    [SerializeField] private Renderer meshRenderer;
    [SerializeField] public NavMeshAgent agent;
    public float timer;
    public IState<EnemyMachine> currentState;
    [SerializeField] private Vector3 nextPoint;
    public Transform tfTarget;
    public bool isCanMove;
    public float wanderTimer;
    public float wanderRadius;
    public override void OnInit()
    {
        base.OnInit();
        timer = 0;
        isDead = false;
        isMagnetic = true;
        SetDataBonusGold();
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
            ChangeState(new DeadState());
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
        Debug.Log(timer + " timer!");

        if (timer >= 5)
        {
            Debug.Log(IsDestination() + " Check!");

            Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
            nextPoint = newPos;
            agent.SetDestination(newPos);
            timer = 0;
        }
      /*  if (IsDestination())
        {
            Debug.Log("idle");
            ChangeState(new IdleState());
        }*/
    }

    public override void CheckPointUpLevel()
    {
        base.CheckPointUpLevel();
        agent.speed = moveSpeed;
    }
    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);
        return navHit.position;
    }
    public bool IsDestination() => Vector3.Distance(transform.position, nextPoint) - Mathf.Abs(transform.position.y - nextPoint.y) < 0.1f;


    #endregion
    public override void SetData(int _Lv, int _lvTime, int _lvEx)
    {
        base.SetData(_Lv, _lvTime, _lvEx);

        /* int rd = Random.Range(0, 2);
         if (rd == 0)
         {
             lvCurrent = _Lv;
         }
         else
         {
             lvCurrent = _Lv + 1;
         }*/

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
            if (lvCurrent < checkPoints.Count - 1)
            {
                lvCurrent = _Lv + 1;
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
