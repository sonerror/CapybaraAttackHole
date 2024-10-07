using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GoldUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tmpGold;
    private int currentGold;
    private void Start()
    {
        currentGold = DataManager.Ins.playerData.gold;
        EventManager.Subscribe(EventType.Gold, UpdateGoldText);
        tmpGold.text = currentGold.ToString();
    }
    public void UpdateGoldText(int targetGold)
    {
        DOTween.To(() => currentGold, x => currentGold = x, targetGold, 1f)
            .OnUpdate(() =>
            {
                tmpGold.text = currentGold.ToString();
                currentGold = DataManager.Ins.playerData.gold;
            });
    }
}
