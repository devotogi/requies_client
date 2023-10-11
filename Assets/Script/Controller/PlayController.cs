using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayController : CreatureController
{
    public int PlayerID { get; set; }
 
    protected float _speed = 3.0f;
    protected GameObject _camera = null;
    protected Type.MoveType _moveType = Type.MoveType.KeyBoard;
    protected Vector3 _target = Vector3.zero;
    protected NavMeshAgent _agent;
    protected Quaternion _cameraLocalRotation;
    protected Type.Dir _mouseDir = Type.Dir.NONE;
    protected Vector3 _dirVector3 = Vector3.zero;
    protected bool _coAttack = false;
    protected bool _coAttacked = false;
    private Vector3 _mousePrevPos = Vector3.zero;
    private void OnDestroy()
    {
        if (_camera != null)
            UnityEngine.Object.Destroy(_camera.transform.parent.gameObject);
    }

    void Update()
    {
        switch (_moveType) 
        {
            case Type.MoveType.Mouse:
                MouseMoveUpdate();
                break;

            case Type.MoveType.KeyBoard:
                KeyBoardMoveUpdate();
                break;
        }

        UpdateAnimation();
    }

    public virtual void UpdateSync(Type.MoveType moveType, Type.State state, Type.Dir dir, Type.Dir mouseDir, Vector3 nowPos, Quaternion quaternion, Vector3 target, Quaternion localRotation)
    {
        
    }

    public void MouseMoveUpdate() 
    {
        Type.State prevState = _state;
        Vector3 prevTarget = _target;

        MouseMove_Update_Input();

        switch (_state) 
        {
            case Type.State.IDLE:
                MouseMove_Update_IDLE();
                break;

            case Type.State.MOVE:
                MouseMove_Update_MOVE();
                break;
        }

        if (prevTarget != _target || prevState != _state)
            SendSyncPlayer();

        Vector3 nowPos = transform.position;

        int prevX = (int)_mousePrevPos.x;
        int prevZ = (int)_mousePrevPos.z;
        int nowX = (int)nowPos.x;
        int nowZ = (int)nowPos.z;

        if (prevX != nowX || prevZ != nowZ)
        { 
            SendSyncMap();
            _mousePrevPos = nowPos;
        }
    }

    public void KeyBoardMoveUpdate() 
    {
        Vector3 prevPos = transform.position;
        Type.Dir prevDir = _dir;
        Type.State prevState = _state;
        Type.Dir prevMouseDir = _mouseDir;

        KeyBoardMove_Update_Input();

        switch (_state)
        {
            case Type.State.IDLE:
                KeyBoardMove_Update_IDLE();
                break;

            case Type.State.MOVE:
                KeyBoardMove_Update_MOVE();
                break;

            case Type.State.ATTACK:
                KeyBoardMove_Update_ATTACK();
                break;
        }

        if (prevState != _state || prevDir != _dir || prevMouseDir != _mouseDir)
            SendSyncPlayer();

        Vector3 nowPos = transform.position;

        int prevX = (int)prevPos.x;
        int prevZ = (int)prevPos.z;
        int nowX = (int)nowPos.x;
        int nowZ = (int)nowPos.z;

        if (prevX != nowX || prevZ != nowZ)
            SendSyncMap();
    }

    public virtual void MouseMove_Update_IDLE() { }
    public virtual void MouseMove_Update_MOVE() { }
    public virtual void MouseMove_Update_Input() { }
    public virtual void KeyBoardMove_Update_IDLE() { }
    public virtual void KeyBoardMove_Update_MOVE() { }
    public virtual void KeyBoardMove_Update_Input() { }
    public virtual void KeyBoardMove_Update_ATTACK() { }
    public virtual void KeyBoardMove_Update_ATTACKED() { }
    public virtual void UpdateAnimation() { }
    public virtual void SendSyncPlayer() { }
    public virtual void SendSyncMap() { }

    public virtual void SetHp(float hp) { }
    public virtual void SetMp(float hp) { }
}
