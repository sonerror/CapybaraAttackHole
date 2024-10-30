using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Character
{
    public Transform tfTarget;
    public bool isUpdate;
    public override void OnInit()
    {
        base.OnInit();
        isDead = false;
        isUpdate = false;

    }
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
}
