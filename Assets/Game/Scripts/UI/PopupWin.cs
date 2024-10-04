using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupWin : UICanvas
{
    public override void Open()
    {
        UIManager.Ins.CloseAll();
        base.Open();
    }
    public void BtnHome()
    {
        LevelManager.Ins.ReloadScene();
    }
}
