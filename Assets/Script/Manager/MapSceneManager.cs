using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MapSceneManager : MonoBehaviour
{
    Network _network;

    void Awake()
    {
        GameObject playerUi = Managers.Resource.Instantiate("UI/PlayerUI");

        // 플레이어 Init 패킷 보내기
        _network = GameObject.Find("Network").GetComponent<Network>();

        byte[] bytes = new byte[1000];
        MemoryStream ms = new MemoryStream(bytes);
        ms.Position = 0;

        Int32 SQ = Managers.Data.userSQ;

        BinaryWriter bw = new BinaryWriter(ms);
        bw.Write((Int16)Type.PacketProtocol.C2S_PLAYERINIT);
        bw.Write((Int16)8);
        bw.Write((Int32)SQ);
        _network.SendPacket(bytes, 8, Type.ServerPort.FieldPort);
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
