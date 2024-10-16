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

        for (int i = 0; i < numberOfBullets; i++)
        {
            Vector3 randomOffset = Random.insideUnitSphere * 0.3f;
            Vector3 startPosition = LevelManager.Ins.player.transform.position + randomOffset;

            float angle = CalculateAngleForBullet(i, numberOfBullets, spreadAngle);
            Vector3 direction = GetBulletDirection(angle, GetTargetDirection(data));

            ShootBullet(startPosition, direction);
        }
    }

    private void HandleEndShooting()
    {
        spriteDown.gameObject.SetActive(false);
        IsShooting = false;
    }

    private float CalculateSpreadAngle(int numberOfBullets)
    {
        return Mathf.Lerp(15f, 60f, (float)numberOfBullets / MaxBullets);
    }

    private float CalculateAngleForBullet(int index, int numberOfBullets, float spreadAngle)
    {
        if (numberOfBullets == 1) return 0;
        return -spreadAngle / 2f + (spreadAngle / (numberOfBullets - 1)) * index;
    }

    private Vector3 GetBulletDirection(float angle, Vector3 targetDirection)
    {
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);
        return rotation * targetDirection;
    }

    private Vector3 GetTargetDirection(LevelManager data)
    {
        return (data.bossTimeUp.tfTarget.position - data.player.transform.position).normalized;
    }

    private void ShootBullet(Vector3 startPosition, Vector3 direction)
    {
        var data = LevelManager.Ins;
        int id = data.historyMagnetics[^1];
        var enemyShot = SimplePool.Spawn<Enemy>((PoolType)id);
        /*enemyShot.transform.position = startPosition;
        enemyShot.transform.localScale = Vector3.one * 0.6f;

        Vector3 targetPosition = data.bossTimeUp.tfTarget.position;
        Vector3 apexPoint = CalculateApex(startPosition, targetPosition);

        enemyShot.transform
            .DOPath(new Vector3[] { startPosition, apexPoint, targetPosition }, 1.5f, PathType.CatmullRom)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => HandleBulletComplete(enemyShot));*/
        enemyShot.transform.position = data.player.transform.position;
        enemyShot.transform.localScale = data.player.transform.localScale*12;
        data.historyMagnetics.RemoveAt(data.historyMagnetics.Count - 1);
        UpdateUI();

        if (data.historyMagnetics.Count <= 0 && data.bossTimeUp.point > 0)
        {
            HandleEndShooting();
            StartCoroutine(IE_ShowUILose());
        }
    }

    private Vector3 CalculateApex(Vector3 start, Vector3 target)
    {
        Vector3 midPoint = (start + target) / 2;
        midPoint += Vector3.up * Random.Range(2f, 4f);
        return midPoint;
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

        SimplePool.Despawn(bullet);
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
        UIManager.Ins.OpenUI<PopupReward>().Oninit(true);
    }

    private IEnumerator IE_ShowUILose()
    {
        yield return new WaitForSeconds(2f);
        UIManager.Ins.OpenUI<PopupReward>().Oninit(false);
    }

    private IEnumerator IE_UpdateUI(UnityAction unityAction)
    {
        yield return new WaitForSeconds(0.8f);
        UIManager.Ins.GetUI<UIGamePlay>().UIHPBoss();
        unityAction.Invoke();
    }
}
