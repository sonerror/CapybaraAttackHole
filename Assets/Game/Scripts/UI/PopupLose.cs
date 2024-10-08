using AssetKits.ParticleImage;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PopupLose : UICanvas
{
    [SerializeField] private ParticleImage confesti;
    [SerializeField] private Transform tfStart;
    public override void Open()
    {
        UIManager.Ins.CloseAll();
        base.Open();
        confesti.gameObject.transform.position = tfStart.position;
        confesti.gameObject.SetActive(false);
    }
    public void BtnHome()
    {
        CollectGold(1, 20, () =>
        {
            confesti.gameObject.SetActive(false);
            LevelManager.Ins.ReloadScene();
        });
    }
  
    private void CollectGold(int multiplier, int amount, UnityAction OnComplete = null)
    {
        confesti.gameObject.SetActive(true);
        confesti.rateOverTime = 150;
        confesti.Play();
        DOVirtual.DelayedCall(1.3f, () =>
        {
            DataManager.Ins.ChangeGold(200);
            DOVirtual.DelayedCall(1.3f, () =>
            {
                OnComplete?.Invoke();
            });
        });
    }
}
