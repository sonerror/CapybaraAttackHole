using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShootingController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Sprite spriteUp;
    private Sprite spriteCurrent;
    public bool isShooting;
    public Image spriteDown;

    public void Start()
    {
        spriteCurrent = spriteDown.sprite;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        isShooting = true;
        spriteDown.sprite = spriteUp;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        isShooting = false;
        spriteDown.sprite = spriteCurrent;
        LevelManager.Ins.bossTimeUp.CheckWinLose();
    }
    void Update()
    {
        if (isShooting)
        {
            OnShoot();
        }
    }
    private void OnShoot()
    {
        var data = LevelManager.Ins;
        Debug.LogError("Fire");
        if (data.historyMagnetics.Count > 0)
        {
            if (data.bossTimeUp.point > 0)
            {
                int numberOfBullets = Mathf.Min(3, data.historyMagnetics.Count);
                float spreadAngle = CalculateSpreadAngle(numberOfBullets);
                Vector3 targetDirection = GetTargetDirection(data);

                for (int i = 0; i < numberOfBullets; i++)
                {
                    ShootBullet(i, numberOfBullets, spreadAngle, targetDirection);
                }
            }
            else
            {
                spriteDown.gameObject.SetActive(false);
                isShooting = false;
            }
        }
        else
        {
            spriteDown.gameObject.SetActive(false);
            isShooting = false;
        }

    }

    /// <summary>
    /// Tính toán góc bắn dựa trên số lượng đạn.
    /// </summary>
    private float CalculateSpreadAngle(int numberOfBullets)
    {
        return Mathf.Lerp(30f, 60f, (float)numberOfBullets / 30f);
    }

    /// <summary>
    /// Lấy hướng của mục tiêu dựa trên vị trí của người chơi và mục tiêu.
    /// </summary>
    private Vector3 GetTargetDirection(LevelManager data)
    {
        return (data.bossTimeUp.tfTarget.position - data.player.transform.position).normalized;
    }

    /// <summary>
    /// Xử lý việc bắn một viên đạn.
    /// </summary>
    private void ShootBullet(int index, int numberOfBullets, float spreadAngle, Vector3 targetDirection)
    {
        var data = LevelManager.Ins;
        float angle = CalculateAngleForBullet(index, numberOfBullets, spreadAngle);
        Vector3 bulletDirection = GetBulletDirection(angle, targetDirection);
        int id = data.historyMagnetics[data.historyMagnetics.Count - 1];
        var enemyShot = SimplePool.Spawn<Enemy>((PoolType)id);
        SetupBulletTransform(enemyShot, bulletDirection);
        Vector3[] path = GenerateBulletPath(bulletDirection);
        MoveBulletAlongPath(enemyShot, path);
        data.historyMagnetics.RemoveAt(data.historyMagnetics.Count - 1);
    }

    /// <summary>
    /// Tính toán góc bắn cho mỗi viên đạn.
    /// </summary>
    private float CalculateAngleForBullet(int index, int numberOfBullets, float spreadAngle)
    {
        float startAngle = -spreadAngle / 2f;
        return startAngle + (spreadAngle / numberOfBullets) * index;
    }

    /// <summary>
    /// Lấy hướng bay của viên đạn dựa trên góc tính toán.
    /// </summary>
    private Vector3 GetBulletDirection(float angle, Vector3 targetDirection)
    {
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);
        return rotation * targetDirection;
    }

    /// <summary>
    /// Thiết lập vị trí và kích thước của viên đạn.
    /// </summary>
    private void SetupBulletTransform(Enemy bullet, Vector3 bulletDirection)
    {
        var data = LevelManager.Ins;
        bullet.transform.position = data.player.transform.position;
        bullet.transform.localScale = new Vector3(2, 2, 2);  // Kích thước đạn
    }

    /// <summary>
    /// Tạo ra đường bay cho viên đạn (theo dạng vòng cung).
    /// </summary>
    private Vector3[] GenerateBulletPath(Vector3 bulletDirection)
    {
        var data = LevelManager.Ins;
        return new Vector3[]
        {
        data.player.transform.position + bulletDirection * 1 + new Vector3(0, Random.Range(1f, 2f), 0),
        data.player.transform.position + bulletDirection * 2f + new Vector3(0, Random.Range(2f, 4f), 0),
        data.bossTimeUp.tfTarget.position
        };
    }

    /// <summary>
    /// Di chuyển viên đạn theo đường bay đã tạo.
    /// </summary>
    private void MoveBulletAlongPath(Enemy bullet, Vector3[] path)
    {
        bullet.transform.DOPath(path, 2f, PathType.CatmullRom)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                SimplePool.Despawn(bullet);
                Debug.LogError("Hit");
                LevelManager.Ins.bossTimeUp.point -= bullet.point * LevelManager.Ins.player.GetBonusEXP();
            });
    }

    /// <summary>
    /// Xử lý khi trò chơi kết thúc (thắng hoặc thua).
    /// </summary>
    private void HandleEndGameCondition(LevelManager data)
    {
        if (data.bossTimeUp.point > 0)
        {
            Debug.LogError("Lose");
        }
        else
        {
            Debug.LogError("Win");
        }
    }
    public void OnLoadStage()
    {
        LevelManager.Ins.ReLoad();
        DOVirtual.DelayedCall(1f, () =>
        {
            SceneController.Ins.ChangeScene(Const.GAMEPLAY_SCENE, () =>
            {
                UIManager.Ins.OpenUI<UIHome>();
                LevelManager.Ins.OnLoadLevel();
                GameManager.Ins.ChangeState(GameState.MainMenu);
            });
        });
    }

}
