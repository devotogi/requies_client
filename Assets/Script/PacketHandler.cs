using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

public class PacketHandler
{
    Dictionary<int, CreatureController> creatureDic = new Dictionary<int, CreatureController>();
    PlayerController playerController = null;
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

            if (playerId == playerController.PlayerID)
                continue;

            creatureDic.TryGetValue(playerId, out var opc);
            creatureDic.Remove(playerId);
            opc.Destory();
        }
    }

    private void PacketHandler_S2C_PLAYERDESTORY(ArraySegment<byte> dataPtr, int dataSize)
    {
        MemoryStream ms = new MemoryStream(dataPtr.Array, dataPtr.Offset, dataPtr.Count);
        BinaryReader br = new BinaryReader(ms);

        int playerId = br.ReadInt32();
        creatureDic.TryGetValue(playerId, out var opc);
        creatureDic.Remove(playerId);
        opc.Destory();
    }

    private void PacketHandler_S2C_PLAYERNEW(ArraySegment<byte> dataPtr, int dataSize)
    {
        MemoryStream ms = new MemoryStream(dataPtr.Array, dataPtr.Offset, dataPtr.Count);
        BinaryReader br = new BinaryReader(ms);

        Int32 playerId = br.ReadInt32();

        if (playerId == playerController.PlayerID)
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

            GameObject prefab = Resources.Load<GameObject>($"Prefabs/Player");
            GameObject playerGo = Object.Instantiate(prefab);
            playerGo.name = $"otherPlayer{playerId}";

            OtherPlayerController opc = playerGo.AddComponent<OtherPlayerController>();
            opc.PlayerID = playerId;

            creatureDic.Add(opc.PlayerID, opc);

            GameObject cameraPrefab = Resources.Load<GameObject>($"Prefabs/FakeCameraPos");
            GameObject cameraPosGo = Object.Instantiate(cameraPrefab);
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
            creatureDic.TryGetValue(playerId, out var opc);
            creatureDic.Remove(playerId);
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

        GameObject prefab = Resources.Load<GameObject>($"Prefabs/Player");
        GameObject playerGo = Object.Instantiate(prefab);
        // TODO 다른 플레이어
        playerGo.name = $"otherPlayer{playerId}";

        OtherPlayerController opc = playerGo.AddComponent<OtherPlayerController>();
        opc.PlayerID = playerId;

        GameObject cameraPrefab = Resources.Load<GameObject>($"Prefabs/FakeCameraPos");
        GameObject cameraPosGo = Object.Instantiate(cameraPrefab);
        cameraPosGo.GetComponent<FakeCameraPos>().Init(playerGo);

        creatureDic.Add(opc.PlayerID, opc);
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

            if (playerId == playerController.PlayerID)
                continue;

            GameObject prefab = Resources.Load<GameObject>($"Prefabs/Player");
            GameObject playerGo = Object.Instantiate(prefab);
            // TODO 다른 플레이어
            playerGo.name = $"otherPlayer{playerId}";
            
            OtherPlayerController opc = playerGo.AddComponent<OtherPlayerController>();
            opc.PlayerID = playerId;
            try
            {
                creatureDic.Add(opc.PlayerID, opc);
            }
            catch (Exception e) {
                Debug.LogException(e);
            }

            GameObject cameraPrefab = Resources.Load<GameObject>($"Prefabs/FakeCameraPos");
            GameObject cameraPosGo = Object.Instantiate(cameraPrefab);
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

        if (playerId == playerController.PlayerID)
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

            creatureDic.TryGetValue(playerId, out var opc);
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

        GameObject prefab = Resources.Load<GameObject>($"Prefabs/Player");
        GameObject playerGo = Object.Instantiate(prefab);

        playerGo.name = "player";
        playerGo.tag = "player";
        PlayerController pc = playerGo.AddComponent<PlayerController>();
        pc.transform.position = new Vector3(x, y, z);
        pc.PlayerID = playerId;
       
        playerController = pc;
        creatureDic.Add(pc.PlayerID, pc);

        GameObject cameraPrefab = Resources.Load<GameObject>($"Prefabs/CameraPos");
        GameObject cameraPosGo = Object.Instantiate(cameraPrefab);

        cameraPosGo.GetComponent<CameraPos>().Init(playerGo);
        cameraPosGo.transform.GetChild(0).gameObject.AddComponent<CameraController>().Init(playerGo);
        pc.Init(quaternion, cameraPosGo.transform.GetChild(0).gameObject);
    }
}
