using CW.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UIGamePlay : UICanvas
{
    [SerializeField] private TextMeshProUGUI tmpLevel;
    public UIFollowPlayer uiFollow;
    [SerializeField] private Image imgProgress;
   public override void Open()
    {
        base.Open();
        SetProgressSpin(0);
    }
    public void ReLoadUIFollow()
    {
        uiFollow.gameObject.SetActive(true);
        uiFollow.Oninit();
        ReLoadUI();
    }
    public void ReLoadUI()
    {
        tmpLevel.text = $"{LevelManager.Ins.player.lvCurrent + 1}";
    }
    public void SetProgressSpin(float amount)
    {
        imgProgress.fillAmount = amount;
    }
}
