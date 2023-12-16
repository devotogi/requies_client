using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine;

public class LoginController : MonoBehaviour
{
    TMP_InputField _userId;
    TMP_InputField _password;
    Network _network;
    GameObject _alert;
    TMP_Text _alertMsg;

    void Start()
    {
        _network = FindObjectOfType<Network>();

        GameObject backGround = transform.GetChild(0).gameObject;
        _alert = transform.GetChild(1).gameObject;
        _alertMsg = transform.GetChild(1).GetChild(1).GetComponent<TMP_Text>();
        GameObject panel = backGround.transform.GetChild(0).gameObject;
        _userId = panel.transform.GetChild(0).GetComponent<TMP_InputField>();
        _password = panel.transform.GetChild(1).GetComponent<TMP_InputField>();
    }

    public void Login()
    {
        Debug.Log($"UserId:{_userId.text}, UserPw:{_password.text}");

        if (_userId.text.Trim() == "" || _password.text.Trim() == "") 
        {
            SetAlert(true);
            SetAlertMsg("아이디 또는 패스워드를 입력해주십시오.");
            return;
        }


        byte[] bytes = new byte[1000];
        MemoryStream ms = new MemoryStream(bytes);
        ms.Position = 0;

        byte[] userIdBytes = Encoding.Unicode.GetBytes(_userId.text.Trim());
        int userIdLen = userIdBytes.Length;

        byte[] userPwBytes = Encoding.Unicode.GetBytes(_password.text.Trim());
        int userPwLen = userPwBytes.Length;

        int pktSize = userIdLen + userPwLen + 8 + 4;

        BinaryWriter bw = new BinaryWriter(ms);
        bw.Write((Int16)Type.PacketProtocol.C2S_LOGIN);
        bw.Write((Int16)pktSize);
        bw.Write((Int32)userIdLen);
        bw.Write(userIdBytes);
        bw.Write((Int32)userPwLen);
        bw.Write(userPwBytes);

        _network.SendPacket(bytes, pktSize, Type.ServerPort.LoginPort);
        
        _userId.text = "";
        _password.text = "";
    }

    public void SetAlert(bool flag) 
    {
        _alert.SetActive(flag);
    }

    public void SetAlertMsg(string msg) 
    {
        _alertMsg.text = msg;
    }

    public void CloseAlert() 
    {
        SetAlert(false);
        SetAlertMsg("");
    }
}
