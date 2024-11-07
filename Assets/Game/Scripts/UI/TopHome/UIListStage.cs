using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIListStage : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tmpStage;
    [SerializeField] private RectTransform rectTransform;
    public void ChangeColor(Color color)
    {
        tmpStage.color = color;
    }
    public void ChangeUIStage(int stage)
    {
        tmpStage.text = $"{stage}";
    }
    public void HideObj(bool hide)
    {
        this.gameObject.SetActive(hide);
    }
    public RectTransform SetReatTranform()
    {
        return this.rectTransform;
    }
    public void ChangeScale(float scale)
    {
        if (rectTransform != null)
        {
            rectTransform.localScale = Vector3.one * scale;
        }
    }
}
