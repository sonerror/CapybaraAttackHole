using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShootingController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Image spriteDown;
    [SerializeField] private Sprite spriteUp;
    private Sprite spriteCurrent;
    public bool isShooting;

    public void Start()
    {
        spriteCurrent = spriteDown.sprite;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        isShooting = true;
        spriteDown.sprite = spriteUp;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        isShooting = false;
        spriteDown.sprite = spriteCurrent;
    }
    void Update()
    {
        if (isShooting)
        {
            OnShoot();
        }
    }
    private void OnShoot()
    {
        var data = LevelManager.Ins;
        Debug.LogError("Fire");
        if (data.historyMagnetics.Count > 0)
        {
            int id = data.historyMagnetics[data.historyMagnetics.Count - 1];
            Enemy enemyData = data.enemyDatas.GetDataWithID(id).enemy;
            Enemy enemyShot = Instantiate(enemyData);
            enemyShot.transform.position = data.player.transform.position;
            enemyShot.transform.localScale = new Vector3(2, 2, 2);
            enemyShot.transform.DOMove(data.bossTimeUp.tfTarget.position, 2f)
                   .OnComplete(() =>
                   {
                       enemyShot.DesSpawm();
                       Debug.LogError("Hit");
                   });
            data.historyMagnetics.RemoveAt(data.historyMagnetics.Count - 1);
        }
    }
}
