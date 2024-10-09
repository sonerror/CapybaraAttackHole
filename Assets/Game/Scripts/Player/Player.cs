
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
    public bool move = true;
    public int lvTime;
    public int lvEx;
    public Transform mouth;
    private Vector3 lastInputDirection;
    private Quaternion targetRotation;

    public override void OnInit()
    {
        base.OnInit();
        move = true;
    }

    public void SetScale(int _lvScale)
    {
        var data = GetCheckPointData(_lvScale);
        this.transform.localScale = new Vector3(1, 1, 1) * data.scale;
        SetCamera(_lvScale);
        ChangeSpeed(data.speedMove);
    }

    public void SetScaleUpLevel(int _lvScale)
    {
        float targetScale = GetCheckPointData(_lvScale).scale;
        ChangeScale(targetScale);
        SetCamera(_lvScale);
    }

    public void GetDataLevel(List<CheckPoint> _checkPoint)
    {
        this.checkPoints = new List<CheckPoint>(_checkPoint);
    }

    private CheckPoint GetCheckPointData(int id)
    {
        return checkPoints.Find(x => x.id == id);
    }

    public void SetCamera(int _levelCurrent)
    {
        var targetPoint = GetCheckPointData(_levelCurrent);
        CameraManager.Ins.SetTfCamera(targetPoint.offSet, targetPoint.eulerAngles);
    }

    public void SetData(int _Lv, int _lvTime, int _lvEx)
    {
        this.lvCurrent = _Lv;
        this.lvTime = _lvTime;
        this.lvEx = _lvEx;
        SetScale(lvCurrent);
        OnInit();
    }

    private void Update()
    {
        if (GameManager.Ins.gameState != GameState.GamePlay || !move) return;

        Vector3 inputDirection = GetInputDirection();
        lastInputDirection = inputDirection.sqrMagnitude > 0.001f ? inputDirection : Vector3.zero;
    }

    private void FixedUpdate()
    {
        if (GameManager.Ins.gameState == GameState.GamePlay && move && lastInputDirection.sqrMagnitude > 0.001f)
        {
            Move(lastInputDirection);
        }
    }

    private void Move(Vector3 moveDirection)
    {
        rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);

        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * Time.fixedDeltaTime * 100f);
        }
    }

    private Vector3 GetInputDirection()
    {
        Vector3 moveDirection = Vector3.zero;

#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButton(0))
        {
            moveDirection = new Vector3(JoystickControl.direct.x, 0, JoystickControl.direct.z);
        }
#elif UNITY_IOS || UNITY_ANDROID
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
            {
                moveDirection = new Vector3(JoystickControl.direct.x, 0, JoystickControl.direct.z);
            }
        }
#endif
        return moveDirection.normalized;
    }

    public void ChangeSpeed(float _speed)
    {
        moveSpeed = _speed;
    }

    public void CheckPointUpLevel()
    {
        var targetPoint = GetCheckPointData(lvCurrent);
        if (lvCurrent < checkPoints.Count && point >= targetPoint.checkPoint)
        {
            Debug.LogError("isCheckPoint");
            ChangeSpeed(targetPoint.speedMove);
            CameraManager.Ins.SetTfCamera(targetPoint.offSet, targetPoint.eulerAngles);
            lvCurrent += 1;
            SetScaleUpLevel(lvCurrent);
            UIManager.Ins.GetUI<UIGamePlay>().ReLoadUI();
        }
    }

    public float GetCheckPoint(int lv)
    {
        return GetCheckPointData(lv).checkPoint;
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
