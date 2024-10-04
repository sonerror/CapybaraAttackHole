using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialManager : Singleton<MaterialManager>
{
    [SerializeField] private Color meshRenderer;
    public Color Setmat()
    {
        return meshRenderer;
    }
}
