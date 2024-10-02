using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SightCheck : MonoBehaviour
{
    [SerializeField] private Material material;
   public Character Character;
    private Dictionary<Enemy, Material> originalMaterials = new Dictionary<Enemy, Material>();
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Const.TAG_ENEMY))
        {
            Enemy _target = other.GetComponentInParent<Enemy>();
            if (!_target.isDead)
            {
                Character.AddTarget(_target);
                _target.AddMat(material);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(Const.TAG_ENEMY))
        {
            Enemy _target = other.GetComponentInParent<Enemy>();
            Character.RemoveTarget(_target);
            _target.RemoveLastMat();
        }
    }
}
