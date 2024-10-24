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
    private const int MaxBullets = 3;
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
        if (LevelManager.Ins.isShoot && LevelManager.Ins.historyMagnetics.Count > 0 && LevelManager.Ins.bossTimeUp.point > 0)
            IsShooting = true;
    }

    public void OnPointerUp(PointerEventData eventData) => ResetShooting();

    private void Update()
    {
        var data = LevelManager.Ins;
        if (data.bossTimeUp == null || endShoot || data.bossTimeUp.isDead) return;
        if (data.bossTimeUp.point <= 0)
        {
            IsShooting = false;
            data.isShoot = false;
            return;

        }
        if (IsShooting) OnShoot();
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
            Vector3 target = data.bossTimeUp.tfTarget.position + Random.insideUnitSphere * 0.5f;
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
        Vector3 targetPos = data.player.mouth.position;
        if (enemyShot.lvCurrent < 1)
        {
            multi = 3;
        }
        else
        {
            multi = 1;
        }
        DOTween.Sequence()
            .Join(enemyShot.transform.DOScale(new Vector3(1, 1, 1) * multi, 0.2f).SetEase(Ease.Linear))
            .Join(enemyShot.transform.DOMove(targetPos, 0.2f).SetEase(Ease.Linear))
            .OnComplete(() =>
            {
                OnBulletReachTarget(enemyShot, targetPos, targetPosition);
            });
        data.historyMagnetics.RemoveAt(data.historyMagnetics.Count - 1);
        UpdateUI();
    }
    private void OnBulletReachTarget(Enemy bullet, Vector3 startPos, Vector3 targetPos)
    {
        /* Vector3 apex = (startPos + targetPos) / 2 + Vector3.up * Random.Range(-1f, 3f);
         Vector3 apex1 = (startPos + targetPos) / 2 + Vector3.left * Random.Range(-1f, 3f);
         Vector3 apex2 = (startPos + targetPos) / 2 + Vector3.right * Random.Range(-1f, 3f);
 */
        bullet.transform.DOPath(new[] { startPos, RandomApex(startPos, targetPos), targetPos }, 0.4f, PathType.CatmullRom)
            .SetEase(Ease.Linear)
            .OnComplete(() => HandleBulletComplete(bullet));
    }
    private Vector3 RandomApex(Vector3 startPos, Vector3 targetPos)
    {
        Vector3 midPoint = (startPos + targetPos) / 2;
        Vector3 apex = midPoint + Vector3.up * Random.Range(0.1f, 1.5f);
        Vector3 apex1 = midPoint + Vector3.left * Random.Range(0.1f, 0.75f);
        Vector3 apex2 = midPoint + Vector3.right * Random.Range(0.1f, 0.75f);
        Vector3 apex3 = midPoint + Vector3.down * Random.Range(0.1f, 0.75f);
        Vector3 apex4 = midPoint + (Vector3.left + Vector3.up).normalized * Random.Range(0.1f, 1f);
        Vector3 apex5 = midPoint + (Vector3.right + Vector3.up).normalized * Random.Range(0.1f, 1f);
        int randomIndex = Random.Range(0, 6); 
        switch (randomIndex)
        {
            case 0: return apex;
            case 1: return apex1;
            case 2: return apex2;
            case 3: return apex3;
            case 4: return apex4;
            case 5: return apex5;
        }
        return apex; 
    }
    private void HandleBulletComplete(Enemy bullet)
    {
        LevelManager.Ins.bossTimeUp.point -= bullet.point * LevelManager.Ins.player.bonusGlod;
        SimplePool.Despawn(bullet);
        IE_UpdateUI();
        if (LevelManager.Ins.bossTimeUp.point <= 0 && !endShoot)
        {
            Debug.Log("Win 1");
            HandleBossDefeated();
        }
        else if (LevelManager.Ins.historyMagnetics.Count <= 0 && !endShoot)
        {
            Debug.Log("Win 2");
            StartCoroutine(IE_ShowUILose());
        }
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
    private void IE_UpdateUI()
    {
        UIManager.Ins.GetUI<UIGamePlay>().UIHPBoss();
    }
}
