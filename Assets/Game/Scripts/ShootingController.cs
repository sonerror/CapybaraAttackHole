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
    private const int MaxBullets = 10;
    private float multi;
    public bool IsShooting { get; private set; }
    public bool endShoot;

    private void Awake() => Initialize();

    private void Start()
    {
        spriteCurrent = spriteDown.sprite;
        LevelManager.Ins.isShoot = true;
    }

    private void Initialize()
    {
        IsShooting = false;
        endShoot = false;
    }
    public void ResetBool()
    {
        IsShooting = false;
        endShoot = false;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        spriteDown.sprite = spriteUp;
        if (LevelManager.Ins.isShoot && LevelManager.Ins.historyMagnetics.Count > 0)
            IsShooting = true;
    }

    public void OnPointerUp(PointerEventData eventData) => ResetShooting();

    private void Update()
    {
        var data = LevelManager.Ins;
        if (data.bossTimeUp == null || endShoot) return;
        if (IsShooting) OnShoot();
        if (data.bossTimeUp.point <= 0) IsShooting = data.isShoot = false;
    }

    public void UpdateUI() => tmpCountShoot.text = LevelManager.Ins.historyMagnetics.Count.ToString();

    private void OnShoot()
    {
        var data = LevelManager.Ins;
        if (data.historyMagnetics.Count == 0 || data.bossTimeUp.point <= 0)
            HandleEndShooting();
        else
            ShootBullets(data);
    }

    private void ShootBullets(LevelManager data)
    {
        int bulletsToShoot = Mathf.Min(MaxBullets, data.historyMagnetics.Count);
        for (int i = 0; i < bulletsToShoot; i++)
        {
            Vector3 target = data.bossTimeUp.tfTarget.position + Random.insideUnitSphere * 0.3f;
            ShootBullet(target);
        }
    }

    private void ShootBullet(Vector3 targetPosition)
    {
        var data = LevelManager.Ins;
        int id = data.historyMagnetics[^1];
        var enemyShot = SimplePool.Spawn<Enemy>((PoolType)id);

        enemyShot.HideCollider(false);
        enemyShot.transform.position = data.player.tfCenter.position;
        enemyShot.transform.localScale = Vector3.zero;
        multi = data.player.lvCurrent < 2 ? 20 : 12;

        Vector3 targetPos = data.player.mouth.position;
        DOTween.Sequence()
            .Join(enemyShot.transform.DOScale(new Vector3(1,1,1) /*data.player.transform.localScale * multi*/, 0.25f).SetEase(Ease.Linear))
            .Join(enemyShot.transform.DOMove(targetPos, 0.25f).SetEase(Ease.Linear))
            .OnComplete(() => OnBulletReachTarget(enemyShot, targetPos, targetPosition));
        data.historyMagnetics.RemoveAt(data.historyMagnetics.Count - 1);
        UpdateUI();
    }

    private void OnBulletReachTarget(Enemy bullet, Vector3 startPos, Vector3 targetPos)
    {
        Vector3 apex = (startPos + targetPos) / 2 + Vector3.up * Random.Range(-1f, 3f);
        bullet.transform.DOPath(new[] { startPos, apex, targetPos }, 1.5f, PathType.CatmullRom)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => HandleBulletComplete(bullet));
    }

    private void HandleBulletComplete(Enemy bullet)
    {
        LevelManager.Ins.bossTimeUp.point -= bullet.point * LevelManager.Ins.player.bonusGlod;
        SimplePool.Despawn(bullet);

        StartCoroutine(IE_UpdateUI(() =>
        {
            if (LevelManager.Ins.bossTimeUp.point <= 0 && endShoot)
                HandleBossDefeated();
            else if (LevelManager.Ins.historyMagnetics.Count <= 0 && !endShoot)
                StartCoroutine(IE_ShowUILose());
        }));
    }

    private void HandleEndShooting()
    {
        spriteDown.gameObject.SetActive(false);
        IsShooting = false;
    }

    private void HandleBossDefeated()
    {
        LevelManager.Ins.isShoot = false;
        if (LevelManager.Ins.isCont)
        {
            LevelManager.Ins.isCont = false;
            DataManager.Ins.playerData.levelCurrent++;
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
        endShoot = true;
        yield return new WaitForSeconds(2f);
        UIManager.Ins.OpenUI<PopupReward>().Oninit(true);
    }

    private IEnumerator IE_ShowUILose()
    {
        endShoot = true;
        yield return new WaitForSeconds(2f);
        UIManager.Ins.OpenUI<PopupReward>().Oninit(false);
    }

    private IEnumerator IE_UpdateUI(UnityAction unityAction)
    {
        yield return new WaitForSeconds(0.1f);
        UIManager.Ins.GetUI<UIGamePlay>().UIHPBoss();
        unityAction.Invoke();
    }
}
