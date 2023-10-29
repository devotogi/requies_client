using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : CreatureController
{
    public int MonsterId { get { return _monsterId; } set { _monsterId = value; } }
    public Type.MonsterType MonsterType {get { return _monsterType; } set { _monsterType = value; } }
    public int HP { get { return _hp; } set { _hp = value; } }

    private Type.MonsterType _monsterType;
    private int _monsterId;
    private int _hp;

    void Update()
    {
          
    }
}
