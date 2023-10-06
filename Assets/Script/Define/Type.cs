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
    }
    public static string IP { get { return "58.236.130.58"; } }
    public static int PORT { get { return 30002; } }

    public enum State : UInt16
    {
        IDLE,
        MOVE,
        ATTACK,
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
}

