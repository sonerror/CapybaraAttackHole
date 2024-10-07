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

    public override void Open()
    {

        UIManager.Ins.CloseAll();
        base.Open();
        UpdateUI();
    }
    public void BtnPlay()
    {
        UIManager.Ins.CloseUI<UIHome>();
        UIManager.Ins.OpenUI<UIGamePlay>();
        GameManager.Ins.ChangeState(GameState.GamePlay);
        UIManager.Ins.GetUI<UIGamePlay>().ReLoadUIFollow();

    }
    public void BtnUpScale()
    {
        var data = DataManager.Ins.playerData;
        int price = LevelManager.Ins._levelData.GetDataWithID(data.levelCurrent).checkPoints[data.lvScale].price;
        if (data.gold >= price)
        {
            LevelManager.Ins.UpDataScale();
            UpdateUIBtnScale();
            DataManager.Ins.ChangeGold(-price);
        }
    }
    public void BtnUpLvEXP()
    {
        var data = DataManager.Ins.playerData;
        int price = LevelManager.Ins.levelEx.GetDataWithID(data.lvEx).price;
        if (data.gold >= price)
        {
            LevelManager.Ins.UpLVBonusExp();
            UpdateUIBtnExp();
            DataManager.Ins.ChangeGold(-price);
        }
    }
    public void BtnUpLvTime()
    {
        var data = DataManager.Ins.playerData;
        int price = LevelManager.Ins.levelTime.GetDataWithID(data.lvTime).price;
        if (data.gold >= price)
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
    }
    public void UpdateUIBtnScale()
    {
        var data = DataManager.Ins.playerData;
        tmpLevelSize.text = $"Size LV: {data.lvScale + 1}";
        tmpPriceLevelSize.text = $"Price: {LevelManager.Ins._levelData.GetDataWithID(data.levelCurrent).checkPoints[data.lvScale].price}";
    }
    public void UpdateUIBtnExp()
    {
        var data = DataManager.Ins.playerData;
        tmpLevelExp.text = $"EXP LV: {DataManager.Ins.playerData.lvEx + 1}";
        tmpPriceLevelExp.text = $"Price: {LevelManager.Ins.levelEx.GetDataWithID(data.lvEx).price}";
    }
    public void UpdateUIBtnTime()
    {
        var data = DataManager.Ins.playerData;
        tmpLevelTime.text = $"Time LV: {DataManager.Ins.playerData.lvTime + 1}";
        tmpPriceLevelTime.text = $"Price: {LevelManager.Ins.levelTime.GetDataWithID(data.lvTime).price}";
    }
}
