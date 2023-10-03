using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbleController : CreatureController
{
    public int PlayerID { get; set; }
    protected float _speed = 3.0f;
    protected Network _network;
    protected Quaternion _cameraLocalRotation;
    protected Quaternion _prevCameraLocalRotation;
    protected Vector3 _dirVector3 = Vector3.zero;
    protected GameObject _camera;
    protected Type.Dir _mouseDir = Type.Dir.NONE;
    protected Type.MoveType _moveType = Type.MoveType.Mouse;
    [SerializeField]
    protected Vector3 _target = Vector3.zero;
    [SerializeField]
    protected Vector3 _prevDir = Vector3.zero;
    protected UnityEngine.AI.NavMeshAgent _agent;
    protected Vector3 mousePrevPos = Vector3.zero;

    protected Vector3 prevEulerAngles = Vector3.zero;
    protected Vector3 nowEulerAngles = Vector3.zero;
    protected Vector3 _lookrotation = Vector3.zero;

    protected Quaternion _localRotation;

    void OnDestroy()
    {
        if (_camera != null)
        {
            UnityEngine.Object.Destroy(_camera.transform.parent.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _network = FindObjectOfType<Network>();
        _agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        _agent.updateRotation = false;
        _agent.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        switch (_moveType) 
        {
            case Type.MoveType.KeyBoard:
                UpdateKeyboardMove();
                break;

            case Type.MoveType.Mouse:
                UpdateMouseMove();
                break;
        }
    }

    public void UpdateSync(Type.State state, Type.Dir dir, Type.Dir mouseDir, Vector3 nowPos, Quaternion quaternion, Vector3 target, Quaternion localRtation, Vector3 lookRotation) 
    {
        _mouseDir = mouseDir;
        _state = state;
        _dir = dir;
        transform.position = new Vector3(nowPos.x, nowPos.y, nowPos.z);
        _cameraLocalRotation = quaternion;
        _camera.transform.localRotation = _cameraLocalRotation;
        prevEulerAngles = target;
        _target = target;
        _localRotation = localRtation;
        _lookrotation = lookRotation;

        if (_state == Type.State.MOVE)
        {
            Vector3 dest = new Vector3(_target.x, transform.position.y, _target.z);
            transform.LookAt(dest);
        }
        // Quaternion.Euler(eulerAngle.x, eulerAngle.y, eulerAngle.z);
    }
    void UpdateKeyboardMove() 
    {
        Vector3 prevPos = transform.position;
        Type.Dir prevDir = _dir;
        Type.State prevState = _state;
        Type.Dir prevMouseDir = _mouseDir;

        UpdateInput();
        UpdateState();
        UpdateAnimation();

        if (prevState != _state || prevDir != _dir || prevMouseDir != _mouseDir)
        {
            SendSyncPlayer();
        }

        Vector3 nowPos = transform.position;

        int prevX = (int)prevPos.x;
        int prevZ = (int)prevPos.z;
        int nowX = (int)nowPos.x;
        int nowZ = (int)nowPos.z;

        if (prevX != nowX || prevZ != nowZ)
        {
            SendSyncMap();
        }
    }

    void UpdateMouseMove() 
    {
        Type.State prevState = _state;
        Vector3 prevTarget = _target;
        Vector3 prevDir = _prevDir;
        Vector3 prevEulerAnglesd = prevEulerAngles;

        UpdateInput_MousePos();
        UpdateState_MoseMove();
        UpdateAnimation();

        if (prevTarget != _target || prevState != _state)
        {
            Debug.Log("동기화 패킷!");
            SendSyncPlayer();
        }

        Vector3 nowPos = transform.position;

        int nowX = (int)nowPos.x;
        int nowZ = (int)nowPos.z;

        int prevX = (int)mousePrevPos.x;
        int prevZ = (int)mousePrevPos.z;

        if (prevX != nowX || prevZ != nowZ)
        {
            SendSyncMap();
            mousePrevPos = nowPos;
        }
    }

    public virtual void UpdateState_MoseMove() 
    {
        switch (_state)
        {
            case Type.State.IDLE:
                UpdateIdel_MousePos();
                break;

            case Type.State.MOVE:
                UpdateMove_MousePos();
                break;

            case Type.State.ATTACK:
                UpdateAttack_MousePos();
                break;
        }
    }

    public virtual void UpdateAttack_MousePos()
    {

    }

    public virtual void UpdateMove_MousePos()
    {
  
    }

    public virtual void UpdateIdel_MousePos()
    {

    }

    public virtual void UpdateInput() 
    {
    
    }

    public virtual void UpdateInput_MousePos() 
    {
        
    }

    public virtual void UpdateState()
    {
        switch (_state)
        {
            case Type.State.IDLE:
                UpdateIdel();
                break;

            case Type.State.MOVE:
                UpdateMove();
                break;

            case Type.State.ATTACK:
                UpdateAttack();
                break;
        }
    }
    public virtual void UpdateIdel() { }
    public virtual void UpdateAttack() { }
    public virtual void UpdateMove()
    {

    }
    public virtual void UpdateAnimation()
    {

    }
    public virtual void SendSyncPlayer() { }
    public virtual void SendSyncMap() { }
    public override void Attacked()
    {
        Debug.Log($"{PlayerID} 피격됌 !!");
        base.Attacked();
    }
    public override void Destory()
    {
        base.Destory();
    }
}
