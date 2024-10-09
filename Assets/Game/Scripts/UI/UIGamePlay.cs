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
    [SerializeField] private TextMeshProUGUI tmpPercentHpBoss;
    [SerializeField] private GameObject ObjCountTime;
    [SerializeField] private GameObject ObjHpBoss;
    [SerializeField] private Image imgProgress;
    [SerializeField] private Image imgProgressHpBoss;
    [SerializeField] private GameObject objJoystick;
    [SerializeField] private GameObject objFire;
    private float HPBoss;
    public ShootingController shootingController;
    public UIFollowPlayer uiFollow;
    public bool isCountdown;
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
        SetActiveJoystick(true);
        isCountdown = true;
        shootingController.imgTotal.SetActive(false);
    }
    public void Update()
    {
        if (isCountdown)
        {
            UpdateCountDownText();
        }
    }
    public void SetActiveJoystick(bool active)
    {
        objJoystick.SetActive(active);
        uiFollow.gameObject.SetActive(active);
        ObjCountTime.SetActive(active);
        objFire.SetActive(!active);
        shootingController.spriteDown.gameObject.SetActive(active);
        ObjHpBoss.SetActive(!active);
        SetProgressHp(1);
        isCountdown = active;
    }
    public void SetAtiveBtnShot()
    {
        objFire.SetActive(true);
        shootingController.spriteDown.gameObject.SetActive(true);
        shootingController.UpdateUI();
        shootingController.imgTotal.SetActive(true);
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
        if (currentTime <= 6)
        {
            countTime.color = Color.red;
        }
        else
        {
            countTime.color = Color.black;
        }
    }
    public void ShoterBoss()
    {
        Debug.Log(LevelManager.Ins.historyMagnetics.Count);
    }
    public void UIHPBoss()
    {
        SetProgressHp((float)LevelManager.Ins.bossTimeUp.point / HPBoss);
    }
    public void OninitHPBoss()
    {
        HPBoss = LevelManager.Ins.bossTimeUp.point;

    }
    public void SetProgressHp(float amount)
    {
        imgProgressHpBoss.fillAmount = amount;
        float percentHP = amount * 100f;
        if (percentHP <= 0)
        {
            percentHP = 0;
        }
        tmpPercentHpBoss.text = percentHP.ToString("F1") + "%";
    }
}
