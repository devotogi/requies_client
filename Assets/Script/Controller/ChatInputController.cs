using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine;


public class ChatInputController : MonoBehaviour
{
    struct Chat 
    {
        public int chatType;
        public string msg;
    }

    TMP_InputField _text;
    Network _network;
    GameObject _chatContent;
    List<Chat> chat = new List<Chat>();
    TMP_InputField _input;
    int _maxSize = 50;
    TMP_Dropdown dropdown;
    void Start()
    {
        _text = GetComponent<TMP_InputField>();
        _network = GameObject.Find("Network").GetComponent<Network>();
        _chatContent = GameObject.FindGameObjectWithTag("ChatContent");
        _input = GetComponent<TMP_InputField>();
        dropdown = transform.parent.transform.GetChild(3).GetComponent<TMP_Dropdown>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            _input.ActivateInputField();
            int selectedIndex = dropdown.value;
            string chatting = _text.text;

            if (chatting.Trim().Length == 0)
                return;

            byte[] bytes = new byte[1000];
            MemoryStream ms = new MemoryStream(bytes);
            ms.Position = 0;

            byte[] chattingBytes = Encoding.Unicode.GetBytes(chatting.Trim());
            int msgSize = chattingBytes.Length;
            int pktSize = msgSize + 8 + 4;

            BinaryWriter bw = new BinaryWriter(ms);
            bw.Write((Int16)Type.PacketProtocol.C2S_PLAYERCHAT);
            bw.Write((Int16)pktSize);
            bw.Write((Int32)selectedIndex);
            bw.Write((Int32)msgSize);
            bw.Write(chattingBytes);
            _network.SendPacket(bytes, pktSize, Type.ServerType.Field);
            _text.text = "";
        }
    }
}
