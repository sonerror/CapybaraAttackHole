using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Magnet : GameUnit
{
    [SerializeField] private float pullForce = 5f;
    [SerializeField] private float rotationSpeed = 360f;
    public Player player;
    private float pullDuration;
    public Transform blackHoleCenter;

    public virtual void OnTriggerEnter(Collider other)
    {
        if (GameManager.Ins.gameState != GameState.GamePlay) return;

        if (other.CompareTag(Const.TAG_ENEMY))
        {
            Enemy _target = other.GetComponentInParent<Enemy>();
            if (player.lvCurrent >= _target.lvCurrent)
            {
                AddToBlackHole(_target);
                LevelManager.Ins.historyMagnetics.Add((int)_target.poolType);
            }
            /*else
            {
                _target.ChangeColorTriggerEn();
            }*/
        }
        if (other.CompareTag(Const.TAG_ENEMY_MACHINE))
        {
            EnemyMachine _target = other.GetComponentInParent<EnemyMachine>();
            if (player.lvCurrent >= _target.lvCurrent)
            {
                AddToBlackHole1(_target);
            }
        }
    }

    public void AddToBlackHole(Enemy enemy)
    {
        if (enemy != null)
        {
            pullDuration = player.lvCurrent < 1 ? Const.PULLDURATIONMIN : Const.PULLDURATIONMAX;
            float bonus = player.bonusGlod;
            enemy.HideCollider(false);
            Sequence sequence = DOTween.Sequence();
            sequence.Join(enemy.transform.DOMove(blackHoleCenter.position, pullDuration).SetEase(Ease.InExpo))
                .Join(enemy.transform.DOScale(Vector3.zero, pullDuration).SetEase(Ease.InExpo))
                .OnComplete(() =>
                {
                    SimplePool.Despawn(enemy);
                    //player.RemoveTarget(enemy);
                    player.point += enemy.point * bonus;
                    player.CheckPointUpLevel();
                    UpdateUIProgress(player);
                });
        }
    }

    public void AddToBlackHole1(EnemyMachine enemy)
    {
        if (enemy != null)
        {
            pullDuration = player.lvCurrent < 1 ? Const.PULLDURATIONMIN : Const.PULLDURATIONMAX;
            enemy.HideCollider(false);
            Sequence sequence = DOTween.Sequence();
            sequence.Join(enemy.transform.DOMove(blackHoleCenter.position, pullDuration).SetEase(Ease.InExpo))
                .Join(enemy.transform.DOScale(Vector3.one * 0.3f, pullDuration / 2).SetEase(Ease.InExpo))
                .Append(enemy.transform.DOScale(Vector3.zero, pullDuration / 2).SetEase(Ease.InExpo))
                .OnComplete(() =>
                {
                    SimplePool.Despawn(enemy);
                    EnemyManager.Ins.Enemies.Remove(enemy);
                });
        }
    }
    IEnumerator IE_DelaySpawn()
    {
        yield return new WaitForSeconds(0.5f);
        EnemyManager.Ins.SpawmEnemy();
    }
    private void UpdateUIProgress(Player lv)
    {
        float target, root;
        if (lv.lvCurrent > 0)
        {
            target = lv.targetCheckPoint - lv.durProgress;
            root = lv.durProgress;
        }
        else
        {
            target = lv.targetCheckPoint;
            root = 0;
        }
        float detal = lv.point - root;
        Debug.Log(target + " Non" + lv.point + " " + detal);
        UIManager.Ins.GetUI<UIGamePlay>().SetProgressSpin(detal / target);
        if (lv.lvCurrent >= 1)
        {
            float targetSkill = (target * 2) / 3;
            UIManager.Ins.GetUI<UIGamePlay>().SetProgressSkill(detal / targetSkill);
        }

    }

}
