using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine;

public class ChatInputController : MonoBehaviour
{
    TMP_InputField _text;
    Network _network;

    void Start()
    {
        _text = GetComponent<TMP_InputField>();
        _network = GameObject.Find("Network").GetComponent<Network>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return)) 
        {
            string chatting = _text.text;
            byte[] chattingBytes = Encoding.Unicode.GetBytes(chatting);
            int msgSize = chattingBytes.Length;

            byte[] bytes = new byte[1000];
            MemoryStream ms = new MemoryStream(bytes);
            ms.Position = 0;

            BinaryWriter bw = new BinaryWriter(ms);
            bw.Write((Int16)Type.PacketProtocol.C2S_PLAYERCHAT);
            bw.Write((Int16)4 + msgSize + 4);
            bw.Write((Int32)msgSize);
            bw.Write(chattingBytes);

            _network.SendPacket(bytes, 4 + msgSize + 4);
        }              
    }
}
