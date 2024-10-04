using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    [SerializeField] private BoxCollider boxCollider;
    [SerializeField] private Renderer meshRenderer;
    private Color orColor;
    public Transform tfTarget;
    private bool isCheckWinLose = false;
    public void Start()
    {
        OnInit();
        orColor = GetComponentInChildren<MeshRenderer>().material.color;
    }
    public override void OnInit()
    {
        base.OnInit();
        SetPoint(lvCurrent);
    }

    public void CheckWinLose(bool isCheck)
    {
        isCheckWinLose = isCheck;
    }
    public void CheckWinLose()
    {
        if (!isDead)
        {
            if (LevelManager.Ins.historyMagnetics.Count <= 0)
            {
                isDead = true;
                if (point <= 0)
                {
                    DataManager.Ins.playerData.levelCurrent += 1;
                    DataManager.Ins.ResetData();
                    StartCoroutine(IE_ShowUIWin());
                }
                else
                {
                    StartCoroutine(IE_ShowUILose());
                }
              
            }
        }
    }
    public void ChangeColorTriggerEn()
    {
        meshRenderer.material.color = MaterialManager.Ins.Setmat();
    }
    public void ChangeColorTriggerEX()
    {
        meshRenderer.material.color = orColor;
    }
    IEnumerator IE_ShowUIWin()
    {
        yield return new WaitForSeconds(2f);
        UIManager.Ins.OpenUI<PopupWin>();
    }
    IEnumerator IE_ShowUILose()
    {
        yield return new WaitForSeconds(2f);
        UIManager.Ins.OpenUI<PopupLose>();
    }
  
    public void SetPoint(int lv)
    {
        point = LevelManager.Ins.pointData.GetDataWithID(lv).point;
    }
    public void HideCollider(bool isActive)
    {
        boxCollider.enabled = isActive;
    }
    public void AddMat(Material mat)
    {
        if (meshRenderer != null && mat != null)
        {
            List<Material> materials = new List<Material>(meshRenderer.materials);
            materials.Add(mat);
            meshRenderer.materials = materials.ToArray();
        }
    }
    public void DesSpawm()
    {
        Destroy(gameObject);
    }
    public void RemoveLastMat()
    {
        if (meshRenderer != null)
        {
            List<Material> materials = new List<Material>(meshRenderer.materials);
            if (materials.Count > 0)
            {
                materials.RemoveAt(materials.Count - 1);
                meshRenderer.materials = materials.ToArray();
            }
        }
    }
}
[System.Serializable]
public class PointData
{
    public List<PointDataModel> points;
    public PointDataModel GetDataWithID(int id)
    {
        return points.Find(x => x.id == id);
    }
}
[System.Serializable]
public class PointDataModel
{
    public int id;
    public float point;
}