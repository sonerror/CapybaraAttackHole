using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : GameUnit
{
    [SerializeField] private Collider boxCollider;
    [SerializeField] private Animator animator;
    public List<Character> listTarget = new List<Character>();
    public List<CheckPoint> checkPoints = new List<CheckPoint>();
    string currentAnim;
    public float point = 1;
    public int lvCurrent = 1;
    public bool isDead { get; set; }
    public float targetCheckPoint;
    public float moveSpeed = 5f;
    public float durProgress;
    public bool isMagnetic;
    public int lvTime;
    public int lvEx;
    public float bonusGlod;
    public Transform playerSkill;
    public Transform blackHoleCenter;
    public Transform tfCenter;
    public bool isAttack;

    private void Start()
    {
        listTarget.Clear();
    }
    public virtual void OnInit()
    {
        isDead = false;
    }
    public void PlayAnim(string animName)
    {
        if (animName != null)
        {
            Debug.Log("PlayAnim");
            animator.Play(animName, -1, 0);
            currentAnim = animName;
        }
    }
    public void GetDataLevel(List<CheckPoint> _checkPoint)
    {
        this.checkPoints = new List<CheckPoint>(_checkPoint);
    }
    public virtual void AddTarget(Character character)
    {
        this.listTarget.Add(character);
    }
    public virtual void RemoveTarget(Character character)
    {
        this.listTarget.Remove(character);
    }

    public void ChangeScale(float scale)
    {
        this.transform.DOScale(new Vector3(1, 1, 1) * scale, 0.5f).SetEase(Ease.InOutQuad);
    }
    public virtual void SetScale(int _lvScale)
    {
        var data = GetCheckPointData(_lvScale);
        this.transform.localScale = new Vector3(1, 1, 1) * data.scale;
        ChangeSpeed(data.speedMove);
        SetTargetCheckPoint(_lvScale);
        if (lvCurrent > 0)
        {
            point = GetCheckPointData(lvCurrent - 1).checkPoint;
        }
    }
    public CheckPoint GetCheckPointData(int id)
    {
        return checkPoints.Find(x => x.id == id);
    }
    public void ChangeSpeed(float _speed)
    {
        moveSpeed = _speed;
    }
    private void SetTargetCheckPoint(int _Lv)
    {
        targetCheckPoint = GetCheckPointData(_Lv).checkPoint;
        if (lvCurrent > 0)
        {
            durProgress = GetCheckPointData(_Lv - 1).checkPoint;
        }
    }
    public virtual void CheckPointUpLevel()
    {
        if (lvCurrent < checkPoints.Count && point >= targetCheckPoint)
        {
            lvCurrent += 1;
            var targetPoint = GetCheckPointData(lvCurrent);
            targetCheckPoint = targetPoint.checkPoint;
            if (lvCurrent > 0)
            {
                durProgress = GetCheckPointData(lvCurrent - 1).checkPoint;
            }
            StartCoroutine(IE_Uplevel(targetPoint.speedMove));
            SetScaleUpLevel(lvCurrent);
            isMagnetic = true;
        }
    }
    public virtual void SetData(int _Lv, int _lvTime, int _lvEx)
    {
        this.lvCurrent = _Lv;
        this.lvTime = _lvTime;
        this.lvEx = _lvEx;
    }
    public virtual void SetScaleUpLevel(int _lvScale)
    {
        float targetScale = GetCheckPointData(_lvScale).scale;
        ChangeScale(targetScale);
    }
    IEnumerator IE_Uplevel(float _speed)
    {
        yield return new WaitForEndOfFrame();
        if (moveSpeed != _speed)
        {
            DOTween.To(() => moveSpeed,
                      x => moveSpeed = x,
                      _speed, 0.6f);
        }
    }
    public void SetDataBonusGold()
    {
        bonusGlod = GetBonusEXP();
    }
    public float GetBonusEXP()
    {
        return LevelManager.Ins.levelEx.GetDataWithID(lvEx).bonus;
    }
    public void HideCollider(bool isActive)
    {
        if (boxCollider != null)
        {
            boxCollider.enabled = isActive;

        }
    }
    public void ChangeAnim(string animName)
    {
        if (currentAnim != animName && animator != null)
        {
            animator.ResetTrigger(animName);
            currentAnim = animName;
            animator.SetTrigger(currentAnim);
        }
    }
    public virtual void OnDead()
    {
        moveSpeed = 0;
    }
    public virtual void OnMove()
    {
        ChangeAnim(Const.ANIM_IDLE);
    }
    public virtual void OnEat()
    {
        ChangeAnim(Const.ANIM_EAT);
    }
    public float GetTimeAnim()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.length;
    }
}
