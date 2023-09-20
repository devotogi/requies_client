using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : CreatureController
{
    private float _rotateSpeed = 200.0f;
    private float _xRotateMove;
    private int _movePacketCnt = 0;
    private Text _text;

    IEnumerator movePacketCount() 
    {
        while (true) 
        {
            yield return new WaitForSeconds(1.0f);
            //Debug.Log($"movePacket Cnt:{_movePacketCnt}");
            _movePacketCnt = 0;
        }
    }

    public void Init(Quaternion quaternion, GameObject camera) {
        StartCoroutine(movePacketCount());
        _camera = camera;
        _cameraLocalRotation = quaternion;

        GameObject text_ui = GameObject.FindGameObjectWithTag("Respawn");
        _text = text_ui.GetComponent<Text>();
       
    }

    public override void UpdateInput()
    {
        if (_movePacketCnt > 8)
        {
            //Debug.Log("Man Key Input Lock!");
            _dir = Type.Dir.NONE;
            _state = Type.State.IDLE;
            _mouseDir = Type.Dir.NONE;
            return;
        }

        if (_state == Type.State.ATTACK)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _state = Type.State.ATTACK;
            StartCoroutine(CoAttack());
            return;
        }

        if (Input.GetMouseButtonUp(0))
        {
            _mouseDir = Type.Dir.NONE;
        }

        if (Input.GetKeyUp(KeyCode.W) && (_dir & (Type.Dir.UP)) != 0)
        {
            _dir = (Type.Dir)((UInt16)_dir ^ (0x01 << 1));
        }

        if (Input.GetKeyUp(KeyCode.D) && (_dir & (Type.Dir.RIGHT)) != 0)
        {
            _dir = (Type.Dir)((UInt16)_dir ^ (0x01 << 2));
        }

        if (Input.GetKeyUp(KeyCode.S) && (_dir & (Type.Dir.DOWN)) != 0)
        {
            _dir = (Type.Dir)((UInt16)_dir ^ (0x01 << 3));
        }

        if (Input.GetKeyUp(KeyCode.A) &&  (_dir & (Type.Dir.LEFT)) != 0)
        {
            _dir = (Type.Dir)((UInt16)_dir ^ (0x01 << 4));
        }

        if (_dir == Type.Dir.NONE)
        {
            _dir = Type.Dir.NONE;
            _state = Type.State.IDLE;
        }

        if (Input.GetMouseButton(0))
        {
            _prevCameraLocalRotation = _cameraLocalRotation;

            _xRotateMove = Input.GetAxis("Mouse X"); /** Time.deltaTime * _rotateSpeed;*/

            if (_xRotateMove < 0)
            {
                _mouseDir = Type.Dir.LEFT;
            }
            else if (_xRotateMove > 0)
            {
                _mouseDir = Type.Dir.RIGHT;
            }
        }

       
        if (Input.GetKey(KeyCode.W))
        {
            _dir = (Type.Dir)((UInt16)_dir | (0x01 << 1));

            if ((_dir & (Type.Dir.DOWN)) != 0)
                _dir = (Type.Dir)((UInt16)_dir ^ (0x01 << 3));

            _state = Type.State.MOVE;
        }
        if (Input.GetKey(KeyCode.D))
        {
            _dir = (Type.Dir)((UInt16)_dir | (0x01 << 2));
            _state = Type.State.MOVE;
        }
 
        if (Input.GetKey(KeyCode.S))
        {
            _dir = (Type.Dir)((UInt16)_dir | (0x01 << 3));

            if ((_dir & (Type.Dir.UP)) != 0)
                _dir = (Type.Dir)((UInt16)_dir ^ (0x01 << 1));

            _state = Type.State.MOVE;
        }
     
        if (Input.GetKey(KeyCode.A))
        {
            _dir = (Type.Dir)((UInt16)_dir | (0x01 << 4));
            _state = Type.State.MOVE;
        }
    }

    public override void UpdateAttack() 
    {
        int a = 3;
    }

    public override void UpdateIdel()
    {
        if (_mouseDir != Type.Dir.NONE)
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
        _text.text = $"Pos X:{(int)transform.position.x}, Z:{(int)transform.position.z}";
        
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

    public override void SendSyncPlayer() 
    {
        byte[] bytes = new byte[42];
        MemoryStream ms = new MemoryStream(bytes);
        ms.Position = 0;

        BinaryWriter bw = new BinaryWriter(ms);
        bw.Write((Int16)Type.PacketProtocol.C2S_PLAYERSYNC);
        bw.Write((Int16)42);
        bw.Write((Int32)PlayerID);
        bw.Write((UInt16)_state); // 2
        bw.Write((UInt16)_dir); // 2
        bw.Write((UInt16)_mouseDir); // 2

        bw.Write((float)transform.position.x); // 4
        bw.Write((float)transform.position.y); // 4
        bw.Write((float)transform.position.z); // 4

        bw.Write((float)_cameraLocalRotation.x); // 4
        bw.Write((float)_cameraLocalRotation.y); // 4
        bw.Write((float)_cameraLocalRotation.z); // 4
        bw.Write((float)_cameraLocalRotation.w); // 4
        
        _network.SendPacket(bytes, 42);
        _movePacketCnt++;
    }

    public override void SendSyncMap() 
    {
        byte[] bytes = new byte[42];
        MemoryStream ms = new MemoryStream(bytes);
        ms.Position = 0;

        BinaryWriter bw = new BinaryWriter(ms);
        bw.Write((Int16)Type.PacketProtocol.C2S_MAPSYNC);
        bw.Write((Int16)42);
        bw.Write((Int32)PlayerID);
        bw.Write((UInt16)_state); // 2
        bw.Write((UInt16)_dir); // 2
        bw.Write((UInt16)_mouseDir); // 2

        bw.Write((float)transform.position.x); // 4
        bw.Write((float)transform.position.y); // 4 
        bw.Write((float)transform.position.z); // 4

        bw.Write((float)_cameraLocalRotation.x); // 4
        bw.Write((float)_cameraLocalRotation.y); // 4
        bw.Write((float)_cameraLocalRotation.z); // 4
        bw.Write((float)_cameraLocalRotation.w); // 4

        _network.SendPacket(bytes, 42);
    }

    IEnumerator CoAttack() 
    {
        yield return new WaitForSeconds(1.5f);
        _state = Type.State.IDLE;
        _dir = Type.Dir.NONE;
    }
}
