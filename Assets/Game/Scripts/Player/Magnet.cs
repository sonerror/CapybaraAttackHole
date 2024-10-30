using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

[System.Serializable]
public enum TypeMagnet
{
    TypeAnim,
    TypeNoneAnim
}

public class Magnet : GameUnit
{
    [SerializeField] private float pullForce = 5f;
    [SerializeField] private float rotationSpeed = 360f;
    [SerializeField] private TypeMagnet typeMagnet;
    [SerializeField] private float timedur = 2;
    public Character player;
    private float pullDuration;
    public Transform blackHoleCenter;
    private float time = 0;

    private void Awake()
    {
        timedur = player.GetTimeAnim();
        time = timedur + 1;

    }
    private void Update()
    {//13.586 -111.825
        if (typeMagnet == TypeMagnet.TypeAnim && time < timedur + 0.5f)
        {
            if (time > timedur && poolType == PoolType.Player)
            {
                player.OnMove();
                time = 0;
            }
            time += Time.deltaTime;
        }
    }//96 64 36
    public virtual void OnTriggerEnter(Collider other)
    {
        if (GameManager.Ins.gameState != GameState.GamePlay) return;

        if (other.CompareTag(Const.TAG_ENEMY))
        {
            Enemy _target = other.GetComponent<Enemy>();
            if (player.lvCurrent >= _target.lvCurrent)
            {
                if (typeMagnet == TypeMagnet.TypeAnim)
                {
                    StartCoroutine(IE_AddToBlackHoleAnim(_target));
                }
                else
                {
                    StartCoroutine(IE_AddToBlackHole(_target));
                }
                if (poolType == PoolType.Player)
                {
                    LevelManager.Ins.historyMagnetics.Add((int)_target.poolType);
                }
            }
        }
        if (other.CompareTag(Const.TAG_ENEMY_MACHINE) || other.CompareTag(Const.TAG_PLAYER))
        {
            Character _target = other.GetComponentInParent<Character>();
            if (this.player.lvCurrent > _target.lvCurrent && _target.isAttack)
            {
                _target.isAttack = false;
                if (other.CompareTag(Const.TAG_PLAYER))
                {
                    _target.isDead = true;
                    LevelManager.Ins.OnPlayDead();
                }
                else
                {
                    AddToBlackHoleCharacter(_target);
                }

            }
        }
    }

    IEnumerator IE_AddToBlackHole(Enemy enemy)
    {
        if (enemy != null)
        {
            yield return new WaitForEndOfFrame();
            pullDuration = player.lvCurrent < 1 ? Const.PULLDURATIONMIN : Const.PULLDURATIONMAX;
            float bonus = player.bonusGlod;
            enemy.HideCollider(false);
            Sequence sequence = DOTween.Sequence();
            sequence.Join(enemy.transform.DOMove(blackHoleCenter.position, pullDuration).SetEase(Ease.Linear))
                .Join(enemy.transform.DOScale(Vector3.zero, pullDuration).SetEase(Ease.Linear))
                .OnComplete(() =>
                {
                    SimplePool.Despawn(enemy);
                    player.point += enemy.point * bonus;
                    player.CheckPointUpLevel();
                    if (poolType == PoolType.Player)
                    {
                        UpdateUIProgress(player);
                    }
                });
        }
    }
    IEnumerator IE_AddToBlackHoleAnim(Enemy enemy)
    {
        if (enemy != null)
        {
            yield return new WaitForEndOfFrame();
            time = 0;
            player.PlayAnim(Const.ANIM_EAT);
            float bonus = player.bonusGlod;
            enemy.HideCollider(false);
            DOVirtual.DelayedCall(0.05f, () =>
            {
                pullDuration = player.lvCurrent < 1 ? Const.PULLDURATIONMIN : Const.PULLDURATIONMAX;
                enemy.transform.SetParent(blackHoleCenter);
                Sequence sequence = DOTween.Sequence();
                sequence.Join(enemy.transform.DOMove(blackHoleCenter.position, pullDuration).SetEase(Ease.Linear))
                    .Join(enemy.transform.DOScale(Vector3.zero, pullDuration).SetEase(Ease.Linear))
                    .OnComplete(() =>
                    {
                        SimplePool.Despawn(enemy);
                        player.point += enemy.point * bonus;
                        player.CheckPointUpLevel();
                        if (poolType == PoolType.Player)
                        {
                            UpdateUIProgress(player);
                        }
                    });
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
                .Join(enemy.transform.DOScale(enemy.transform.localScale * 0.3f, pullDuration / 2).SetEase(Ease.InExpo))
                .Append(enemy.transform.DOScale(Vector3.zero, pullDuration / 2).SetEase(Ease.InExpo))
                .OnComplete(() =>
                {
                    enemy.isDead = true;
                    StartCoroutine(IE_DelaySpawn());
                });
        }
    }
    IEnumerator IE_DelaySpawn()
    {
        yield return new WaitForSeconds(0.1f);
        EnemyManager.Ins.SpawmIntoMapAfterDeath();
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
