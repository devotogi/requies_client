using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
        }
    }

    private void PacketHandler_S2C_PLAYERATTACKED(ArraySegment<byte> dataPtr, int dataSize)
    {
        MemoryStream ms = new MemoryStream(dataPtr.Array, dataPtr.Offset, dataPtr.Count);
        BinaryReader br = new BinaryReader(ms);

        Int32 attackedPlayerId = br.ReadInt32();

        if (Managers.Data.PlayerController.PlayerID == attackedPlayerId)
        {
            Managers.Data.PlayerController.Attacked();
        }
        else
        {
            Managers.Data.PlayerAbleDic.TryGetValue(attackedPlayerId, out var opc);
            opc.Attacked();
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

            Managers.Data.PlayerAbleDic.TryGetValue(playerId, out var opc);
            Managers.Data.PlayerAbleDic.Remove(playerId);
            opc.Destory();
        }
    }

    private void PacketHandler_S2C_PLAYERDESTORY(ArraySegment<byte> dataPtr, int dataSize)
    {
        MemoryStream ms = new MemoryStream(dataPtr.Array, dataPtr.Offset, dataPtr.Count);
        BinaryReader br = new BinaryReader(ms);

        int playerId = br.ReadInt32();
        Managers.Data.PlayerAbleDic.TryGetValue(playerId, out var opc);
        Managers.Data.PlayerAbleDic.Remove(playerId);
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

            GameObject playerGo = Managers.Resource.Instantiate("Player/Knight");
            playerGo.name = $"otherPlayer{playerId}";

            OtherPlayerController opc = playerGo.AddComponent<OtherPlayerController>();
            opc.PlayerID = playerId;

            Managers.Data.PlayerAbleDic.Add(opc.PlayerID, opc);

            GameObject cameraPosGo = Managers.Resource.Instantiate("Camera/FakeCameraPos");
            cameraPosGo.GetComponent<FakeCameraPos>().Init(playerGo);
            opc.Init(quaternion, cameraPosGo.transform.GetChild(0).gameObject);
            opc.UpdateSync(state, dir, mouseDir, nowPos, quaternion);
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
            Managers.Data.PlayerAbleDic.TryGetValue(playerId, out var opc);
            Managers.Data.PlayerAbleDic.Remove(playerId);
            opc.Destory();
        }
        catch (Exception e) {
            Debug.LogException(e);
        }
    }

    private void PacketHandler_S2C_PLAYERENTER(ArraySegment<byte> dataPtr, int dataSize)
    {
        MemoryStream ms = new MemoryStream(dataPtr.Array, dataPtr.Offset, dataPtr.Count); // 
        BinaryReader br = new BinaryReader(ms);

        int playerId = br.ReadInt32();
        float x = br.ReadSingle();
        float y = br.ReadSingle();
        float z = br.ReadSingle();
        Vector3 startPos = new Vector3(x, y, z);
        Type.State state = (Type.State)br.ReadUInt16();
        Type.Dir dir = (Type.Dir)br.ReadUInt16();
        Type.Dir mouseDir = (Type.Dir)br.ReadUInt16();
        float qx = br.ReadSingle();
        float qy = br.ReadSingle();
        float qz = br.ReadSingle();
        float qw = br.ReadSingle();
        Quaternion cameraLocalRotation = new Quaternion(qx,qy,qz,qw);

        GameObject playerGo = Managers.Resource.Instantiate("Player/Knight");
        // TODO 다른 플레이어
        playerGo.name = $"otherPlayer{playerId}";

        OtherPlayerController opc = playerGo.AddComponent<OtherPlayerController>();
        opc.PlayerID = playerId;
        GameObject cameraPosGo = Managers.Resource.Instantiate("Camera/FakeCameraPos");

        cameraPosGo.GetComponent<FakeCameraPos>().Init(playerGo);

        Managers.Data.PlayerAbleDic.Add(opc.PlayerID, opc);
        opc.Init(cameraLocalRotation, cameraPosGo.transform.GetChild(0).gameObject);
        opc.UpdateSync(state, dir, mouseDir, startPos, cameraLocalRotation);
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

            if (playerId == Managers.Data.PlayerController.PlayerID)
                continue;

            GameObject playerGo = Managers.Resource.Instantiate("Player/Knight");
            // TODO 다른 플레이어
            playerGo.name = $"otherPlayer{playerId}";
            
            OtherPlayerController opc = playerGo.AddComponent<OtherPlayerController>();
            opc.PlayerID = playerId;
            try
            {
                Managers.Data.PlayerAbleDic.Add(opc.PlayerID, opc);
            }
            catch (Exception e) {
                Debug.LogException(e);
            }

            GameObject cameraPosGo = Managers.Resource.Instantiate("Camera/FakeCameraPos");

            cameraPosGo.GetComponent<FakeCameraPos>().Init(playerGo);
            opc.Init(cameraLocalRotation, cameraPosGo.transform.GetChild(0).gameObject);

            opc.UpdateSync(state, dir, mouseDir,startPos, cameraLocalRotation);
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

            Managers.Data.PlayerAbleDic.TryGetValue(playerId, out var opc);
            if (opc != null)
                opc.UpdateSync(state, dir, mouseDir, nowPos, quaternion);
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
        GameObject playerGo = Managers.Resource.Instantiate("Player/Knight");
        playerGo.name = "player";
        playerGo.tag = "player";
        PlayerController pc = playerGo.AddComponent<PlayerController>();
        pc.transform.position = new Vector3(x, y, z);
        pc.PlayerID = playerId;
       
        Managers.Data.PlayerController = pc;
        Managers.Data.PlayerAbleDic.Add(pc.PlayerID, pc);

        GameObject cameraPosGo = Managers.Resource.Instantiate("Camera/CameraPos");
        cameraPosGo.GetComponent<CameraPos>().Init(playerGo);
        cameraPosGo.transform.GetChild(0).gameObject.AddComponent<CameraController>().Init(playerGo);
        pc.Init(quaternion, cameraPosGo.transform.GetChild(0).gameObject);
    }
}
