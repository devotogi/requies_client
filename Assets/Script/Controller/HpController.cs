using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpController : MonoBehaviour
{
    public GameObject _target = null;
    private Image _hpImg;
    private float _hp = 1000f;
    private float _hpMax = 1000f;

    void Awake()
    {
        _hpImg = transform.GetChild(0).GetComponent<Image>();
        _hpImg.fillAmount = _hp / _hpMax;
    }

    private void Update()
    {
       
    }

    public void SetHp(float hp)
    {
        _hp = hp;
        HpUpdate();
    }

    private void HpUpdate()
    {
        _hpImg.fillAmount = _hp / _hpMax;
    }
}
