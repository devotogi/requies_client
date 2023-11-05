using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

public class PacketHandler
{
    internal void Handler(ArraySegment<byte> segment)
    {
        ArraySegment<byte> pktCodeByte = new ArraySegment<byte>(segment.Array, 0, sizeof(Int16)); // 2byte 타입 추출
        ArraySegment<byte> pktSizeByte = new ArraySegment<byte>(segment.Array, sizeof(Int16), sizeof(Int16)); // 2byte 사이즈 추출

        Type.PacketProtocol pktCode = (Type.PacketProtocol)BitConverter.ToInt16(pktCodeByte);
        Int16 pktSize = BitConverter.ToInt16(pktSizeByte);
        int dataSize = pktSize - sizeof(Int32);
        ArraySegment<byte> dataPtr = new ArraySegment<byte>(segment.Array, sizeof(Int32), dataSize);

        switch (pktCode)
        {
            case Type.PacketProtocol.S2C_PLAYERINIT:
                PacketHandler_SC2_PLAYERINIT(dataPtr, dataSize);
                break;

            case Type.PacketProtocol.S2C_PLAYERSYNC:
                PacketHandler_S2C_PLAYERSYNC(dataPtr, dataSize);
                break;

            case Type.PacketProtocol.S2C_PLAYEROUT:
                PacketHandler_S2C_PLAYEROUT(dataPtr, dataSize);
                break;

            case Type.PacketProtocol.S2C_PLAYERNEW:
                PacketHandler_S2C_PLAYERNEW(dataPtr, dataSize);
                break;

            case Type.PacketProtocol.S2C_PLAYERDESTORY:
                PacketHandler_S2C_PLAYERDESTORY(dataPtr, dataSize);
                break;

            case Type.PacketProtocol.S2C_PLAYERLIST:
                PacketHandler_S2C_PLAYERLIST(dataPtr, dataSize);
                break;

            case Type.PacketProtocol.S2C_PLAYERREMOVELIST:
                PacketHandler_PLAYERREMOVELIST(dataPtr, dataSize);
                break;

            case Type.PacketProtocol.S2C_PLAYERATTACKED:
                PacketHandler_S2C_PLAYERATTACKED(dataPtr, dataSize);
                break;

            case Type.PacketProtocol.S2C_PLAYERCHAT:
                PacketHandler_S2C_PLAYERCHAT(dataPtr, dataSize);
                break;

            case Type.PacketProtocol.S2C_PLAYERDETH:
                PacketHandler_S2C_PLAYERDETH(dataPtr, dataSize);
                break;

            case Type.PacketProtocol.S2C_PLAYERESPAWN:
                PacketHandler_S2C_PLAYERESPAWN(dataPtr, dataSize);
                break;

            case Type.PacketProtocol.S2C_MONSTERSPAWN:
                PacketHandler_S2C_MONSTERSPAWN(dataPtr, dataSize);
                break;

            case Type.PacketProtocol.S2C_MONSTERREMOVELIST:
                PacketHandler_S2C_MONSTERREMOVELIST(dataPtr, dataSize);
                break;

            case Type.PacketProtocol.S2C_MONSTERRENEWLIST:
                PacketHandler_S2C_MONSTERRENEWLIST(dataPtr, dataSize);
                break;
            
            case Type.PacketProtocol.S2C_MONSTERATTACKED:
                PacketHandler_S2C_MONSTERATTACKED(dataPtr, dataSize);
                break;

            case Type.PacketProtocol.S2C_MONSTERDEAD:
                PacketHandler_S2C_MONSTERDEAD(dataPtr, dataSize);
                break;

            case Type.PacketProtocol.S2C_MONSTERSYNC:
                PacketHandler_S2C_MONSTERSYNC(dataPtr, dataSize);
                break;

            case Type.PacketProtocol.S2C_NEWMONSTER:
                PacketHandler_S2C_NEWMONSTER(dataPtr, dataSize);
                break;

            case Type.PacketProtocol.S2C_DELETEMONSTER:
                PacketHandler_S2C_DELETEMONSTER(dataPtr, dataSize);
                break;

        }
    }

    private void PacketHandler_S2C_DELETEMONSTER(ArraySegment<byte> dataPtr, int dataSize)
    {
        MemoryStream ms = new MemoryStream(dataPtr.Array, dataPtr.Offset, dataPtr.Count);
        BinaryReader br = new BinaryReader(ms);

        Type.State monsterState = (Type.State)br.ReadInt16();
        int monsterId = br.ReadInt32();
        int monsterType = br.ReadInt32();
        float x = br.ReadSingle();
        float y = br.ReadSingle();
        float z = br.ReadSingle();
        float hp = br.ReadSingle();
        float lx = br.ReadSingle();
        float ly = br.ReadSingle();
        float lz = br.ReadSingle();
        Vector3 pos = new Vector3(x, y, z);
        Vector3 look = new Vector3(lx, ly, lz);

        Managers.Data.MonsterDic.TryGetValue(monsterId, out var monster);
        Managers.Resource.Destory(monster.gameObject);
        Managers.Data.MonsterDic.Remove(monsterId);
    }

    private void PacketHandler_S2C_NEWMONSTER(ArraySegment<byte> dataPtr, int dataSize)
    {
        MemoryStream ms = new MemoryStream(dataPtr.Array, dataPtr.Offset, dataPtr.Count);
        BinaryReader br = new BinaryReader(ms);

        Type.State monsterState = (Type.State)br.ReadInt16();
        int monsterId = br.ReadInt32();
        int monsterType = br.ReadInt32();
        float x = br.ReadSingle();
        float y = br.ReadSingle();
        float z = br.ReadSingle();
        float hp = br.ReadSingle();
        float lx = br.ReadSingle();
        float ly = br.ReadSingle();
        float lz = br.ReadSingle();
        Vector3 pos = new Vector3(x, y, z);
        Vector3 look = new Vector3(lx, ly, lz);

        GameObject bear = Managers.Resource.Instantiate("Monster/Bear");
        MonsterController mc = bear.AddComponent<MonsterController>();
        GameObject hpObject = Managers.Resource.Instantiate("UI/HP");
        HpController hpc = hpObject.GetComponent<HpController>();
        mc.HPC = hpc;

        mc.SetHp(hp);
        mc.MonsterId = monsterId;
        mc.MonsterType = (Type.MonsterType)monsterType;
        bear.transform.position = pos;

        Managers.Data.MonsterDic.Add(monsterId, bear);
        mc.GetComponent<MonsterController>().Sync(monsterState, pos, hp, look);
    }

    private void PacketHandler_S2C_MONSTERSYNC(ArraySegment<byte> dataPtr, int dataSize)
    {
        MemoryStream ms = new MemoryStream(dataPtr.Array, dataPtr.Offset, dataPtr.Count);
        BinaryReader br = new BinaryReader(ms);
        int monsterId = 0;
        try
        {
            Type.State monsterState = (Type.State)br.ReadInt16();
            monsterId = br.ReadInt32();
            float x = br.ReadSingle();
            float y = br.ReadSingle();
            float z = br.ReadSingle();
            float hp = br.ReadSingle();
            float lx = br.ReadSingle();
            float ly = br.ReadSingle();
            float lz = br.ReadSingle();
            Vector3 pos = new Vector3(x, y, z);
            Vector3 look = new Vector3(lx, ly, lz);
            Managers.Data.MonsterDic.TryGetValue(monsterId, out var monster);
            monster.GetComponent<MonsterController>().Sync(monsterState, pos, hp, look);
            monster.transform.eulerAngles = look;
        }
        catch (Exception e)
        {
            Debug.Log(monsterId);
        }
    }

    private void PacketHandler_S2C_MONSTERDEAD(ArraySegment<byte> dataPtr, int dataSize)
    {
        MemoryStream ms = new MemoryStream(dataPtr.Array, dataPtr.Offset, dataPtr.Count);
        BinaryReader br = new BinaryReader(ms);

        int monsterId = br.ReadInt32();

        Managers.Data.MonsterDic.TryGetValue(monsterId, out var monster);
        Managers.Resource.Destory(monster.gameObject);
        Managers.Data.MonsterDic.Remove(monsterId);
    }

    private void PacketHandler_S2C_MONSTERATTACKED(ArraySegment<byte> dataPtr, int dataSize)
    {
        MemoryStream ms = new MemoryStream(dataPtr.Array, dataPtr.Offset, dataPtr.Count);
        BinaryReader br = new BinaryReader(ms);

        Type.State monsterState = (Type.State)br.ReadInt16();
        Type.MonsterType type = (Type.MonsterType)br.ReadInt32();
        int monsterId = br.ReadInt32();
        float x = br.ReadSingle();
        float y = br.ReadSingle();
        float z = br.ReadSingle();
        Vector3 pos = new Vector3(x, y, z);
        float hp = br.ReadSingle();
        float lx = br.ReadSingle();
        float ly = br.ReadSingle();
        float lz = br.ReadSingle();
        Vector3 look = new Vector3(lx, ly, lz);

        Managers.Data.MonsterDic.TryGetValue(monsterId, out var monster);
        MonsterController mc = monster.GetComponent<MonsterController>();
        mc.Sync(monsterState, pos, hp, look);   
    }

    private void PacketHandler_S2C_MONSTERRENEWLIST(ArraySegment<byte> dataPtr, int dataSize)
    {
        MemoryStream ms = new MemoryStream(dataPtr.Array, dataPtr.Offset, dataPtr.Count);
        BinaryReader br = new BinaryReader(ms);

        Int32 cnt = br.ReadInt32();

        for (int i = 0; i < cnt; i++)
        {
            Int32 monsterId = br.ReadInt32();
            Int32 monsterType = br.ReadInt32();
            float x = br.ReadSingle();
            float y = br.ReadSingle();
            float z = br.ReadSingle();
            float hp = br.ReadSingle();

            GameObject bear = Managers.Resource.Instantiate("Monster/Bear");
            MonsterController mc = bear.AddComponent<MonsterController>();
            GameObject hpObject = Managers.Resource.Instantiate("UI/HP");
            HpController hpc = hpObject.GetComponent<HpController>();
            mc.HPC = hpc;

            mc.SetHp(hp);
            mc.MonsterId = monsterId;
            mc.MonsterType = (Type.MonsterType)monsterType;
            bear.transform.position = new Vector3(x, y, z);
            Managers.Data.MonsterDic.Add(monsterId, bear);
        }
    }

    private void PacketHandler_S2C_MONSTERREMOVELIST(ArraySegment<byte> dataPtr, int dataSize)
    {
        MemoryStream ms = new MemoryStream(dataPtr.Array, dataPtr.Offset, dataPtr.Count);
        BinaryReader br = new BinaryReader(ms);

        Int32 cnt = br.ReadInt32();

        for (int i = 0; i < cnt; i++)
        {
            int monsterId = br.ReadInt32();
            Managers.Data.MonsterDic.TryGetValue(monsterId, out var monster);
            Managers.Resource.Destory(monster);
            Managers.Data.MonsterDic.Remove(monsterId);
        }
    }

    private void PacketHandler_S2C_MONSTERSPAWN(ArraySegment<byte> dataPtr, int dataSize)
    {
        MemoryStream ms = new MemoryStream(dataPtr.Array, dataPtr.Offset, dataPtr.Count);
        BinaryReader br = new BinaryReader(ms);

        Int32 monsterId = br.ReadInt32();
        Int32 monsterType = br.ReadInt32();
        float x = br.ReadSingle();
        float y = br.ReadSingle();
        float z = br.ReadSingle();
        float hp = br.ReadSingle();
        GameObject bear = Managers.Resource.Instantiate("Monster/Bear");
        MonsterController mc = bear.AddComponent<MonsterController>();

        GameObject hpObject = Managers.Resource.Instantiate("UI/HP");
        HpController hpc = hpObject.GetComponent<HpController>();
        mc.HPC = hpc;
        mc.MonsterId = monsterId;
        mc.MonsterType = (Type.MonsterType) monsterType;
        mc.SetHp(hp);
        bear.transform.position = new Vector3(x, y, z);

        Managers.Data.MonsterDic.Add(monsterId, bear);
    }

    private void PacketHandler_S2C_PLAYERESPAWN(ArraySegment<byte> dataPtr, int dataSize)
    {
        MemoryStream ms = new MemoryStream(dataPtr.Array, dataPtr.Offset, dataPtr.Count);
        BinaryReader br = new BinaryReader(ms);

        Int32 playerId = br.ReadInt32();
        Type.State state = (Type.State)br.ReadUInt16();
        Type.Dir dir = (Type.Dir)br.ReadUInt16();
        Type.Dir mouseDir = (Type.Dir)br.ReadUInt16();
        float x = br.ReadSingle();
        float y = br.ReadSingle();
        float z = br.ReadSingle();
        Vector3 nowPos = new Vector3(x, y, z);
        float qx = br.ReadSingle();
        float qy = br.ReadSingle();
        float qz = br.ReadSingle();
        float qw = br.ReadSingle();
        Quaternion quaternion = new Quaternion(qx, qy, qz, qw);
        float hp = br.ReadSingle();
        float mp = br.ReadSingle();

        Managers.Data.PlayerController.Respwan(nowPos, state, dir, mouseDir, quaternion, hp, mp);
    }

    private void PacketHandler_S2C_PLAYERDETH(ArraySegment<byte> dataPtr, int dataSize)
    {
        MemoryStream ms = new MemoryStream(dataPtr.Array, dataPtr.Offset, dataPtr.Count);
        BinaryReader br = new BinaryReader(ms);

        int playerId = br.ReadInt32();

        if (Managers.Data.PlayerController.PlayerID == playerId)
        {
            Managers.Data.PlayerController.Death();
        }
        else
        {
            Managers.Data.PlayerDic.TryGetValue(playerId, out var opc);
            opc.Death();
        }
    }

    private void PacketHandler_S2C_PLAYERCHAT(ArraySegment<byte> dataPtr, int dataSize)
    {
        MemoryStream ms = new MemoryStream(dataPtr.Array, dataPtr.Offset, dataPtr.Count);
        BinaryReader br = new BinaryReader(ms);

        int playerId = br.ReadInt32();

        int msgSize = br.ReadInt32();
        byte[] msgBytes = br.ReadBytes(msgSize);
        string msg = Encoding.Unicode.GetString(msgBytes);

        if (Managers.Data.PlayerController.PlayerID == playerId)
        {
            Managers.Data.PlayerController.Talk(msg);
        }
        else
        {
            Managers.Data.PlayerDic.TryGetValue(playerId, out var opc);
            opc.Talk(msg);
        }


        GameObject chatInput = GameObject.FindGameObjectWithTag("ChatInput");
        chatInput.GetComponent<ChatInputController>().Push(msg);
    }

    private void PacketHandler_S2C_PLAYERATTACKED(ArraySegment<byte> dataPtr, int dataSize)
    {
        MemoryStream ms = new MemoryStream(dataPtr.Array, dataPtr.Offset, dataPtr.Count);
        BinaryReader br = new BinaryReader(ms);

        Int32 attackedPlayerId = br.ReadInt32();
        float attackedHp = br.ReadSingle();
        float attackedMp = br.ReadSingle();

        if (Managers.Data.PlayerController.PlayerID == attackedPlayerId)
        {
            Managers.Data.PlayerController.Attacked();
            Managers.Data.PlayerController.SetHp(attackedHp);
            Managers.Data.PlayerController.SetMp(attackedMp);
        }
        else
        {
            Managers.Data.PlayerDic.TryGetValue(attackedPlayerId, out var opc);
            opc.Attacked();
            opc.SetHp(attackedHp);
            opc.SetMp(attackedMp);
        }
    }

    private void PacketHandler_PLAYERREMOVELIST(ArraySegment<byte> dataPtr, int dataSize)
    {
        MemoryStream ms = new MemoryStream(dataPtr.Array, dataPtr.Offset, dataPtr.Count);
        BinaryReader br = new BinaryReader(ms);

        Int32 userCnt = br.ReadInt32();
        for (int i = 0; i < userCnt; i++)
        {
            int playerId = br.ReadInt32();

            if (playerId == Managers.Data.PlayerController.PlayerID)
                continue;

            Managers.Data.PlayerDic.TryGetValue(playerId, out var opc);
            Managers.Data.PlayerDic.Remove(playerId);
            opc.Destory();
        }
    }

    private void PacketHandler_S2C_PLAYERDESTORY(ArraySegment<byte> dataPtr, int dataSize)
    {
        MemoryStream ms = new MemoryStream(dataPtr.Array, dataPtr.Offset, dataPtr.Count);
        BinaryReader br = new BinaryReader(ms);

        int playerId = br.ReadInt32();
        Managers.Data.PlayerDic.TryGetValue(playerId, out var opc);
        Managers.Data.PlayerDic.Remove(playerId);
        opc.Destory();
    }

    private void PacketHandler_S2C_PLAYERNEW(ArraySegment<byte> dataPtr, int dataSize)
    {
        MemoryStream ms = new MemoryStream(dataPtr.Array, dataPtr.Offset, dataPtr.Count);
        BinaryReader br = new BinaryReader(ms);

        Int32 playerId = br.ReadInt32();

        if (playerId == Managers.Data.PlayerController.PlayerID)
        {
            return;
        }

        try
        {
            Type.State state = (Type.State)br.ReadUInt16();
            Type.Dir dir = (Type.Dir)br.ReadUInt16();
            Type.Dir mouseDir = (Type.Dir)br.ReadUInt16();
            float x = br.ReadSingle();
            float y = br.ReadSingle();
            float z = br.ReadSingle();
            Vector3 nowPos = new Vector3(x, y, z);
            float qx = br.ReadSingle();
            float qy = br.ReadSingle();
            float qz = br.ReadSingle();
            float qw = br.ReadSingle();
            Quaternion quaternion = new Quaternion(qx, qy, qz, qw);
            float tx = br.ReadSingle();
            float ty = br.ReadSingle();
            float tz = br.ReadSingle();
            Vector3 target = new Vector3(tx, ty, tz);
            Type.MoveType moveType = (Type.MoveType)br.ReadInt32();
            float lx = br.ReadSingle();
            float ly = br.ReadSingle();
            float lz = br.ReadSingle();
            float lw = br.ReadSingle();
            Quaternion localRotation = new Quaternion(lx, ly, lz, lw);
            float hp = br.ReadSingle();
            float mp = br.ReadSingle();

            GameObject playerGo = Managers.Resource.Instantiate("Player/Knight");
            playerGo.name = $"otherPlayer{playerId}";

            OtherPlayerController opc = playerGo.AddComponent<OtherPlayerController>();
            opc.PlayerID = playerId;

            Managers.Data.PlayerDic.Add(opc.PlayerID, opc);

            GameObject cameraPosGo = Managers.Resource.Instantiate("Camera/FakeCameraPos");
            cameraPosGo.GetComponent<FakeCameraPos>().Init(playerGo);
            GameObject hpObject = Managers.Resource.Instantiate("UI/HP");
            HpController hpc = hpObject.GetComponent<HpController>();

            opc.Init(quaternion, cameraPosGo.transform.GetChild(0).gameObject, hpc);
            opc.UpdateSync(moveType, state, dir, mouseDir, nowPos, quaternion, target, localRotation);
            opc.SetHp(hp);
            opc.SetMp(mp);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            Debug.Log(playerId);
        }
    }

    private void PacketHandler_S2C_PLAYEROUT(ArraySegment<byte> dataPtr, int dataSize)
    {
        MemoryStream ms = new MemoryStream(dataPtr.Array, dataPtr.Offset, dataPtr.Count);
        BinaryReader br = new BinaryReader(ms);
        int playerId = br.ReadInt32();
        try
        {
            Managers.Data.PlayerDic.TryGetValue(playerId, out var opc);
            Managers.Data.PlayerDic.Remove(playerId);
            opc.Destory();
        }
        catch (Exception e) {
            Debug.LogException(e);
        }
    }

    private void PacketHandler_S2C_PLAYERLIST(ArraySegment<byte> dataPtr, int dataSize)
    {
        MemoryStream ms = new MemoryStream(dataPtr.Array, dataPtr.Offset, dataPtr.Count);
        BinaryReader br = new BinaryReader(ms);

        Int32 userCnt = br.ReadInt32();
        for (int i = 0; i < userCnt; i++) 
        {
            int playerId = br.ReadInt32();
            Type.State state = (Type.State)br.ReadUInt16();
            Type.Dir dir = (Type.Dir)br.ReadUInt16();
            Type.Dir mouseDir = (Type.Dir)br.ReadUInt16();
            float x = br.ReadSingle();
            float y = br.ReadSingle();
            float z = br.ReadSingle();
            Vector3 startPos = new Vector3(x,y,z);
            float qx = br.ReadSingle();
            float qy = br.ReadSingle();
            float qz = br.ReadSingle();
            float qw = br.ReadSingle();
            Quaternion cameraLocalRotation = new Quaternion(qx, qy, qz, qw);
            float tx = br.ReadSingle();
            float ty = br.ReadSingle();
            float tz = br.ReadSingle();
            Vector3 target = new Vector3(tx, ty, tz);
            Type.MoveType moveType = (Type.MoveType)br.ReadInt32();
            float lx = br.ReadSingle();
            float ly = br.ReadSingle();
            float lz = br.ReadSingle();
            float lw = br.ReadSingle();
            Quaternion localRotation = new Quaternion(lx, ly, lz, lw);
            float hp = br.ReadSingle();
            float mp = br.ReadSingle();

            if (playerId == Managers.Data.PlayerController.PlayerID)
                continue;

            GameObject playerGo = Managers.Resource.Instantiate("Player/Knight");
            // TODO 다른 플레이어
            playerGo.name = $"otherPlayer{playerId}";
            
            OtherPlayerController opc = playerGo.AddComponent<OtherPlayerController>();
            opc.PlayerID = playerId;
            try
            {
                Managers.Data.PlayerDic.Add(opc.PlayerID, opc);
            }
            catch (Exception e) {
                Debug.LogException(e);
            }

            GameObject hpObject = Managers.Resource.Instantiate("UI/HP");

            GameObject cameraPosGo = Managers.Resource.Instantiate("Camera/FakeCameraPos");
            cameraPosGo.GetComponent<FakeCameraPos>().Init(playerGo);

            HpController hpc = hpObject.GetComponent<HpController>();

            opc.Init(cameraLocalRotation, cameraPosGo.transform.GetChild(0).gameObject, hpc);
            opc.UpdateSync(moveType, state, dir, mouseDir,startPos, cameraLocalRotation, target, localRotation);
            opc.SetHp(hp);
            opc.SetMp(mp);
        }
    }

    private void PacketHandler_S2C_PLAYERSYNC(ArraySegment<byte> dataPtr, int dataSize)
    {

        MemoryStream ms = new MemoryStream(dataPtr.Array, dataPtr.Offset, dataPtr.Count);
        BinaryReader br = new BinaryReader(ms);

        Int32 playerId = br.ReadInt32();

        if (playerId == Managers.Data.PlayerController.PlayerID)
        {
            return;
        }

        try
        {
            Type.State state = (Type.State)br.ReadUInt16();
            Type.Dir dir = (Type.Dir)br.ReadUInt16();
            Type.Dir mouseDir = (Type.Dir)br.ReadUInt16();
            float x = br.ReadSingle();
            float y = br.ReadSingle();
            float z = br.ReadSingle();
            Vector3 nowPos = new Vector3(x, y, z);
            float qx = br.ReadSingle();
            float qy = br.ReadSingle();
            float qz = br.ReadSingle();
            float qw = br.ReadSingle();
            Quaternion quaternion = new Quaternion(qx, qy, qz, qw);
            float tx = br.ReadSingle();
            float ty = br.ReadSingle();
            float tz = br.ReadSingle();
            Vector3 target = new Vector3(tx, ty, tz);
            Type.MoveType moveType = (Type.MoveType)br.ReadInt32();
            float lx = br.ReadSingle();
            float ly = br.ReadSingle();
            float lz = br.ReadSingle();
            float lw = br.ReadSingle();
            Quaternion localRotation = new Quaternion(lx, ly, lz, lw);

            Managers.Data.PlayerDic.TryGetValue(playerId, out var opc);
            if (opc != null)
                opc.UpdateSync(moveType, state, dir, mouseDir, nowPos, quaternion,target, localRotation);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            Debug.Log(playerId);
        }
    }

    private void PacketHandler_SC2_PLAYERINIT(ArraySegment<byte> dataPtr, int dataSize)
    {
        MemoryStream ms = new MemoryStream(dataPtr.Array, dataPtr.Offset, dataPtr.Count);
        BinaryReader br = new BinaryReader(ms);

        Int32 playerId = br.ReadInt32();
        Type.State state = (Type.State)br.ReadUInt16();
        Type.Dir dir = (Type.Dir)br.ReadUInt16();
        Type.Dir mouseDir = (Type.Dir)br.ReadUInt16();
        float x = br.ReadSingle();
        float y = br.ReadSingle();
        float z = br.ReadSingle();
        Vector3 nowPos = new Vector3(x, y, z);
        float qx = br.ReadSingle();
        float qy = br.ReadSingle();
        float qz = br.ReadSingle();
        float qw = br.ReadSingle(); 
        Quaternion quaternion = new Quaternion(qx, qy, qz, qw);
        float hp = br.ReadSingle();
        float mp = br.ReadSingle();

        GameObject playerGo = Managers.Resource.Instantiate("Player/Knight");
        playerGo.name = "player";
        PlayerController pc = playerGo.AddComponent<PlayerController>();
        pc.SetAgentPos(new Vector3(x, y, z));
        pc.PlayerID = playerId;
        pc.SetMp(mp);
        pc.SetHp(hp);
        Managers.Data.PlayerController = pc;
        Managers.Data.PlayerDic.Add(pc.PlayerID, pc);

        GameObject cameraPosGo = Managers.Resource.Instantiate("Camera/CameraPos");
        cameraPosGo.GetComponent<CameraPos>().Init(playerGo);
        cameraPosGo.transform.GetChild(0).gameObject.AddComponent<CameraController>().Init(playerGo);

        pc.Init(quaternion, cameraPosGo.transform.GetChild(0).gameObject);
    }
}
