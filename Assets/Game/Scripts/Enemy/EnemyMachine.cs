using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMachine : Character
{
    [SerializeField] private SphereCollider boxCollider;
    [SerializeField] private Renderer meshRenderer;
    [SerializeField] public NavMeshAgent agent;
    [SerializeField] public Animator animator;
    private float timer;
    public IState<EnemyMachine> currentState;
    private Vector3 nextPoint;
    string currentAnim;

    public Transform tfTarget;
    public bool isCanMove;
    public float wanderTimer;
    public float wanderRadius;
    public override void OnInit()
    {
        base.OnInit();
        isDead = false;
        isMagnetic = true;
        SetDataBonusGold();
    }
    #region
    void Update()
    {
        if (GameManager.Ins.gameState != GameState.GamePlay || !isCanMove) return;
        if (currentState != null)
        {
            currentState.OnExecute(this);
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
        agent.enabled = true;
        timer += Time.deltaTime;
        if (timer >= wanderTimer)
        {
            Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
            agent.SetDestination(newPos);
            timer = 0;
        }
        if (IsDestination())
        {
            ChangeState(new IdleState());
        }
    }
    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);
        return navHit.position;
    }
    bool IsDestination() => Vector3.Distance(transform.position, nextPoint) - Mathf.Abs(transform.position.y - nextPoint.y) < 0.1f;


    #endregion
    public override void SetData(int _Lv, int _lvTime, int _lvEx)
    {
        base.SetData(_Lv, _lvTime, _lvEx);
        OnInit();
        SetScale(lvCurrent);
    }
    public void SetScale(float scale)
    {
        this.transform.localScale = new Vector3(1, 1, 1) * scale;
    }    
    public void ChangeAnim(string animName)
    {
        if (currentAnim != animName)
        {
            animator.ResetTrigger(animName);
            currentAnim = animName;
            animator.SetTrigger(currentAnim);
        }
    }
    public void SetPoint(int lv)
    {
        point = LevelManager.Ins.pointData.GetDataWithID(lv).point;
    }
    public void HideCollider(bool isActive)
    {
        boxCollider.enabled = isActive;
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

}
