using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : CreatureController
{
    public float rotateSpeed = 3.0f;       // 회전 속도
    public int MonsterId { get { return _monsterId; } set { _monsterId = value; } }
    public Type.MonsterType MonsterType {get { return _monsterType; } set { _monsterType = value; } }
    public float HP { get { return _hp; } set { _hp = value; } }
    private Type.MonsterType _monsterType;
    private int _monsterId;
    private float _hp;
    private Vector3 _dir = Vector3.zero;
    private float _speed = 1.5f;
    private Vector3 _dest;
    private List<Vector2Int> _conner = new List<Vector2Int>();
    private void LateUpdate()
    {

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

    internal void Sync(Type.State monsterState, Vector3 pos, float hp, Vector3 dir, Vector3 dest, List<Vector2Int> conner)
    {
        _state = monsterState;
        transform.position = pos;
        _hp = hp;
        _dir = dir;
        transform.rotation = Quaternion.LookRotation(_dir);
        _dest = dest; 
        SetHp(hp);
        _conner = conner;

        switch (_monsterType) 
        {
            case Type.MonsterType.Bear:
                _speed = 1.5f;
                break;

            case Type.MonsterType.Skeleton:
                _speed = 2.0f;
                break;

            case Type.MonsterType.Thief:
                _speed = 2.5f;
                break;
        }
    }

    void Update_IDLE() 
    {
    
    }

    void Update_MOVE() 
    {
        transform.rotation = Quaternion.LookRotation(_dir).normalized;
        transform.position += (_dir * Time.deltaTime * _speed);


        Vector3 dest = new Vector3(_conner[0].x + 0.5f, 0, _conner[0].y + 0.5f);
        Vector3 destV= dest - transform.position;
        _dir = destV.normalized;
        float distF = destV.magnitude;

        if (distF <= 0.02)
        {
            transform.position = dest;
            _conner.RemoveAt(0);

            if (_conner.Count == 0) 
                _state = Type.State.IDLE;
        }
    }
    
    void Update_ATTACK() 
    {
        transform.rotation = Quaternion.LookRotation(_dir);
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
                _animator.Play("WalkForward");
                break;

            case Type.State.ATTACK:
                _animator.Play("Attack1");
                break;

            case Type.State.ATTACKED:
                _animator.Play("Hit");
                break;

            case Type.State.DEATH:
                _animator.Play("Death");
                break;
        }
    }

    public override void Destory()
    {
        base.Destory();
    }
    public void SetHp(float hp)
    {

    }

    public void Dead() 
    {
        _state = Type.State.DEATH;
        StartCoroutine(CoDead());
    }

    IEnumerator CoDead() 
    {
        yield return new WaitForSeconds(5f);
        Managers.Resource.Destory(gameObject);
    }
}
