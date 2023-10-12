using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherPlayerController : PlayController
{
    private float _xRotateMove;
    private float _rotateSpeed = 200.0f;
    private HpController _hpController = null;

    private void LateUpdate()
    {
        if (_hpController != null)
            _hpController.transform.position = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 1.9f);
    }

    public override void CInit()
    {
        base.CInit();
        _agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        _agent.updateRotation = false;
        _agent.enabled = true;
    }

    public void Init(Quaternion cameraLocalRotation, GameObject camera, HpController hpc)
    {
        _camera = camera;
        _cameraLocalRotation = cameraLocalRotation;
        _hpController = hpc;
    }
    public override void KeyBoardMove_Update_IDLE()
    {
        if (_coAttacked)
            return;

        if (_camera != null && _mouseDir != Type.Dir.NONE)
        {
            _xRotateMove = _mouseDir == Type.Dir.LEFT ? -1 : 1;

            _xRotateMove = _xRotateMove * Time.deltaTime * _rotateSpeed;

            _camera.transform.RotateAround(transform.position, Vector3.up, _xRotateMove);

            _cameraLocalRotation = _camera.transform.localRotation;
        }
    }

    public override void KeyBoardMove_Update_MOVE()
    {
        if (_coAttacked)
            return;

        if (_mouseDir != Type.Dir.NONE)
        {
            _xRotateMove = _mouseDir == Type.Dir.LEFT ? -1 : 1;

            _xRotateMove = _xRotateMove * Time.deltaTime * _rotateSpeed;

            _camera.transform.RotateAround(transform.position, Vector3.up, _xRotateMove);

            _cameraLocalRotation = _camera.transform.localRotation;
        }

        Vector3 cameraFVector = _cameraLocalRotation * Vector3.forward;
        cameraFVector.y = 0;
        cameraFVector = cameraFVector.normalized;

        Vector3 cameraRVector = _cameraLocalRotation * Vector3.right;
        cameraRVector.y = 0;
        cameraRVector = cameraRVector.normalized;

        Vector3 cameraBVector = _cameraLocalRotation * Vector3.back;
        cameraBVector.y = 0;
        cameraBVector = cameraBVector.normalized;

        Vector3 cameraLVector = _cameraLocalRotation * Vector3.left;
        cameraLVector.y = 0;
        cameraLVector = cameraLVector.normalized;

        Vector3 cameraFRVector = (cameraFVector + cameraRVector).normalized;
        Vector3 cameraRBVector = (cameraBVector + cameraRVector).normalized;
        Vector3 cameraLBVector = (cameraLVector + cameraBVector).normalized;
        Vector3 cameraLFVector = (cameraLVector + cameraFVector).normalized;

        switch (_dir)
        {
            case Type.Dir.UP:
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(cameraFVector), 0.2f);
                transform.position += cameraFVector * Time.deltaTime * _speed;
                _dirVector3 = cameraFVector;
                break;
            case Type.Dir.UPRIGHT:
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(cameraFRVector), 0.2f);
                transform.position += cameraFRVector * Time.deltaTime * _speed;
                _dirVector3 = cameraFRVector;
                break;
            case Type.Dir.RIGHT:
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(cameraRVector), 0.2f);
                transform.position += cameraRVector * Time.deltaTime * _speed;
                _dirVector3 = cameraRVector;
                break;
            case Type.Dir.RIGHTDOWN:
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(cameraRBVector), 0.2f);
                transform.position += cameraRBVector * Time.deltaTime * _speed;
                _dirVector3 = cameraRBVector;
                break;
            case Type.Dir.DOWN:
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(cameraBVector), 0.2f);
                transform.position += cameraBVector * Time.deltaTime * _speed;
                _dirVector3 = cameraBVector;
                break;
            case Type.Dir.LEFTDOWN:
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(cameraLBVector), 0.2f);
                transform.position += cameraLBVector * Time.deltaTime * _speed;
                _dirVector3 = cameraLBVector;
                break;
            case Type.Dir.LEFT:
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(cameraLVector), 0.2f);
                transform.position += cameraLVector * Time.deltaTime * _speed;
                _dirVector3 = cameraLVector;
                break;
            case Type.Dir.LEFTUP:
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(cameraLFVector), 0.2f);
                transform.position += cameraLFVector * Time.deltaTime * _speed;
                _dirVector3 = cameraLFVector;
                break;
        }
    }

    public override void UpdateAnimation()
    {
        if (_coAttacked == true)
            return;

        switch (_state)
        {
            case Type.State.IDLE:
                _animator.Play("knight_idle");
                break;

            case Type.State.MOVE:
                _animator.Play("knight_run");
                break;

            case Type.State.ATTACK:
                _animator.Play("knight_attack");
                break;
        } 
    }

    public override void MouseMove_Update_IDLE()
    {
        if (_mouseDir != Type.Dir.NONE)
        {
            _xRotateMove = _mouseDir == Type.Dir.LEFT ? -1 : 1;

            _xRotateMove = _xRotateMove * Time.deltaTime * _rotateSpeed;

            _camera.transform.RotateAround(transform.position, Vector3.up, _xRotateMove);

            _cameraLocalRotation = _camera.transform.localRotation;
        }
    }

    public override void MouseMove_Update_MOVE()
    {
        Vector3 dirVector3 = _target - transform.position;
        transform.position += dirVector3.normalized * Time.deltaTime * _speed;
    }

    public override void UpdateSync(Type.MoveType moveType, Type.State state, Type.Dir dir, Type.Dir mouseDir, Vector3 nowPos, Quaternion quaternion, Vector3 target, Quaternion localRotation)
    {
        _moveType = moveType;
        _mouseDir = mouseDir;
        _state = state;
        _dir = dir;
        GetComponent<UnityEngine.AI.NavMeshAgent>().Warp(new Vector3(nowPos.x, nowPos.y, nowPos.z));
        _cameraLocalRotation = quaternion;
        _camera.transform.localRotation = _cameraLocalRotation;
        transform.localRotation = localRotation;
        _target = target;

        if (_moveType == Type.MoveType.Mouse && _state == Type.State.MOVE)
        {
            Vector3 dest = new Vector3(_target.x, transform.position.y, _target.z);
            transform.LookAt(dest);
        }
    }

    IEnumerator CoAttacked()
    {
        _coAttacked = true;
        yield return new WaitForSeconds(1.2f);
        _dir = Type.Dir.NONE;
        _coAttacked = false;
    }

    public override void Attacked()
    {
        if (_coAttacked) return;
        _animator.Play("knight_attacked");
        StartCoroutine(CoAttacked());
    }

    public override void Destory()
    {
        base.Destory();
        if (_hpController)
            Managers.Resource.Destory(_hpController.gameObject);
    }

    public override void SetHp(float hp)
    {
        if (_hpController)
            _hpController.SetHp(hp);
    }
}
