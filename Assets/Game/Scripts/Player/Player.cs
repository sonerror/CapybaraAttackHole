using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Player : Character
{
    [SerializeField] public float moveSpeed = 5f;
    [SerializeField] private float rotateSpeed = 5f;
    private Quaternion targetRotation;
    private Vector3 inputDirection = Vector3.zero;
    private bool isCreateEnemy;
    private bool isMagnetic;

    public List<CheckPoint> checkPoints = new List<CheckPoint>();
    public bool move = true;
    public int lvTime;
    public int lvEx;
    public Transform mouth;

    public float targetCheckPoint;
    public float durProgress;
    public Transform playerSkill;
    public Transform blackHoleCenter;

    public override void OnInit()
    {
        base.OnInit();
        move = true;
        isCreateEnemy = true;
        isMagnetic = true;
    }

    public void SetScale(int _lvScale)
    {
        var data = GetCheckPointData(_lvScale);
        this.transform.localScale = new Vector3(1, 1, 1) * data.scale;
        SetCamera(_lvScale);
        ChangeSpeed(data.speedMove);
        SetTargetCheckPoint();
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
        CameraManager.Ins.AdjustCameraDistance((_levelCurrent + 1) * Const.CAMERADISTANCE);
    }

    public void SetData(int _Lv, int _lvTime, int _lvEx)
    {
        this.lvCurrent = _Lv;
        this.lvTime = _lvTime;
        this.lvEx = _lvEx;
        SetScale(lvCurrent);
        OnInit();
    }

    private void SetTargetCheckPoint()
    {
        targetCheckPoint = GetCheckPointData(lvCurrent).checkPoint;
        if (lvCurrent > 0)
        {
            durProgress = GetCheckPointData(lvCurrent - 1).checkPoint;
        }
        Debug.LogError("targetCheckPoint: " + targetCheckPoint);
    }
    private void Update()
    {
        if (GameManager.Ins.gameState != GameState.GamePlay || !move) return;

        Vector3 currentInputDirection = GetInputDirection();
        if (currentInputDirection.sqrMagnitude > 0.001f)
        {
            inputDirection = currentInputDirection;
            Move(inputDirection);
        }
        else
        {
            StopMovement();
        }
    }

    private void Move(Vector3 dir)
    {
        transform.Translate(dir * moveSpeed * Time.deltaTime, Space.World);
        Rotate(dir);
    }

    private void StopMovement()
    {
        inputDirection = Vector3.zero;
    }
    private void Rotate(Vector3 rotationDir)
    {
        if (rotationDir != Vector3.zero)
        {
            targetRotation = Quaternion.LookRotation(rotationDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
        }
    }

    private Vector3 GetInputDirection()
    {
        Vector3 moveDirection = Vector3.zero;
        if (Input.GetMouseButton(0))
        {
            if (JoystickControl.direct.sqrMagnitude > 0.001f)
            {
                moveDirection = new Vector3(JoystickControl.direct.x, 0, JoystickControl.direct.z);
            }
        }
        return moveDirection.normalized;
    }

    public void ChangeSpeed(float _speed)
    {
        moveSpeed = _speed;
    }

    public void CheckPointUpLevel()
    {
        CheckTurnOnSkill();
        if (lvCurrent < checkPoints.Count && point >= targetCheckPoint)
        {
            lvCurrent += 1;
            var targetPoint = GetCheckPointData(lvCurrent);
            targetCheckPoint = targetPoint.checkPoint;
            if (lvCurrent > 0)
            {
                durProgress = GetCheckPointData(lvCurrent - 1).checkPoint;
            }
            ChangeSpeed(targetPoint.speedMove);
            SetScaleUpLevel(lvCurrent);
            UIManager.Ins.GetUI<UIGamePlay>().ReLoadUI();
            isMagnetic = true;
        }
        CheckSpanEnemy();
    }
    public void CheckSpanEnemy()
    {
        if (isCreateEnemy && lvCurrent >= 3)
        {
            isCreateEnemy = false;
            StartCoroutine(IE_DelaySpawn());
        }
    }
    public void CheckTurnOnSkill()
    {
        if (isMagnetic && lvCurrent >= 1)
        {
            float targetSkill = (targetCheckPoint * 2) / 3;
            if (point >= targetSkill)
            {
                isMagnetic = false;
                StartCoroutine(IE_Spawn());
            }
        }
    }
    IEnumerator IE_Spawn()
    {
        yield return new WaitForSeconds(0.5f);
        SkillManager.Ins.SpawnObjMagnet();
    }
    IEnumerator IE_DelaySpawn()
    {
        yield return new WaitForSeconds(0.5f);
        EnemyManager.Ins.SpawmEnemy();
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
