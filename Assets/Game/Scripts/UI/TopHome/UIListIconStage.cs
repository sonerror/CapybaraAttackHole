using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIListIconStage : MonoBehaviour
{
    [SerializeField] private List<UIListStage> uIListStage = new List<UIListStage>();
    [SerializeField] private List<RectTransform> listReatTranforms = new List<RectTransform>();
    [SerializeField] private RectTransform rectTransform;

    public void OnInit()
    {
        LoadUIListStage();
    }

    private void LoadUIListStage()
    {
        int levelCurrent = DataManager.Ins.playerData.levelCurrent;
        int startLevel = levelCurrent - 2;
        listReatTranforms.Clear();
        for (int i = 0; i < uIListStage.Count; i++)
        {
            int stageReal = startLevel + i + 1;
            bool isHighlighted = stageReal == levelCurrent + 1;
            uIListStage[i].ChangeUIStage(stageReal);
            bool shouldHide = stageReal <= 0;
            uIListStage[i].HideObj(!shouldHide);
            if (!shouldHide)
            {
                listReatTranforms.Add(uIListStage[i].SetReatTranform());
            }
            uIListStage[i].ChangeScale(isHighlighted ? 1.25f : 1f);
            uIListStage[i].ChangeColor(isHighlighted ? Color.white : new Color32(4, 68, 104, 255));
        }
        StartCoroutine(IE_SetUILine());
    }

    IEnumerator IE_SetUILine()
    {
        yield return new WaitForSeconds(0.5f);
        SetUILine();
    }

    private void SetUILine()
    {
        int countIconSatge = listReatTranforms.Count;

        if (countIconSatge > 0)
        {
            rectTransform.sizeDelta = new Vector2(100 * (countIconSatge - 1), rectTransform.sizeDelta.y);
            if (countIconSatge > 1)
            {
                float firstPosX = listReatTranforms[0].anchoredPosition.x;
                float lastPosX = listReatTranforms[countIconSatge - 1].anchoredPosition.x;
                rectTransform.anchoredPosition = new Vector2((firstPosX + lastPosX) / 2, rectTransform.anchoredPosition.y);
            }
        }
    }
}
