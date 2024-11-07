using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMagnet : Magnet
{
    private float currentTime = 0f;
    public Transform playerTF;
    public Transform playerSkill;
    public bool isMaget = false;

    private float radius;          

    public void Oninit()
    {
        currentTime = 0f;
        player = LevelManager.Ins.player;
        blackHoleCenter = player.blackHoleCenter;
        playerTF = player.transform;
        playerSkill = player.playerSkill;
        isMaget = false;
    }
    public void SetTransform()
    {
        radius = Vector3.Distance(playerTF.position, playerSkill.position);
      
        this.transform.localScale = player.transform.localScale;
    }

    void Update()
    {
        if (isMaget && player != null)
        {
            float angle = Const.SPEEDROTATE * Time.deltaTime;
            Vector3 direction = (transform.position - playerTF.position).normalized;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);
            radius = Vector3.Distance(playerTF.position, playerSkill.position);
            Vector3 newPosition = playerTF.position + rotation * (direction * radius);
            transform.position = newPosition;
            currentTime += Time.deltaTime;
            if (currentTime >= Const.ROTATETIME)
            {
                isMaget = false;
                currentTime = 0f;
                DespawnObj();
            }
        }
    }
    public void DespawnObj()
    {
        SimplePool.Despawn(this);
    }

    public void IsRotate(bool isRot)
    {
        isMaget = isRot;
    }
    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
    }
}
