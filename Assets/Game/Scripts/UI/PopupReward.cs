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
    [SerializeField] private PinResult pinResult;
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
    }
    public void BtnHome(float multiplier, int amount)
    {
        if (isClick)
        {
            isClick = false;
            CollectGold(multiplier, amount, () =>
            {
                LevelManager.Ins.ReloadScene();
            });
        }

    }
    private void CollectGold(float multiplier, int amount, UnityAction OnComplete = null)
    {
        confesti.gameObject.SetActive(true);
        confesti.rateOverTime = 150;
        confesti.Play();
        DOVirtual.DelayedCall(1.3f, () =>
        {
            DataManager.Ins.ChangeGold((int)(amount * multiplier));
            DOVirtual.DelayedCall(1.3f, () =>
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

    }
    public void PlayAnim()
    {
        animDollSpin.enabled = true;
        animDollSpin.Play("Doll_Spin");
    }
    public void BtnNoThanks()
    {
        StopAnim();
        BtnHome(1, amountBonus);
    }
    public void BtnAds()
    {
        StopAnim();
        RectTransform rectTfPin = pinResult.GetRectTF();
        multiplier = SetMultiplier(rectTfPin.rotation.z);
        Debug.LogError(SetMultiplier(multiplier));
        Debug.LogError(rectTfPin.rotation.z);
        BtnHome(multiplier, amountBonus);
    }
    public void StopAnim()
    {
        animDollSpin.enabled = false;
    }
    private float SetMultiplier(float _multiplier)
    {
        float index = 1.5f;
        switch (_multiplier)
        {
            case >= -95f and < -70f:
                index = 3f;
                break;
            case >= -70f and < -35f:
                index = 2.5f;
                break;
            case >= -35f and <= 38.5f:
                index = 2f;
                break;
            case > 38.5f and <= 95f:
                index = 1.5f;
                break;
            default:
                index = 1.5f;
                break;
        }
        return index;
    }
}
