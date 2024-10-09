using DG.Tweening;
using UnityEngine;

public class Magnet : MonoBehaviour
{
    [SerializeField] private float pullForce = 5f;
    [SerializeField] private Transform blackHoleCenter;
    [SerializeField] private float rotationSpeed = 360f;

    private Player player;
    private float pullDuration;

    void Start()
    {
        player = LevelManager.Ins.player;
    }

    void OnTriggerEnter(Collider other)
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
            else
            {
                _target.ChangeColorTriggerEn();
            }
        }
    }

    public void AddToBlackHole(Enemy enemy)
    {
        if (enemy != null)
        {
            pullDuration = player.lvCurrent < 1 ? Const.PULLDURATIONMIN : Const.PULLDURATIONMAX;
            float bonus = player.GetBonusEXP();
            enemy.HideCollider(false);
            Sequence sequence = DOTween.Sequence();

            sequence.Join(enemy.transform.DOMove(blackHoleCenter.position, pullDuration).SetEase(Ease.InExpo))
                .Join(enemy.transform.DOScale(Vector3.one * 0.3f, pullDuration / 2).SetEase(Ease.InExpo))
                .Append(enemy.transform.DOScale(Vector3.zero, pullDuration / 2).SetEase(Ease.InExpo))
                .OnComplete(() =>
                {
                    Destroy(enemy.gameObject);
                    player.RemoveTarget(enemy);
                    player.point += enemy.point * bonus;
                    player.CheckPointUpLevel();
                    UpdateUIProgress(player);
                });
        }
    }

    private void UpdateUIProgress(Player lv)
    {
        float target, root;
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
        UIManager.Ins.GetUI<UIGamePlay>().SetProgressSpin(detal / target);
    }
}
