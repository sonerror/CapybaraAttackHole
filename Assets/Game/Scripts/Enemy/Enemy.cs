using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    [SerializeField] private BoxCollider boxCollider;
    [SerializeField] private Renderer meshRenderer;
    public Transform tfTarget;
    public void Start()
    {
        OnInit();
    }
    public override void OnInit()
    {
        base.OnInit();
        SetPoint(lvCurrent);
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
        Destroy(this);
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
        return points.Find(x=>x.id == id);
    }
}
[System.Serializable]
public class PointDataModel
{
    public int id;
    public float point;
}