using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class FloorBoss : MonoBehaviour
{
    [SerializeField] private NavMeshSurface navMeshSurface;
    public Transform targetPlayerMove;
    public Transform tfBoss;
    public Collider colliderFl;
    private void Awake()
    {
        OnEnableNavMesh(false);
    }
    public void OnEnableNavMesh(bool isActive)
    {
        navMeshSurface.enabled = isActive;
    }
    public void HideCollider(bool isEnabled)
    {
        colliderFl.enabled = isEnabled;
    }
}
