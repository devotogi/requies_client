using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class StatInfoController : MonoBehaviour
{
    private int _statPoint;
    private float _damage;
    private float _speed;
    private float _defense;
    private TMP_Text _damageText;
    private TMP_Text _speedText;
    private TMP_Text _defenseText;
    private TMP_Text _statPointText;
    private Network _network;
    public void Init() 
    {
        _network = FindObjectOfType<Network>();
        _damageText = transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>();
        _speedText = transform.GetChild(1).GetChild(1).GetComponent<TMP_Text>();
        _defenseText = transform.GetChild(2).GetChild(1).GetComponent<TMP_Text>();
        _statPointText = transform.GetChild(4).GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpDamage() 
    {
        SendUpStat(0);
    }
    public void UpSpeed() 
    {
        SendUpStat(1);
    }
    public void UpDefense() 
    {
        SendUpStat(2);
    }

    private void SendUpStat(int type) 
    {
        byte[] bytes = new byte[30];
        MemoryStream ms = new MemoryStream(bytes);
        ms.Position = 0;

        BinaryWriter bw = new BinaryWriter(ms);
        bw.Write((Int16)Type.PacketProtocol.C2S_UPSTAT);
        bw.Write((Int16)8);
        bw.Write((Int32)type); // 0 Attack, 1 Speed, 2 Defense
        _network.SendPacket(bytes, 8);
    }

    void RefreshRender() 
    {
        _damageText.text = $"공격력:{_damage}";
        _speedText.text = $"이동속도:{_speed}";
        _defenseText.text = $"방어력:{_defense}";
        _statPointText.text = $"SP:{_statPoint}";
    }

    internal void Off()
    {
        gameObject.SetActive(false);
    }

    internal void On(float damage, float speed, float defense, int statPoint)
    {
        _damage = damage;
        _speed = speed; 
        _defense = defense; 
        _statPoint = statPoint;

        if (gameObject.active == false)
            gameObject.SetActive(true);

        RefreshRender();
    }
}
