
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] public float moveSpeed = 5f;
    [SerializeField] private float rotateSpeed = 5f;
    public static Player Instance { get; private set; }
    public List<CheckPoint> checkPoints = new List<CheckPoint>();
    public bool isMove;
    public bool move = true;
    public int lvTime;
    public int lvEx;

    public override void OnInit()
    {
        base.OnInit();
        move = true;
    }
    public void SetScale(int _lvScale)
    {
        var data = checkPoints.Find(x => x.id == _lvScale);
        this.transform.localScale = new Vector3(1, 1, 1) * data.scale;
        SetCamera(_lvScale);
        ChangeSpeed(data.speedMove);
    }
    public void SetScaleUpLevel(int _lvScale)
    {
        float targetScale = checkPoints.Find(x => x.id == _lvScale).scale;
        ChangeScale(targetScale);
        SetCamera(_lvScale);
    }
    public void SetCamera(int _levelCurrent)
    {
        var targetPont = checkPoints.Find(x => x.id == _levelCurrent);
        CameraManager.Ins.SetTfCamera(targetPont.offSet, targetPont.eulerAngles);
    }
    public void SetData(int _Lv, int _lvTime, int _lvEx)
    {
        this.lvCurrent = _Lv;
        this.lvTime = _lvTime;
        this.lvEx = _lvEx;
        SetScale(lvCurrent);
        OnInit();
    }
    public void GetDataLevel(List<CheckPoint> _checkPoint)
    {
        this.checkPoints = new List<CheckPoint>(_checkPoint);
    }
    void FixedUpdate()
    {
        Move();
    }
    private void Move()
    {
        if (GameManager.Ins.gameState != GameState.GamePlay) return;
        if (move)
        {
            if (Input.GetMouseButton(0) && JoystickControl.direct.sqrMagnitude > 0.001f)
            {
                isMove = true;
                Vector3 moveDirection = new Vector3(JoystickControl.direct.x, 0, JoystickControl.direct.z);
                rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);
                Vector3 direction = Vector3.RotateTowards(transform.forward, JoystickControl.direct, rotateSpeed * Time.deltaTime, 0.0f);
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }
    }
    private void StopMove()
    {
        move = false;
    }
    public void ChangeSpeed(float _speed)
    {
        moveSpeed = _speed;
    }
    public void CheckPointUpLevel()
    {
        var targetPont = checkPoints.Find(x => x.id == (lvCurrent));
        if (lvCurrent <= checkPoints.Count && point >= targetPont.checkPoint)
        {
            Debug.LogError("isCheckPoint");
            ChangeSpeed(targetPont.speedMove);
            CameraManager.Ins.SetTfCamera(targetPont.offSet, targetPont.eulerAngles);
            lvCurrent += 1;
            SetScaleUpLevel(lvCurrent);
            UIManager.Ins.GetUI<UIGamePlay>().ReLoadUI();
        }
    }
    public float GetCheckPoint(int lv)
    {
        return checkPoints.Find(x => x.id == lv).checkPoint;
    }
    public float GetBonusEXP()
    {
        return LevelManager.Ins.levelEx.GetDataWithID(lvEx).bonus;
    }
    public void OnLose()
    {

    }
    public void MoveToTfTarget(Transform tfTarget)
    {
        this.transform.DOMove(tfTarget.position, 1f).SetEase(Ease.InOutQuad);
    }
}
