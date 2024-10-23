using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Magnet : GameUnit
{
    [SerializeField] private float pullForce = 5f;
    [SerializeField] private float rotationSpeed = 360f;
    public Character player;
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
                if (poolType == PoolType.Player)
                {
                    LevelManager.Ins.historyMagnetics.Add((int)_target.poolType);
                }
            }
        }
        if (other.CompareTag(Const.TAG_ENEMY_MACHINE) || other.CompareTag(Const.TAG_PLAYER))
        {
            Character _target = other.GetComponentInParent<Character>();
            if (player.lvCurrent > _target.lvCurrent)
            {
                AddToBlackHoleCharacter(_target);
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
                    player.point += enemy.point * bonus;
                    player.CheckPointUpLevel();
                    UpdateUIProgress(player);
                });
        }
    }

    public void AddToBlackHoleCharacter(Character enemy)
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
                    enemy.isDead = true;
                    StartCoroutine(IE_DelaySpawn());

                });
        }
    }
    IEnumerator IE_DelaySpawn()
    {
        yield return new WaitForSeconds(0.1f);
        EnemyManager.Ins.SpawnNewEnemyAfterDeath();
    }
    private void UpdateUIProgress(Character lv)
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
        UIManager.Ins.GetUI<UIGamePlay>().SetProgressSpin(detal / target);
        if (lv.lvCurrent >= 1)
        {
            float targetSkill = (target * 2) / 3;
            UIManager.Ins.GetUI<UIGamePlay>().SetProgressSkill(detal / targetSkill);
        }

    }
}
