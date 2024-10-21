using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class Enemy : Character
{
    [SerializeField] private Collider boxCollider;
    [SerializeField] private Renderer meshRenderer;
    [SerializeField] public NavMeshAgent agent;
    [SerializeField] public Animator animator;
    private Color orColor;
    string currentAnim;

    public Transform tfTarget;
    public bool isCanMove;
    public float wanderTimer;
    public float wanderRadius;
    public bool isUpdate;
    public void Start()
    {
        OnInit();
        if (orColor != null)
        {
            orColor = GetComponentInChildren<MeshRenderer>().material.color;
        }
    }
    public override void OnInit()
    {
        base.OnInit();
        isDead = false;
        isUpdate = false;
    }
    #region
    /*    void Update()
        {
            if (currentState != null)
            {
                currentState.OnExecute(this);
            }
        }
        public void ChangeState(IState<Enemy> state)
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

        public void OnMoveStop()
        {
            agent.enabled = false;
        }
    */


    #endregion
    private void Update()
    {
        if (isUpdate)
        {
            if (point <= 0)
            {
                isDead = true;
            }
        }
    }
    public void ChangeColorTriggerEn()
    {
        meshRenderer.material.color = MaterialManager.Ins.Setmat();
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
    public void ChangeColorTriggerEX()
    {
        meshRenderer.material.color = orColor;
    }
    public void SetPoint(int lv)// goi khi sinh ra prefabs 
    {
        point = LevelManager.Ins.pointData.GetDataWithID(lv).point;
    }

    public void HideCollider(bool isActive)
    {
        if (boxCollider != null)
        {
            boxCollider.enabled = isActive;
        }
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
    public void Despawn()
    {
        Destroy(gameObject);
    }
}
[System.Serializable]
public class PointData
{
    public List<PointDataModel> points;
    public PointDataModel GetDataWithID(int id)
    {
        return points.Find(x => x.id == id);
    }
}
[System.Serializable]
public class PointDataModel
{
    public int id;
    public float point;
}