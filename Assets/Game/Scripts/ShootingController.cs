using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
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
    public List<CheckPoint> checkPoints = new List<CheckPoint>();
    public bool IsShooting { get; private set; }
    public bool endShoot;
    public int LevelPlayer;
    public float scalePlayer;
    public float pointPlayer;
    public float currenPoint;
    public float targetCheckPoint;
    private float curPoint;
    public bool isMove;
    public bool isAttack;
    public bool isShowPopup;
    public bool isShowPopupLose;
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
        isMove = false;
    }
    public void ResetData()
    {
        isMove = false;
    }
    public void ResetBool()
    {
        IsShooting = false;
        endShoot = false;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        spriteDown.sprite = spriteUp;
        if (LevelManager.Ins.isShoot && LevelManager.Ins.bossTimeUp.point > 0 && LevelManager.Ins.infoEnemyList.Count > 0)
            IsShooting = true;
    }
    public void OnPointerUp(PointerEventData eventData) => ResetShooting();

    private void Update()
    {
        var data = LevelManager.Ins;
        if (data.bossTimeUp == null) return;
        if (!data.bossTimeUp.isDead)
        {
            data.bossTimeUp.CheckIfReachedTarget();
            if (LevelManager.Ins.player.point <= 0&& isShowPopup)
            {
                HandleEndShooting();
                StartCoroutine(IE_ShowUILose(1f));
                isShowPopup = false;
            }
        }
        if (data.bossTimeUp == null || endShoot || data.bossTimeUp.isDead) return;
        if (data.bossTimeUp.point <= 0)
        {
            IsShooting = false;
            data.isShoot = false;
            return;
        }
        if (IsShooting) OnShoot();
    }
    public void UpdateUI() => tmpCountShoot.text = LevelManager.Ins.infoEnemyList.Count.ToString();

    private void OnShoot()
    {
        var data = LevelManager.Ins;
        if (data.bossTimeUp.point <= 0 || data.infoEnemyList.Count == 0)
            HandleEndShooting();
        else
            ShootBullets(data);
    }

    private void ShootBullets(LevelManager data)
    {
        if (!isMove)
        {
            LevelManager.Ins.bossTimeUp.OnMoveBoss();
            isMove = true;
        }
        LevelManager.Ins.bossTimeUp.agent.speed = 0;
        int bulletsToShoot = Mathf.Min(Const.MAX_BULLETS, data.infoEnemyList.Count);
        for (int i = 0; i < bulletsToShoot; i++)
        {
            Vector3 target = data.bossTimeUp.tfTarget.position + Random.insideUnitSphere * 0.5f;
            ShootBullet(target);
        }
    }

    private void ShootBullet(Vector3 targetPosition)
    {
        var data = LevelManager.Ins;
        InforEnemy infor = data.infoEnemyList[data.infoEnemyList.Count - 1];
        int id = infor.poolType;
        float scale = infor.scale;
        var enemyShot = SimplePool.Spawn<Enemy>((PoolType)id);
        curPoint = infor.point * data.player.bonusGlod;
        pointPlayer -= curPoint;
        CheckPointLv(data.player);
        enemyShot.HideCollider(false);
        enemyShot.transform.position = data.player.tfCenter.position;
        enemyShot.transform.localScale = Vector3.zero;
        Vector3 targetPos = data.player.mouth.position;
        DOTween.Sequence()
            .Join(enemyShot.transform.DOScale(new Vector3(scale, scale, scale), 0.2f).SetEase(Ease.Linear))
            .Join(enemyShot.transform.DOMove(targetPos, 0.2f).SetEase(Ease.Linear))
            .OnComplete(() =>
            {
                OnBulletReachTarget(enemyShot, targetPos, targetPosition);
            });
        data.infoEnemyList.RemoveAt(data.infoEnemyList.Count - 1);
        UpdateUI();
    }
    private void OnBulletReachTarget(Enemy bullet, Vector3 startPos, Vector3 targetPos)
    {
        bullet.transform.DOPath(new[] { startPos, RandomApex(startPos, targetPos), targetPos }, 0.4f, PathType.CatmullRom)
            .SetEase(Ease.Linear)
            .OnComplete(() => HandleBulletComplete(bullet));
    }
    private Vector3 RandomApex(Vector3 startPos, Vector3 targetPos)
    {
        Vector3 midPoint = (startPos + targetPos) / 2;
        Vector3 apex = midPoint + Vector3.up * Random.Range(0.1f, 1.75f);
        Vector3 apex1 = midPoint + Vector3.left * Random.Range(0.1f, 1f);
        Vector3 apex2 = midPoint + Vector3.right * Random.Range(0.1f, 1f);
        Vector3 apex3 = midPoint + Vector3.down * Random.Range(0.1f, 1f);
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
        var data = LevelManager.Ins;
        data.bossTimeUp.point -= curPoint;
        SimplePool.Despawn(bullet);
        IE_UpdateUI();
        if (data.bossTimeUp.point <= 0 && !endShoot)
        {
            HandleBossDefeated();
        }
        else if (!endShoot && data.infoEnemyList.Count <= 0)
        {
            endShoot = true;
            StartCoroutine(IE_ShowUILose(5f));
        }
    }
    public void CheckPointLv(Player player)
    {
        if (pointPlayer < targetCheckPoint && LevelPlayer >= 0)
        {
            if (LevelPlayer > 0)
            {
                LevelPlayer--;
            }
            TargetCheckPoint();
            player.ChangeScale(GetDataScale(), 1f);
            player.ChangeCamera(checkPoints.Find(x => x.id == LevelPlayer).cameraDistance);
        }
    }
    public float GetDataScale()
    {
        return checkPoints.Find(x => x.id == LevelPlayer).scale;
    }
    public void GetDataLevel(List<CheckPoint> _checkPoint, int lvPlayer, float pointBoss)
    {
        LevelPlayer = lvPlayer;
        pointPlayer = pointBoss;
        isShowPopup = true;
        isShowPopupLose = true;
        this.checkPoints = new List<CheckPoint>(_checkPoint);
        if (checkPoints.Count > 0)
        {
            TargetCheckPoint();
        }
        ResetData();
    }

    public void TargetCheckPoint()
    {
        if (LevelPlayer < 1)
        {
            targetCheckPoint = 0;
        }
        else
        {
            targetCheckPoint = checkPoints.Find(x => x.id == LevelPlayer - 1).checkPoint;
        }
    }
    public Vector3 UpdateScale(float points)
    {
        currenPoint = Mathf.Clamp(currenPoint - points, 0, pointPlayer);
        float scaleMultiplier = (float)currenPoint / pointPlayer;
        return Vector3.one * (scalePlayer * scaleMultiplier);
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
        if (!endShoot)
        {
            LevelManager.Ins.bossTimeUp.agent.speed = 3.5f;
        }
        else
        {
            LevelManager.Ins.bossTimeUp.agent.speed = 30f;
            LevelManager.Ins.bossTimeUp.ChangeSpeedAnim(2);
            Debug.Log("check");

        }
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

    private IEnumerator IE_ShowUILose(float timeDur)
    {
        if(isShowPopupLose)
        {
            isShowPopupLose = false;
            yield return new WaitForSeconds(timeDur);
            UIManager.Ins.OpenUI<PopupReward>().Oninit(false);

        }
    }
    private void IE_UpdateUI()
    {
        UIManager.Ins.GetUI<UIGamePlay>().UIHPBoss();
    }
}
