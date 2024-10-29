using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SightCheck : MonoBehaviour
{
    [SerializeField] private Material material;
    public Character character;
/*    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag(Const.TAG_ENEMY))
        {
            Enemy _target = other.GetComponent<Enemy>();
            if (character.lvCurrent >= _target.lvCurrent)
            {
                if (!_target.isDead)
                {
                    _target.AddMat(material);
                }
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {

        if (other.CompareTag(Const.TAG_ENEMY))
        {
            Enemy _target = other.GetComponent<Enemy>();
            if (character.lvCurrent >= _target.lvCurrent)
            {
                if (!_target.isDead)
                {
                    _target.RemoveLastMat();
                }
            }
        }
    }*/
}
