using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinResult : MonoBehaviour
{
    [SerializeField] private RectTransform recTfPin;

    public RectTransform GetRectTF()
    {
        return recTfPin;
    }

}
