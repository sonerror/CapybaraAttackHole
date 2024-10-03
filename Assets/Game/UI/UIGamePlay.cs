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
    [SerializeField] private TextMeshProUGUI countTime;
    [SerializeField] private GameObject ObjCountTime;
    [SerializeField] private Image imgProgress;
    [SerializeField] private GameObject objJoystick;
    [SerializeField] private GameObject objFire;

    public UIFollowPlayer uiFollow;
    public override void Setup()
    {
        base.Setup();
    }
    public override void Open()
    {
        base.Open();
        SetProgressSpin(0);
        UpdateCountDownText();
        LevelManager.Ins.stage.IsCountDown(true);
    }
    public void Update()
    {
        UpdateCountDownText();
    }
    public void SetActiveJoystick(bool active)
    {
        objJoystick.SetActive(active);
        uiFollow.gameObject.SetActive(active);
        ObjCountTime.SetActive(active);
    }
    public void SetAtiveBtnShot()
    {
        objFire.SetActive(true);
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
    private void UpdateCountDownText()
    {
        float currentTime = LevelManager.Ins.stage.countDownTime;
        int minute = (int)currentTime / 60;
        int second = (int)currentTime % 60;
        string minuteString = minute.ToString();
        string secondString = second.ToString();
        if (minuteString.Length < 2)
        {
            minuteString = "0" + minuteString;
        }
        if (secondString.Length < 2)
        {
            secondString = "0" + secondString;
        }
        countTime.text = minuteString + ":" + secondString;
    }
    public void ShoterBoss()
    {
       Debug.Log(LevelManager.Ins.historyMagnetics.Count);
    }
}
