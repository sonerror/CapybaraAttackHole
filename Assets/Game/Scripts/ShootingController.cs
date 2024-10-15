using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShootingController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Sprite spriteUp;
    [SerializeField] private TextMeshProUGUI tmpCountShoot;
    public Image spriteDown;

    public GameObject imgTotal;
    private Sprite spriteCurrent;
    private Coroutine shootingCoroutine;
    public bool IsShooting { get; private set; }

    private const int MaxBullets = 10;

    private void Awake()
    {
        Initialize();
    }

    private void Start()
    {
        spriteCurrent = spriteDown.sprite;
        LevelManager.Ins.isShoot = true;
    }

    private void Initialize()
    {
        IsShooting = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        spriteDown.sprite = spriteUp;

        if (LevelManager.Ins.isShoot && LevelManager.Ins.historyMagnetics.Count > 0)
        {
            IsShooting = true;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ResetShooting();
    }

    private void Update()
    {
        if (LevelManager.Ins.bossTimeUp == null) return;

        if (IsShooting)
        {
            OnShoot();
        }
        if (LevelManager.Ins.bossTimeUp.point <= 0)
        {
            IsShooting = false;
            LevelManager.Ins.isShoot = false;
        }
    }

    public void UpdateUI()
    {
        tmpCountShoot.text = LevelManager.Ins.historyMagnetics.Count.ToString();
    }

    private void OnShoot()
    {
        var data = LevelManager.Ins;

        if (data.historyMagnetics.Count == 0 || data.bossTimeUp.point <= 0)
        {
            HandleEndShooting();
            return;
        }

        ShootBullets(data);
    }

    private void ShootBullets(LevelManager data)
    {
        int numberOfBullets = Mathf.Min(MaxBullets, data.historyMagnetics.Count);
        float spreadAngle = CalculateSpreadAngle(numberOfBullets);
        Vector3 targetDirection = GetTargetDirection(data);

        for (int i = 0; i < numberOfBullets; i++)
        {
            ShootBullet(i, numberOfBullets, spreadAngle, targetDirection);
        }
    }

    private void HandleEndShooting()
    {
        spriteDown.gameObject.SetActive(false);
        IsShooting = false;
    }

    private float CalculateSpreadAngle(int numberOfBullets)
    {
        return Mathf.Lerp(30f, 60f, (float)numberOfBullets / MaxBullets);
    }

    private Vector3 GetTargetDirection(LevelManager data)
    {
        return (data.bossTimeUp.tfTarget.position - data.player.transform.position).normalized;
    }

    private void ShootBullet(int index, int numberOfBullets, float spreadAngle, Vector3 targetDirection)
    {
        var data = LevelManager.Ins;
        float angle = CalculateAngleForBullet(index, numberOfBullets, spreadAngle);
        Vector3 bulletDirection = GetBulletDirection(angle, targetDirection);
        int id = data.historyMagnetics[^1];
        var enemyShot = SimplePool.Spawn<Enemy>((PoolType)id);
        SetupBulletTransform(enemyShot);
        enemyShot.transform.DOMove(LevelManager.Ins.player.mouth.transform.position, 0.1f).SetEase(Ease.InOutQuad).OnComplete(() =>
        {
            Vector3[] path = GenerateBulletPath(bulletDirection);
            MoveBulletAlongPath(enemyShot, path);

            data.historyMagnetics.RemoveAt(data.historyMagnetics.Count - 1);
            if (data.historyMagnetics.Count <= 0 && data.bossTimeUp.point > 0)
            {
                HandleEndShooting();

                StartCoroutine(IE_ShowUILose());
            }
            UpdateUI();
        });
    }

    private float CalculateAngleForBullet(int index, int numberOfBullets, float spreadAngle)
    {
        return -spreadAngle / 2f + (spreadAngle / numberOfBullets) * index;
    }

    private Vector3 GetBulletDirection(float angle, Vector3 targetDirection)
    {
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);
        return rotation * targetDirection;
    }

    private void SetupBulletTransform(Enemy bullet)
    {
        bullet.transform.position = LevelManager.Ins.player.transform.position;
        bullet.transform.localScale = LevelManager.Ins.player.transform.localScale / 5;
    }

    private Vector3[] GenerateBulletPath(Vector3 bulletDirection)
    {
        var data = LevelManager.Ins;
        Vector3 startPosition = data.player.mouth.position;
        Vector3 targetPosition = data.bossTimeUp.tfTarget.position;
        Vector3 apexPoint = startPosition + bulletDirection * 2f + Vector3.up * Random.Range(2f, 6f);

        apexPoint += new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));

        return new Vector3[]
        {
            startPosition,
            apexPoint,
            targetPosition
        };
    }

    private void MoveBulletAlongPath(Enemy bullet, Vector3[] path)
    {
        bullet.transform.DOPath(path, 1f, PathType.CatmullRom)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() => SimplePool.Despawn(bullet));
        HandleBulletComplete(bullet);
    }

    IEnumerator IE_UpdateUI(UnityAction unityAction)
    {
        yield return new WaitForSeconds(0.8f);
        UIManager.Ins.GetUI<UIGamePlay>().UIHPBoss();
        unityAction.Invoke();
    }

    private void HandleBulletComplete(Enemy bullet)
    {
        LevelManager.Ins.bossTimeUp.point -= bullet.point * LevelManager.Ins.player.GetBonusEXP();
        StartCoroutine(IE_UpdateUI(() =>
        {
            if (LevelManager.Ins.bossTimeUp.point <= 0)
            {
                HandleBossDefeated();
            }
            if (LevelManager.Ins.historyMagnetics.Count <= 0 && LevelManager.Ins.bossTimeUp.point > 0)
            {
                StartCoroutine(IE_ShowUILose());
            }
        }));

    }

    private void HandleBossDefeated()
    {
        LevelManager.Ins.isShoot = false;

        if (LevelManager.Ins.isCont)
        {
            LevelManager.Ins.isCont = false;
            DataManager.Ins.playerData.levelCurrent += 1;
            DataManager.Ins.ResetData();
        }
        StartCoroutine(IE_ShowUIWin());
    }

    private void ResetShooting()
    {
        spriteDown.sprite = spriteCurrent;
        IsShooting = false;

        if (shootingCoroutine != null)
        {
            StopCoroutine(shootingCoroutine);
            shootingCoroutine = null;
        }
    }

    private IEnumerator IE_ShowUIWin()
    {
        yield return new WaitForSeconds(2f);
        UIManager.Ins.OpenUI<PopupWin>();
    }

    private IEnumerator IE_ShowUILose()
    {
        yield return new WaitForSeconds(2f);
        UIManager.Ins.OpenUI<PopupLose>();
    }
}
