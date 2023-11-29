using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MonsterInfoController : MonoBehaviour
{
    private Image _hpImg;
    private TMP_Text _levelText;
    private TMP_Text _labelText;

    private int _monsterId;
    private float _hp = 1000f;
    private float _hpMax = 1000f;
    private int _level = 0;

    void Update()
    {
        
    }

    public void Init() 
    {
        _labelText = transform.GetChild(0).GetComponent<TMP_Text>();
        _levelText = transform.GetChild(1).GetComponent<TMP_Text>();
        Transform hpBar = transform.GetChild(2);
        _hpImg = hpBar.GetChild(0).GetComponent<Image>();
    }

    internal void SetMonsterInfo(int monsterId, int monsterType, float monsterHp)
    {
        _monsterId = monsterId;
        gameObject.SetActive(true);
        switch (monsterType)
        {
            case 0:
                _hpMax = 2000f;
                _level = 10;
                _labelText.text = "°õ";
                break;
                
            case 1:
                _hpMax = 1000f;
                _level = 20;
                _labelText.text = "ÇØ°ñº´»ç";
                break;

            case 2:
                _hpMax = 500f;
                _level = 30;
                _labelText.text = "µµÀû";
                break;
        }

        _levelText.text = $"Lv.{_level}";
        SetHP(monsterHp);
    }

    void SetHP(float hp) 
    {
        _hp = hp;
        _hpImg.fillAmount = _hp / _hpMax;
    }

    internal void MonsterInfoIsUnActive(int monsterId)
    {
        if (_monsterId != monsterId) return;
        gameObject.SetActive(false);
    }

    internal void MonsterInfoIsAttacked(int monsterId, float hp)
    {
        if (_monsterId != monsterId) return;
        SetHP(hp);
    }
}
