using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : CreatureController
{
    public int MonsterId { get { return _monsterId; } set { _monsterId = value; } }
    public Type.MonsterType MonsterType {get { return _monsterType; } set { _monsterType = value; } }
    public float HP { get { return _hp; } set { _hp = value; } }
    public HpController HPC { get { return _hpController; } set { _hpController = value; _hpController.SetHPMax(1000); } }
    private Type.MonsterType _monsterType;
    private int _monsterId;
    private float _hp;
    private HpController _hpController = null;

    private void LateUpdate()
    {
        if (_hpController != null)
        {
            _hpController.transform.position = transform.position + Vector3.up * 2.5f;
            _hpController.LookMainCamera();
        }
    }

    void Update()
    {
        switch (_state) 
        {
            case Type.State.IDLE:
                Update_IDLE();
                break;

            case Type.State.MOVE:
                Update_MOVE();
                break;

            case Type.State.ATTACK:
                Update_ATTACK();
                break;

            case Type.State.ATTACKED:
                Update_ATTACKED();
                break;

            case Type.State.DEATH:
                Update_DEATH();
                break;
        }

        UpdateAnimation();
    }

    internal void Sync(Type.State monsterState, Vector3 pos, float hp)
    {
        _state = monsterState;
        transform.position = pos;
        _hp = hp;
        SetHp(hp);
    }

    void Update_IDLE() 
    {
    
    }

    void Update_MOVE() 
    {
    
    }
    
    void Update_ATTACK() 
    {
    
    }
    void Update_ATTACKED() { }
    void Update_DEATH() { }

    void UpdateAnimation() 
    {
        switch (_state)
        {
            case Type.State.IDLE:
                _animator.Play("CombatIdle");
                break;

            case Type.State.MOVE:
                break;

            case Type.State.ATTACK:
                break;

            case Type.State.ATTACKED:
                _animator.Play("Hit");
                break;

            case Type.State.DEATH:
                break;
        }
    }

    public override void Destory()
    {
        base.Destory();
        if (_hpController)
            Managers.Resource.Destory(_hpController.gameObject);
    }
    public void SetHp(float hp)
    {
        if (_hpController)
            _hpController.SetHp(hp);
    }
}
