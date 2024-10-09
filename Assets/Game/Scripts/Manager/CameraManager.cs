using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
    [SerializeField] private float _lerpTime = 10f;
    [SerializeField] private Player _player;
    private Vector3 _targetOffset;
    private Vector3 _offset;
    private Vector3 _targetEulerAngles;
    private Vector3 _eulerAngles;
    [SerializeField] Vector3 offsetMax;
    [SerializeField] Vector3 offsetMin;


    [SerializeField] private Vector3 _offsetFBoss;
    [SerializeField] private Vector3 _eulerAnglesFBoss;
    public void Oninit()
    {
        _player = LevelManager.Ins.player;
        this._offset = _targetOffset;
        this._eulerAngles =_targetEulerAngles;
    }
    private void LateUpdate()
    {
        if (_player != null && GameManager.Ins.gameState == GameState.GamePlay)
        {
            _offset = Vector3.Lerp(_offset, _targetOffset, _lerpTime * Time.deltaTime);
            transform.position = _player.transform.position + _offset;
            _eulerAngles = Vector3.Lerp(_eulerAngles, _targetEulerAngles, _lerpTime * Time.deltaTime);
            transform.rotation = Quaternion.Euler(_eulerAngles);
        }
    }
    public void SetRateOffset(float rate)
    {
        _targetOffset = Vector3.Lerp(offsetMin, offsetMax, rate);
    }
    public void SetTFStart()
    {
        var data = LevelManager.Ins.player.checkPoints.Find(x => x.id == _player.lvCurrent);
        this._targetOffset = data.offSet;
        this._targetEulerAngles = data.eulerAngles;
    }
    public void SetTfCamera(Vector3 _offSetStruct, Vector3 _eulerAnglesStruct)
    {
        this._targetOffset = _offSetStruct;
        this._targetEulerAngles = _eulerAnglesStruct;
        Sequence cameraSequence = DOTween.Sequence();
        cameraSequence.Join(this.transform.DOMove(_targetOffset, 0.3f).SetEase(Ease.InOutQuad));
        cameraSequence.Join(this.transform.DORotate(_targetEulerAngles, 0.3f).SetEase(Ease.InOutQuad));
    }
    public void SetCameraFBoss()
    {
        SetTfCamera(_offsetFBoss, _eulerAnglesFBoss);
    }
}
