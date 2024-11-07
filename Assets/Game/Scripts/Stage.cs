using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class Stage : MonoBehaviour
{
    [SerializeField] private NavMeshSurface navMeshSurface;
    public float initialCountDownTime = 100f;
    public float countDownTime = 100f;
    public bool isCountDown;
    public List<PositonStartEnemy> listPosStartEnemy = new List<PositonStartEnemy>();
    public void Oninit()
    {
        isCountDown = false;
    }
    public void OnEnableNavMesh(bool isActive)
    {
        navMeshSurface.enabled = isActive;
    }
    public void SetTimeData(int time)
    {
        countDownTime = time;
    }
    private void Update()
    {
        if (!isCountDown || countDownTime < 0) return;
        if (GameManager.Ins.gameState == GameState.GamePlay)
        {
            countDownTime -= Time.deltaTime;
        }
        if (countDownTime <= 0)
        {
            LevelManager.Ins.player.move = false;
            isCountDown = false;
            EnemyManager.Ins.PlayIdle();
            StartCoroutine(IE_OnTimeUp());
        }
    }
    IEnumerator IE_OnTimeUp()
    {
        yield return new WaitForSeconds(2f);
        LevelManager.Ins.OnTimeUP();
    }
    public void IsCountDown(bool isCount)
    {
        isCountDown = isCount;
    }
    public void OnContinue(float plusTime)
    {
        countDownTime += plusTime;
        isCountDown = true;
    }
}