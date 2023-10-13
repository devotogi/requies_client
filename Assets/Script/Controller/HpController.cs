using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpController : MonoBehaviour
{
    private Image _hpImg;
    private float _hp = 1000f;
    private float _hpMax = 1000f;
    private Camera _camera;

    void Awake()
    {
        _hpImg = transform.GetChild(0).transform.GetChild(0).GetComponent<Image>();
        _hpImg.fillAmount = _hp / _hpMax;
        _camera = Camera.main;  
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

    public void LookMainCamera() 
    {
        if (_camera == null)
            _camera = Camera.main;

        if (_camera == null)
            return;

        transform.LookAt(_camera.transform.position);
    }
}
