using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Type
{
    public enum PacketProtocol : Int16
    {
        C2S_PLAYERINIT,
        S2C_PLAYERINIT,
        C2S_PLAYERSYNC,
        S2C_PLAYERSYNC,
        S2C_PLAYERLIST,
        S2C_PLAYERREMOVELIST,
        S2C_PLAYERENTER,
        S2C_PLAYEROUT,
        C2S_LATENCY,
        S2C_LATENCY,
        C2S_MAPSYNC,
        S2C_MAPSYNC,
        S2C_PLAYERNEW,
        S2C_PLAYERDESTORY,
        C2S_PLAYERATTACK,
        S2C_PLAYERATTACKED,
        C2S_PLAYERCHAT,
        S2C_PLAYERCHAT,
        S2C_PLAYERDETH,
        C2S_PLAYERESPAWN,
        S2C_PLAYERESPAWN,
        S2C_MONSTERSPAWN,
        S2C_MONSTERREMOVELIST,
        S2C_MONSTERRENEWLIST,
        C2S_MONSTERATTACKED,
        S2C_MONSTERATTACKED,
        S2C_MONSTERDEAD,
        S2C_MONSTERSYNC,
        S2C_NEWMONSTER,
        S2C_DELETEMONSTER,
        S2C_MONSTERDEADCLIENT,
        S2C_MONSTERINFO,
        S2C_PLAYEREXP,
        C2S_PLAYERSTATINFO,
        S2C_PLAYERSTATINFO,
        C2S_UPSTAT,
        C2S_LOGIN,
        S2C_LOGIN,
    }
    public static string IP { get { return "58.236.130.58"; } }
    public static int FieldPORT { get { return 30002; } }
    public static int LoginPort { get { return 30003; } }

    public enum ServerPort : UInt16 
    {
        FieldPort = 30002,
        LoginPort = 30003,
    }
    public enum State : UInt16
    {
        IDLE,
        MOVE,
        ATTACK,
        ATTACKED,
        DEATH,
        STATE_NONE,
        COOL_TIME,
        PATROL,
        TRACE,
    }
    public enum Dir : UInt16
    {
        NONE = 0,
        UP = 2,
        RIGHT = 4,
        DOWN = 8,
        LEFT = 16,
        UPRIGHT = 6,
        RIGHTDOWN = 12,
        LEFTDOWN = 24,
        LEFTUP = 18,
    }

    public enum MoveType : Int32
    {
        KeyBoard,
        Mouse
    }

    public enum SceneType : Int32 
    {
        Debug,
        Run
    }

    public enum MonsterType : Int32 
    {
        Bear,
        Skeleton,
        Thief
    }

    public enum ServerType : Int32 
    {
        Login,
        Field
    }
}

