using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Magnet : MonoBehaviour
{
    [SerializeField] private float pullForce;
    [SerializeField] private float pullDuration = 2f;
    [SerializeField] private Transform blackHoleCenter;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Const.TAG_ENEMY))
        {
            Enemy _target = other.GetComponentInParent<Enemy>();
            if (LevelManager.Ins.player.score >= _target.score)
            {
                AddToBlackHole(_target);
            }
        }
    }
    public void AddToBlackHole(Enemy enemy)
    {
        if (enemy != null)
        {
            enemy.HideCollider(false);
            Sequence sequence = DOTween.Sequence();
            sequence.Join(enemy.transform.DOMove(blackHoleCenter.position, pullDuration)
                    .SetEase(Ease.InQuad));
            sequence.Join(enemy.transform.DOScale(new Vector3(1, 1, 1) * 0.3f, pullDuration)
                    .SetEase(Ease.InQuad));
            sequence.OnComplete(() =>
            {
                Destroy(enemy.transform.gameObject);
                LevelManager.Ins.player.RemoveTarget(enemy);
                LevelManager.Ins.player.score += enemy.score;
            });
        }
    }
}
