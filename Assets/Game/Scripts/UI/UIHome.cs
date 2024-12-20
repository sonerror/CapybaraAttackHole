using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIHome : UICanvas
{
    [SerializeField] private TextMeshProUGUI tmpLevelSize;
    [SerializeField] private TextMeshProUGUI tmpPriceLevelSize;
    [SerializeField] private TextMeshProUGUI tmpLevelExp;
    [SerializeField] private TextMeshProUGUI tmpPriceLevelExp;
    [SerializeField] private TextMeshProUGUI tmpLevelTime;
    [SerializeField] private TextMeshProUGUI tmpPriceLevelTime;
    [SerializeField] private TextMeshProUGUI countTime;
    [SerializeField] private TextMeshProUGUI tmpStage;
    [SerializeField] private UIListIconStage uIListIconStage;
    private int maxlvUp = 3;
    private PlayerData data;
    public override void Setup()
    {
        base.Setup();
        data = DataManager.Ins.playerData;
    }
    public override void Open()
    {
        UIManager.Ins.CloseAll();
        base.Open();
        uIListIconStage.OnInit();
        UpdateUI();
    }
    public void BtnPlay()
    {
        UIManager.Ins.CloseUI<UIHome>();
        UIManager.Ins.OpenUI<UIGamePlay>();
        GameManager.Ins.ChangeState(GameState.GamePlay);
        UIManager.Ins.GetUI<UIGamePlay>().ReLoadUIFollow();
        EnemyManager.Ins.PlayPantrol();
    }
    public void BtnUpScale()
    {
        if (data.lvScale < maxlvUp)
        {
            int totalLevels = LevelManager.Ins._levelData.levels.Count;
            int validLevelID = data.levelCurrent % totalLevels;
            if (data.lvScale < LevelManager.Ins._levelData.GetDataWithID(validLevelID).checkPoints.Count - 1)
            {
                int price = LevelManager.Ins._levelData.GetDataWithID(validLevelID).checkPoints[data.lvScale].price;
                if (data.gold >= price)
                {
                    LevelManager.Ins.UpDataScale();
                    UpdateUIBtnScale();
                    DataManager.Ins.ChangeGold(-price);
                }
            }
        }
    }
    public void BtnUpLvEXP()
    {
        int price = LevelManager.Ins.levelEx.GetDataWithID(data.lvEx).price;
        if (data.gold >= price && data.lvEx < LevelManager.Ins.levelEx.levelBonusDataModels.Count - 1)
        {
            LevelManager.Ins.UpLVBonusExp();
            UpdateUIBtnExp();
            DataManager.Ins.ChangeGold(-price);
        }
    }
    public void BtnUpLvTime()
    {
        int price = LevelManager.Ins.levelTime.GetDataWithID(data.lvTime).price;
        if (data.gold >= price && data.lvTime < LevelManager.Ins.levelTime.levelBonusDataModels.Count - 1)
        {
            LevelManager.Ins.UpLVBonusTime();
            UpdateUIBtnTime();
            DataManager.Ins.ChangeGold(-price);
        }
    }
    public void UpdateUI()
    {
        UpdateUIBtnScale();
        UpdateUIBtnExp();
        UpdateUIBtnTime();
        SetUITmpStage();
    }
    public void SetUITmpStage()
    {
        tmpStage.text = $"STAGE {DataManager.Ins.playerData.levelCurrent + 1}";
    }
    public void UpdateUIBtnScale()
    {
        int totalLevels = LevelManager.Ins._levelData.levels.Count;
        int validLevelID = data.levelCurrent % totalLevels;
        tmpLevelSize.text = $"Size LV: {data.lvScale + 1}";
        Debug.LogError(data.lvScale + " " + (LevelManager.Ins._levelData.GetDataWithID(validLevelID).checkPoints.Count - 1));
        if (data.lvScale >= LevelManager.Ins._levelData.GetDataWithID(validLevelID).checkPoints.Count - 1 || data.lvScale >= maxlvUp)
        {
            tmpPriceLevelSize.text = $"Max";
        }
        else
        {
            tmpPriceLevelSize.text = $"Price: {LevelManager.Ins._levelData.GetDataWithID(validLevelID).checkPoints[data.lvScale].price}";
        }
    }
    public void UpdateUIBtnExp()
    {
        tmpLevelExp.text = $"EXP LV: {data.lvEx + 1}";
        if (data.lvEx >= LevelManager.Ins.levelEx.levelBonusDataModels.Count - 1)
        {
            tmpPriceLevelExp.text = $"Max";
        }
        else
        {
            tmpPriceLevelExp.text = $"Price: {LevelManager.Ins.levelEx.GetDataWithID(data.lvEx).price}";
        }
    }
    public void UpdateUIBtnTime()
    {
        tmpLevelTime.text = $"Time LV: {data.lvTime + 1}";
        if (data.lvTime >= LevelManager.Ins.levelTime.levelBonusDataModels.Count - 1)
        {
            tmpPriceLevelTime.text = $"Max";
        }
        else
        {
            tmpPriceLevelTime.text = $"Price: {LevelManager.Ins.levelTime.GetDataWithID(data.lvTime).price}";
        }
        UpdateCountDownText();
    }
    private void UpdateCountDownText()
    {
        float currentTime = LevelManager.Ins.SetTime();
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
}