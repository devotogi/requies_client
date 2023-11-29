using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExpController : MonoBehaviour
{
    private Image _expImg;
    private float _exp = 0f;
    private float _defaultExp = 1000f;
    private float _expMax = 1000f;
    private int _level = 1;
    
    public void Init()
    {
        _expImg = transform.GetChild(0).GetComponent<Image>();
    }

    public void SetExp(int level, float exp, float expMax) 
    {
        _level = level;
        _expMax = expMax;   
        _exp = exp;
        _expImg.fillAmount = _exp / _expMax;
    }
}
