using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIGoldReward : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tmpGold;

    public void SetData(int gold)
    {
        tmpGold.text = $"{gold}";
    }
}
