using AssetKits.ParticleImage;
using CW.Common;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class PopupReward : UICanvas
{
    [SerializeField] private Animator animDollSpin;
    [SerializeField] private UIGoldReward uIGold;
    [SerializeField] private ParticleImage confesti;
    [SerializeField] private Transform tfStart;
    [SerializeField] private RotatePin rotatePin;
    private bool isClick;
    public bool isWin;
    private float multiplier;
    private int amountBonus;

    public override void Open()
    {
        base.Open();
        PlayAnim();
        confesti.gameObject.transform.position = tfStart.position;
        confesti.gameObject.SetActive(false);
        isClick = true;
        Debug.Log("Open");
    }
    public void BtnHome(float multiplier, int amount)
    {
        CollectGold(multiplier, amount, () =>
        {
            LevelManager.Ins.ReloadScene();
        });
    }
    private void CollectGold(float multiplier, int amount, UnityAction OnComplete = null)
    {
        confesti.gameObject.SetActive(true);
        confesti.rateOverTime = 150;
        confesti.Play();
        DOVirtual.DelayedCall(1.3f, () =>
        {
            DataManager.Ins.ChangeGold((int)(amount * multiplier));
            DOVirtual.DelayedCall(1.5f, () =>
            {
                confesti.gameObject.SetActive(false);
                OnComplete?.Invoke();
            });
        });
    }
    public void Oninit(bool _isWin)
    {
        this.isWin = _isWin;
        if (isWin)
        {
            amountBonus = Const.BONUSWIN;
        }
        else
        {
            amountBonus = Const.BONUSLOSE;
        }
        uIGold.SetData(amountBonus);
        rotatePin.Oninit(amountBonus);
    }
    public void PlayAnim()
    {
        animDollSpin.Play("Doll_Spin");
        animDollSpin.speed = 1;
    }
    public void BtnNoThanks()
    {
        if (isClick)
        {
            isClick = false;
            Debug.Log("NoThank");
            StopAnim();
            BtnHome(1, amountBonus);
        }
    }
    public void BtnAds()
    {
        if (isClick)
        {
            isClick = false;

            StopAnim();
            multiplier = rotatePin.index;
            BtnHome(multiplier, amountBonus);
        }

    }
    public void StopAnim()
    {
        animDollSpin.speed = 0;
    }
}
