using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    [SerializeField] private BoxCollider boxCollider;
   public void Start()
    {
        OnInit();
    }
    public override void OnInit()
    {
        base.OnInit();
    }
    public void HideCollider(bool isActive)
    {
        boxCollider.enabled = isActive;
    }
}
