using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class PlayerController : PlayController
{
    [SerializeField]
    private float _rotateSpeed = 200.0f;
    [SerializeField]
    private float _xRotateMove;
    [SerializeField]
    private float _damage = 300f;

    private int _movePacketCnt = 0;
    private Text _text;

    private Quaternion _prevCameraLocalRotation;
    private Network _network;
    HpMpController _hpMpController;

    void OnDrawGizmos()
    {

        float maxDistance = 2;
        RaycastHit hit;
        int layerMask = 1 << LayerMask.NameToLayer("Enemy");
        // Physics.SphereCast (�������� �߻��� ��ġ, ���� �ݰ�, �߻� ����, �浹 ���, �ִ� �Ÿ�)
        bool isHit = Physics.SphereCast(transform.position + Vector3.up, transform.lossyScale.x / 2, transform.forward, out hit, maxDistance, layerMask);
        Gizmos.color = Color.red;
        if (isHit)
        {
            Gizmos.DrawRay(transform.position + Vector3.up, transform.forward * hit.distance);
            Gizmos.DrawWireSphere((transform.position + Vector3.up) + (transform.forward * hit.distance), transform.lossyScale.x / 2);
        }
        else
        {
            Gizmos.DrawRay(transform.position + Vector3.up, transform.forward * maxDistance);
        }
    }

    IEnumerator movePacketCount()
    {
        while (true)
        {
            yield return new WaitForSeconds(1.0f);
            // Debug.Log($"movePacket Cnt:{_movePacketCnt}");
            _movePacketCnt = 0;
        }
    }

    public override void CInit()
    {
        base.CInit();
        _network = FindObjectOfType<Network>();
        _agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        _agent.updateRotation = false;
        _agent.enabled = true;
        _hpMpController = GameObject.FindGameObjectWithTag("HpMp").GetComponent<HpMpController>();
    }

    public void Init(Quaternion cameraLocalRotation, GameObject camera)
    {
        StartCoroutine(movePacketCount());
        _camera = camera;
        _cameraLocalRotation = cameraLocalRotation;
        GameObject text_ui = GameObject.FindGameObjectWithTag("Respawn");
    }

    public void Respwan(Vector3 pos, Type.State state, Type.Dir dir, Type.Dir mouseDir, Quaternion q, float hp, float mp) 
    {
        _agent.Warp(pos);
        _state = state;
        _dir = dir;
        _mouseDir = mouseDir;
        _hpMpController.SetHp(hp);
        _hpMpController.SetMp(mp);
        _death = false;
    }

    public void SetAgentPos(Vector3 pos) 
    {
        GetComponent<UnityEngine.AI.NavMeshAgent>().Warp(pos);
    }

    public override void KeyBoardMove_Update_Input()
    {
        if (_death) return;

        if (_coAttacked == true)
            return;

        if (_coAttack == false)
            _state = Type.State.IDLE;

        if (_state == Type.State.ATTACK)
            return;

        if (_movePacketCnt > 8)
        {
            _dir = Type.Dir.NONE;
            _state = Type.State.IDLE;
            _mouseDir = Type.Dir.NONE;
            return;
        }

        if (Input.GetMouseButtonUp(1))
        {
            _state = Type.State.ATTACK;
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

        if (Input.GetKeyUp(KeyCode.A) && (_dir & (Type.Dir.LEFT)) != 0)
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

    public override void KeyBoardMove_Update_IDLE()
    {
        if (_death) return;

        if (_coAttacked == true)
            return;

        if (_mouseDir != Type.Dir.NONE)
        {
            _xRotateMove = _mouseDir == Type.Dir.LEFT ? -1 : 1;

            _xRotateMove = _xRotateMove * Time.deltaTime * _rotateSpeed;

            _camera.transform.RotateAround(transform.position, Vector3.up, _xRotateMove);

            _cameraLocalRotation = _camera.transform.localRotation;
        }
    }

    public override void KeyBoardMove_Update_MOVE()
    {
        if (_death) return;

        if (_coAttacked == true)
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

    public override void KeyBoardMove_Update_ATTACK()
    {
        if (_coAttack == false)
            StartCoroutine(CoAttack());
    }

    public override void KeyBoardMove_Update_ATTACKED()
    {
        if (_coAttacked == false)
            StartCoroutine(CoAttacked());
    }

    public override void UpdateAnimation()
    {
        if (_death) return;

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

    public override void SendSyncPlayer()
    {
        byte[] bytes = new byte[74];
        MemoryStream ms = new MemoryStream(bytes);
        ms.Position = 0;

        BinaryWriter bw = new BinaryWriter(ms);
        bw.Write((Int16)Type.PacketProtocol.C2S_PLAYERSYNC);
        bw.Write((Int16)74);
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

        bw.Write((float)_target.x); // 4
        bw.Write((float)_target.y); // 4
        bw.Write((float)_target.z); // 4

        bw.Write((Int32)_moveType); // 4

        bw.Write((float)transform.localRotation.x);
        bw.Write((float)transform.localRotation.y);
        bw.Write((float)transform.localRotation.z);
        bw.Write((float)transform.localRotation.w);

        _network.SendPacket(bytes, 74);
        _movePacketCnt++;
    }

    public override void SendSyncMap()
    {
        byte[] bytes = new byte[74];
        MemoryStream ms = new MemoryStream(bytes);
        ms.Position = 0;

        BinaryWriter bw = new BinaryWriter(ms);
        bw.Write((Int16)Type.PacketProtocol.C2S_MAPSYNC);
        bw.Write((Int16)74);
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

        bw.Write((float)_target.x); // 4
        bw.Write((float)_target.y); // 4
        bw.Write((float)_target.z); // 4

        bw.Write((Int32)_moveType); // 4

        bw.Write((float)transform.localRotation.x);
        bw.Write((float)transform.localRotation.y);
        bw.Write((float)transform.localRotation.z);
        bw.Write((float)transform.localRotation.w);

        _network.SendPacket(bytes, 74);
    }
    public override void MouseMove_Update_Input()
    {
        if (Input.GetMouseButtonUp(0))
        {
            _mouseDir = Type.Dir.NONE;
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

        if (Input.GetMouseButtonUp(1))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            //����ĳ��Ʈ �浹 �߻� ��
            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                //�浹�� ������Ʈ�� �±׷� �����ؼ� OK�� ���
                _agent.SetDestination(hit.point);
                _state = Type.State.MOVE;
            }
            return;
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
        NavMeshPath navMesh = _agent.path;

        for (int i = 0; i < navMesh.corners.Length - 1; i++)
        {
            Debug.DrawLine(navMesh.corners[i], navMesh.corners[i + 1], Color.red);
        }

        if (navMesh.corners.Length >= 2)
        {
            Vector3 dest = new Vector3(navMesh.corners[1].x, transform.position.y, navMesh.corners[1].z);
            transform.LookAt(dest);
            _target = navMesh.corners[1];
        }

        if (_mouseDir != Type.Dir.NONE)
        {
            _xRotateMove = _mouseDir == Type.Dir.LEFT ? -1 : 1;

            _xRotateMove = _xRotateMove * Time.deltaTime * _rotateSpeed;

            _camera.transform.RotateAround(transform.position, Vector3.up, _xRotateMove);

            _cameraLocalRotation = _camera.transform.localRotation;
        }

        if (_agent.velocity.sqrMagnitude >= 0.2f * 0.2f && _agent.remainingDistance <= 0.1f)
        {
            _state = Type.State.IDLE;
        }
    }

    IEnumerator CoAttack() 
    {
        _coAttack = true;
        yield return new WaitForSeconds(0.1f);

        RaycastHit hit;
        int layerMask = 1 << LayerMask.NameToLayer("Enemy");

        if (Physics.SphereCast(transform.position + Vector3.up, transform.lossyScale.x / 2, transform.TransformDirection(Vector3.forward), out hit, 2, layerMask))
        {
            Gizmos.color = Color.red;

            PlayController pc = hit.transform.gameObject.GetComponent<PlayController>();
            Int32 otherPlayerId = pc.PlayerID;

            byte[] bytes = new byte[12];
            MemoryStream ms = new MemoryStream(bytes);
            ms.Position = 0;

            BinaryWriter bw = new BinaryWriter(ms);
            bw.Write((Int16)Type.PacketProtocol.C2S_PLAYERATTACK);
            bw.Write((Int16)12);
            bw.Write((Int32)otherPlayerId);
            bw.Write((Int32)_damage);
            _network.SendPacket(bytes, 12);
        }
        yield return new WaitForSeconds(0.5f);
        _coAttack = false;
        _dir = Type.Dir.NONE;
    }

    IEnumerator CoAttacked()
    {
        _coAttacked = true;
        yield return new WaitForSeconds(1.2f);
        _dir = Type.Dir.NONE;
        _coAttacked = false;
    }

    IEnumerator CoDeath()
    {
        _animator.Play("knight_death");
        yield return new WaitForSeconds(4.0f);
        // TODO ������ ��Ŷ
        byte[] bytes = new byte[12];
        MemoryStream ms = new MemoryStream(bytes);
        ms.Position = 0;
        BinaryWriter bw = new BinaryWriter(ms);
        bw.Write((Int16)Type.PacketProtocol.C2S_PLAYERESPAWN);
        bw.Write((Int16)4);
        _network.SendPacket(bytes, 4);

    }

    public override void Attacked() 
    {
        if (_coAttacked) return;
        _animator.Play("knight_attacked");
        StartCoroutine(CoAttacked());
    }

    public override void SetHp(float hp)
    {
        GameObject.FindGameObjectWithTag("HpMp").GetComponent<HpMpController>().SetHp(hp);
    }

    public override void SetMp(float mp)
    {
        GameObject.FindGameObjectWithTag("HpMp").GetComponent<HpMpController>().SetMp(mp);
    }

    public override void Death()
    {
        _death = true;
        StartCoroutine(CoDeath());
    }
}
