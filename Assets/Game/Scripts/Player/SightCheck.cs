using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SightCheck : MonoBehaviour
{
    [SerializeField] private Material material;
    public Character Character;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Const.TAG_ENEMY))
        {
            Enemy _target = other.GetComponentInParent<Enemy>();
            if (LevelManager.Ins.player.lvCurrent >= _target.lvCurrent)
            {
                if (!_target.isDead)
                {
                    //Character.AddTarget(_target);
                    Debug.Log("Hit");
                    _target.AddMat(material);
                }
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(Const.TAG_ENEMY))
        {
            Enemy _target = other.GetComponentInParent<Enemy>();
            if (LevelManager.Ins.player.lvCurrent >= _target.lvCurrent)
            {
                if (!_target.isDead)
                {
                    //Character.RemoveTarget(_target);
                    _target.RemoveLastMat();
                }
            }
        }
    }
}
