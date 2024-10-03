using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIHome : UICanvas
{
    [SerializeField] private TextMeshProUGUI tmpLevelSize;
    [SerializeField] private TextMeshProUGUI tmpLevelExp;
    public override void Open()
    {
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
        LevelManager.Ins.UpDataScale();
        UpdateUIBtnScale();
    }
    public void BtnUpLvEXP()
    {
        LevelManager.Ins.UpLVBonusExp();
        UpdateUIBtnExp();
    }
    public void UpdateUI()
    {
        UpdateUIBtnScale();
        UpdateUIBtnExp();
    }
    public void UpdateUIBtnScale()
    {
        tmpLevelSize.text = $"Size LV: {DataManager.Ins.playerData.lvScale + 1}";
    }
    public void UpdateUIBtnExp()
    {
        tmpLevelExp.text = $"EXP LV: {DataManager.Ins.playerData.lvEx + 1}";
    }
}
