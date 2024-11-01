using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Player : Character
{
    [SerializeField] private float rotateSpeed = 5f;
    private Quaternion targetRotation;
    private Vector3 inputDirection = Vector3.zero;
    private bool isCreateEnemy;
    public Transform mouth;
    public override void OnInit()
    {
        base.OnInit();
        move = true;
        isCreateEnemy = true;
        isMagnetic = true;
        isAttack = true;
        isMoving = false;
    }
    private void Update()
    {
        if (GameManager.Ins.gameState != GameState.GamePlay || !move) return;
        Vector3 currentInputDirection = GetInputDirection();
        if (currentInputDirection.sqrMagnitude > 0.001f)
        {
            inputDirection = currentInputDirection;
            Move(inputDirection);
            isMoving = true;
        }
        else
        {
            StopMovement();
            isMoving = false;
        }

    }
    public float GetDataScale()
    {
       return checkPoints.Find(x => x.id == lvCurrent).scale;
    }
    public override void SetScale(int _lvScale)
    {
        base.SetScale(_lvScale);
        SetCamera(_lvScale);
    }
    public override void SetScaleUpLevel(int _lvScale)
    {
        base.SetScaleUpLevel(_lvScale);
        SetCamera(_lvScale);
    }
    public void SetCamera(int _levelCurrent)
    {
        CameraManager.Ins.AdjustCameraDistanceSmooth(GetCheckPointData(_levelCurrent).cameraDistance, 0.5f);
    }
    public void ChangeCamera(float cameraDistance)
    {
        CameraManager.Ins.AdjustCameraDistanceSmooth(cameraDistance, 1f);
    }
    public override void SetData(int _Lv, int _lvTime, int _lvEx)
    {
        base.SetData(_Lv, _lvTime, _lvEx);
        SetScale(lvCurrent);
        OnInit();
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
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotateSpeed * Time.deltaTime * 75f
            );
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


    public override void CheckPointUpLevel()
    {
        base.CheckPointUpLevel();
        //CheckTurnOnSkill();
        SetDataBonusGold();
        UIManager.Ins.GetUI<UIGamePlay>().ReLoadUI();
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
    }
}
