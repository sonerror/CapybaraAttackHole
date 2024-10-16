using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorBoss : MonoBehaviour
{
    public Transform targetPlayerMove;
    public Transform tfBoss;
    public Collider colliderFl;
    public void HideCollider(bool isEnabled)
    {
        colliderFl.enabled = isEnabled;
    }
}
