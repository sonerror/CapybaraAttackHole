using AssetKits.ParticleImage;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PopupWin : UICanvas
{
    [SerializeField] private ParticleImage confesti;
    [SerializeField] private Transform tfStart;
    private bool isClick;
    public override void Open()
    {
        UIManager.Ins.CloseAll();
        base.Open();
        confesti.gameObject.transform.position = tfStart.position;
        confesti.gameObject.SetActive(false);
        isClick = true;
    }
    private void LoadUI()
    {

    }
    public void BtnHome()
    {
        if(isClick)
        {
            isClick = false;
            CollectGold(1, 20, () =>
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
            DataManager.Ins.ChangeGold(500);
            DOVirtual.DelayedCall(1.3f, () =>
            {
                confesti.gameObject.SetActive(false);
                OnComplete?.Invoke();
            });
        });
    }
}
