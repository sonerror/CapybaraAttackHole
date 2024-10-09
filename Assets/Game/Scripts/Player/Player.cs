using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] public float moveSpeed = 5f;
    [SerializeField] private float rotateSpeed = 5f;
    public static Player Instance { get; private set; }

    private Dictionary<int, CheckPoint> checkPointDict = new Dictionary<int, CheckPoint>();
    public List<CheckPoint> checkPoints = new List<CheckPoint>();
    public bool move = true;
    public int lvTime;
    public int lvEx;
    public Transform mouth;
    private Vector3 lastInputDirection;

    public override void OnInit()
    {
        base.OnInit();
        move = true;
    }

    public void SetScale(int _lvScale)
    {
        if (checkPointDict.TryGetValue(_lvScale, out var data))
        {
            transform.localScale = Vector3.one * data.scale;
            ChangeSpeed(data.speedMove);
            SetCamera(_lvScale);
        }
    }

    public void SetScaleUpLevel(int _lvScale)
    {
        SetScale(_lvScale);
    }

    public void GetDataLevel(List<CheckPoint> _checkPoint)
    {
        checkPoints = _checkPoint;
        checkPointDict.Clear();
        foreach (var cp in checkPoints)
        {
            checkPointDict[cp.id] = cp;
        }
    }

    public void SetCamera(int _levelCurrent)
    {
        if (checkPointDict.TryGetValue(_levelCurrent, out var targetPoint))
        {
            CameraManager.Ins.SetTfCamera(targetPoint.offSet, targetPoint.eulerAngles);
        }
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

        // Chỉ cần hướng tới moveDirection nếu nó không phải là zero
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
            SetCamera(lvCurrent);
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
        // Handle losing state if needed
    }

    public void MoveToTfTarget(Transform tfTarget)
    {
        this.transform.DOMove(tfTarget.position, 1f).SetEase(Ease.InOutQuad);
    }

    private CheckPoint GetCheckPointData(int id)
    {
        checkPointDict.TryGetValue(id, out var checkpoint);
        return checkpoint;
    }
}

