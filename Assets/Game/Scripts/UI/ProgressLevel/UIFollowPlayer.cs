using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class UIFollowPlayer : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Transform targetTransform;
    [SerializeField] private float speed;
    [SerializeField] private Vector2 offset;
    [SerializeField] private Canvas canvas;
    private Vector3 targetPosition;

    void LateUpdate()
    {
        if (GameManager.Ins.gameState == GameState.GamePlay && targetTransform != null)
        {
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(targetTransform.position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.GetComponent<RectTransform>(), screenPosition, canvas.worldCamera, out Vector2 localPos);
            rectTransform.localPosition = localPos + offset;

        }
    }

    public void SetTargetTransform(Transform targetTF)
    {
        targetTransform = targetTF;
    }


}
