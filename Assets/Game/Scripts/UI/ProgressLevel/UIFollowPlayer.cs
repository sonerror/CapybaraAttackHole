using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIFollowPlayer : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private TextMeshProUGUI tmp;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float speed;
    private Vector3 targetPosition;
    private Transform targetTransform;
    public void Oninit()
    {
       // targetTransform = LevelManager.Ins.player.transform;
    }
    void LateUpdate()
    {
        if (GameManager.Ins.gameState == GameState.GamePlay)
        {
            rectTransform.position = Vector3.Lerp(rectTransform.position, targetPosition, speed * Time.deltaTime);
        }
    }
}
