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
    GameObject _chatContent;
    List<string> chat = new List<string>();
    int _maxSize = 50;
    void Start()
    {
        _text = GetComponent<TMP_InputField>();
        _network = GameObject.Find("Network").GetComponent<Network>();
        _chatContent = GameObject.FindGameObjectWithTag("ChatContent"); 
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return)) 
        {
            byte[] bytes = new byte[1000];
            MemoryStream ms = new MemoryStream(bytes);
            ms.Position = 0;

            string chatting = _text.text;
            byte[] chattingBytes = Encoding.Unicode.GetBytes(chatting.Trim());
            int msgSize = chattingBytes.Length;
            int pktSize = msgSize + 8;

            BinaryWriter bw = new BinaryWriter(ms);
            bw.Write((Int16)Type.PacketProtocol.C2S_PLAYERCHAT);
            bw.Write((Int16)pktSize);
            bw.Write((Int32)msgSize);
            bw.Write(chattingBytes);
            _network.SendPacket(bytes, pktSize);
            _text.text = "";
        }

        for (int i = 0; i < _chatContent.transform.childCount; i++)
            Destroy(_chatContent.transform.GetChild(i).gameObject);

        foreach (string msg in chat)
        {
            GameObject textGo = Managers.Resource.Instantiate("UI/Chatting", _chatContent.transform);
            textGo.GetComponent<TMP_Text>().text = msg;
        }
    }

    internal void Push(string msg)
    {
        if (chat.Count >= _maxSize)
            chat.RemoveAt(chat.Count - 1);

        chat.Insert(0, msg);
    }
}
