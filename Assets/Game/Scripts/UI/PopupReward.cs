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
    private bool isClick;
    public bool isWin;
    
    public override void Open()
    {
        base.Open();
        PlayAnim();
        confesti.gameObject.transform.position = tfStart.position;
        confesti.gameObject.SetActive(false);
        isClick = true;
    }
    public void BtnHome(int multiplier, int amount)
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
    private void CollectGold(int multiplier, int amount, UnityAction OnComplete = null)
    {
        confesti.gameObject.SetActive(true);
        confesti.rateOverTime = 150;
        confesti.Play();
        DOVirtual.DelayedCall(1.3f, () =>
        {
            DataManager.Ins.ChangeGold(amount * multiplier);
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
        if(isWin)
        {
            uIGold.SetData(500);
        }
        else
        {
            uIGold.SetData(200);
        }
    }
    public void PlayAnim()
    {
        animDollSpin.Play("Doll_Spin");
    }
    public void BtnNoThanks()
    {
        StopAnim();
        if (isWin)
        {
            BtnHome(1, 500);
        }
        else
        {
            BtnHome(1, 200);
        }
        
    }
    public void BtnAds()
    {
        StopAnim();
        BtnHome(1, 500);
    }
    public void StopAnim()
    {
        animDollSpin.enabled = false;
    }
}
