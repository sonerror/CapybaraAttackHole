using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour
{
    public float initialCountDownTime = 100f;
    public float countDownTime = 100f;
    public bool isCountDown;
    public void Oninit()
    {
        isCountDown = false;

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
        if (countDownTime < 0)
        {
            LevelManager.Ins.OnTimeUP();
            isCountDown = false;
        }
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
