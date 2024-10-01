using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SightCheck : MonoBehaviour
{
    public Character Character;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Const.TAG_ENEMY))
        {
            Character _target = other.GetComponentInParent<Character>();
            if (!_target.isDead)
            {
                Character.AddTarget(_target);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(Const.TAG_ENEMY))
        {
            Character _target = other.GetComponentInParent<Character>();
            Character.RemoveTarget(_target);
        }
    }
}
