using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherPlayerController : PlayerAbleController
{
    private float _xRotateMove;
    private float _rotateSpeed = 200.0f;

    public void Start()
    {
        _animator = GetComponent<Animator>();
        _network = FindObjectOfType<Network>();
        _agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        _agent.enabled = false;
    }

    public void Init(Quaternion quaternion, GameObject camera)
    {
        _camera = camera;
        _cameraLocalRotation = quaternion;
    }

    public override void UpdateIdel_MousePos()
    {
      
    }

    public override void UpdateMove_MousePos()
    { 
        Vector3 dirVector3 = _target - transform.position;
        transform.position += dirVector3.normalized * Time.deltaTime* _speed;
    }

    public override void UpdateIdel() 
    {
        if (_camera != null && _mouseDir != Type.Dir.NONE)
        {
            _xRotateMove = _mouseDir == Type.Dir.LEFT ? -1 : 1;

            _xRotateMove = _xRotateMove * Time.deltaTime * _rotateSpeed;

            _camera.transform.RotateAround(transform.position, Vector3.up, _xRotateMove);

            _cameraLocalRotation = _camera.transform.localRotation;
        }
    }

    public override void UpdateMove()
    {
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

        Vector3 cameraFRVector = cameraFVector + cameraRVector;
        Vector3 cameraRBVector = cameraBVector + cameraRVector;
        Vector3 cameraLBVector = cameraLVector + cameraBVector;
        Vector3 cameraLFVector = cameraLVector + cameraFVector;

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
        switch (_state)
        {
            case Type.State.IDLE:
                _animator.Play("knight_idle");
                break;

            case Type.State.MOVE:
                _animator.Play("knight_run");
                break;

            case Type.State.ATTACK:
                Debug.DrawRay(transform.position + Vector3.up, transform.TransformDirection(Vector3.forward) * 2, Color.red);
                _animator.Play("knight_attack");
                break;
        }
    }

    public override void Attacked()
    {
        base.Attacked();
    }
}
