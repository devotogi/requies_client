using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureController : MonoBehaviour
{
    public int PlayerID { get; set; }
    protected Type.Dir _dir = Type.Dir.NONE;
    protected Type.Dir _mouseDir = Type.Dir.NONE;
    protected Type.State _state = Type.State.IDLE;
    protected Animator _animator;
    protected float _speed = 3.0f;
    protected Network _network;
    protected Quaternion _cameraLocalRotation;
    protected Quaternion _prevCameraLocalRotation;
    protected Vector3 _dirVector3 = Vector3.zero;
    protected GameObject _camera;


    void OnDestroy()
    {
        if (_camera != null)
        {
            Object.Destroy(_camera.transform.parent.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _network = FindObjectOfType<Network>();
    }

    // Update is called once per frame
    void Update()
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

        int prevX = (int) prevPos.x;
        int prevZ = (int) prevPos.z;
        int nowX = (int)nowPos.x;
        int nowZ = (int)nowPos.z;

        if (prevX != nowX || prevZ != nowZ) 
        {
            // �� ����ȭ
            SendSyncMap();
        }
    }

    public void UpdateSync(Type.State state, Type.Dir dir, Type.Dir mouseDir, Vector3 nowPos, Quaternion quaternion) 
    {
        _mouseDir = mouseDir;
        _state = state;
        _dir = dir;
        transform.position = new Vector3(nowPos.x, nowPos.y, nowPos.z);
        _cameraLocalRotation = quaternion;
        _camera.transform.localRotation = _cameraLocalRotation;
    }

    public virtual void UpdateInput() 
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
        }
    }

    public virtual void UpdateIdel() { }

    public virtual void UpdateMove()
    {

    }

    public virtual void UpdateAnimation()
    {

    }

    public virtual void SendSyncPlayer() { }

    public virtual void SendSyncMap() { }

    public void Destory() 
    {
        Destroy(gameObject);
    }
}