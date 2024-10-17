using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RotatePin : MonoBehaviour
{
    public float index;
    [SerializeField] private TextMeshProUGUI tmpBonusGold;
    private int bonusAds;
    public void Oninit(int _bonusAds)
    {
        bonusAds = _bonusAds;
    }
    public void X2()
    {
        index = 1.5f;
        tmpBonusGold.text = $"+{(int)(bonusAds * index)}";
    }
    public void X3()
    {
        index = 2;
        tmpBonusGold.text = $"+{(int)(bonusAds * index)}";
    }
    public void X4()
    {
        index = 2.5f;
        tmpBonusGold.text = $"+{(int)(bonusAds * index)}";
    }
    public void X5()
    {
        index = 3f;
        tmpBonusGold.text = $"+{(int)(bonusAds * index)}";
    }
}
