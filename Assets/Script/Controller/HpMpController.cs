using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HpMpController : MonoBehaviour
{
    private Image _hpImg;
    private Image _mpImg;
    private TMP_Text _hpText;   
    private TMP_Text _mpText;   

    private float _hp = 1000f;
    private float _mp = 1000f;
    private float _hpMax = 1000f;
    private float _mpMax = 1000f;

    void Start()
    {
        Transform hpBar = transform.GetChild(0);
        Transform mpBar = transform.GetChild(1);

        _hpImg = hpBar.GetChild(0).GetComponent<Image>();
        _hpText = hpBar.GetChild(1).GetComponent<TMP_Text>();

        _mpImg = mpBar.GetChild(0).GetComponent<Image>();
        _mpText = mpBar.GetChild(1).GetComponent<TMP_Text>();

        _hpText.text = $"{_hp} / {_hpMax}";
        _mpText.text = $"{_mp} / {_mpMax}";

        _hpImg.fillAmount = _hp / _hpMax;
        _mpImg.fillAmount = _mp / _mpMax;
    }

    public void SetHp(float hp) 
    {
        _hp = hp;
        HpMpUpdate();
    }

    public void SetMp(float mp) 
    {
        _mp = mp;
        HpMpUpdate();
    }

    public float GetHP() 
    {
        return _hp;
    }

    private void HpMpUpdate() 
    {
        _hpImg.fillAmount = _hp / _hpMax;
        _mpImg.fillAmount = _mp / _mpMax;
        _hpText.text = $"{_hp} / {_hpMax}";
        _mpText.text = $"{_mp} / {_mpMax}";
    }
}
