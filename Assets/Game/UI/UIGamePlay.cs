using CW.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class UIGamePlay : UICanvas
{
    public UIFollowPlayer uiFollow;
    public override void Open()
    {
        base.Open();
    }
    public void ReLoadUIFollow()
    {
        uiFollow.gameObject.SetActive(true);
        uiFollow.Oninit();
    }
}
