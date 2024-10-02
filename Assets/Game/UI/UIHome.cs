using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHome : UICanvas
{
    public override void Open()
    {
        base.Open();
    }
    public void BtnPlay()
    {
        UIManager.Ins.CloseUI<UIHome>();
        UIManager.Ins.OpenUI<UIGamePlay>();
        UIManager.Ins.GetUI<UIGamePlay>().ReLoadUIFollow();
        GameManager.Ins.ChangeState(GameState.GamePlay);
        CameraManager.Ins.ChangeStateCamera(GameState.GamePlay);

    }
}
