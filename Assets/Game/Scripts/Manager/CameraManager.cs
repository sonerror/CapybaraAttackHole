using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : Singleton<CameraManager>
{
    public Player player;
    public CinemachineVirtualCamera virtualCamera;
    private CinemachineFramingTransposer framingTransposer;
    private void Start()
    {
        framingTransposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
    }
    public void SetData(Player _player)
    {
        player = _player;
        virtualCamera.Follow = player.transform;
    }
    public void AdjustCameraDistance(float newDistance)
    {
        if (framingTransposer != null)
        {
            framingTransposer.m_CameraDistance = newDistance;
        }
    }
    public Vector3 SetTFCamera()
    {
        return virtualCamera.transform.position;
    }
    public void AdjustCamera(Vector3 targetRotation, float duration)
    {
        virtualCamera.gameObject.transform
            .DORotate(targetRotation, duration, RotateMode.FastBeyond360);
    }
    public void SetTransform()
    {
        AdjustCamera(new Vector3(10f, 3.5f, 0f), 1f);
    }
}
