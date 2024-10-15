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
    public void SetData( Player _player)
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
    public void SetTransform()
    {
        virtualCamera.gameObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        AdjustCameraDistance(0.6f);
    }
}
