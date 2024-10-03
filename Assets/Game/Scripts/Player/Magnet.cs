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
        if (GameManager.Ins.gameState != GameState.GamePlay) return;
        if (other.CompareTag(Const.TAG_ENEMY))
        {
            Enemy _target = other.GetComponentInParent<Enemy>();
            if (LevelManager.Ins.player.lvCurrent >= _target.lvCurrent)
            {
                AddToBlackHole(_target);
                LevelManager.Ins.historyMagnetics.Add(_target.lvCurrent);
            }
        }
    }
    public void AddToBlackHole(Enemy enemy)
    {
        if (enemy != null)
        {
            var lv = LevelManager.Ins.player;
            float bonus = lv.GetBonusEXP();
            float root;
            float target;
            enemy.HideCollider(false);
            Sequence sequence = DOTween.Sequence();
            sequence.Join(enemy.transform.DOMove(blackHoleCenter.position, pullDuration).SetEase(Ease.InQuad));
            sequence.Join(enemy.transform.DOScale(new Vector3(1, 1, 1) * 0.3f, pullDuration).SetEase(Ease.InQuad));
            sequence.OnComplete(() =>
            {
                Destroy(enemy.transform.gameObject);
                lv.RemoveTarget(enemy);
                float bonusEx = enemy.point * bonus;
                lv.point += bonusEx;
                lv.CheckPointUpLevel();
                if (lv.lvCurrent > 0)
                {
                    target = lv.GetCheckPoint(lv.lvCurrent) - lv.GetCheckPoint(lv.lvCurrent - 1);
                    root = lv.GetCheckPoint(lv.lvCurrent - 1);
                }
                else
                {
                    target = lv.GetCheckPoint(lv.lvCurrent);
                    root = 0;
                }
                float detal = lv.point - root;
                UIManager.Ins.GetUI<UIGamePlay>().SetProgressSpin((float)detal / target);
            });
        }
    }
}
