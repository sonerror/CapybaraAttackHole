
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] public float moveSpeed = 5f;
    [SerializeField] private float rotateSpeed = 5f;
    public static Player Instance { get; private set; }
    public List<CheckPoint> checkPoints;
    public int lvPlayer = 1;
    public bool isMove;
    public bool move = true;
    public void Start()
    {
        OnInit();
    }
    public override void OnInit()
    {
        base.OnInit();
      
    }
    public void GetDataLevel(List<CheckPoint> _checkPoint)
    {
        this.checkPoints = _checkPoint;
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
}
