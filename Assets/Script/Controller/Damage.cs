using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Damage : MonoBehaviour
{
    float _damageSpeed = 0.01f;
    float _maxYpos = 0;
    bool _isMoving = false;
    Camera _camera = null;
    TMP_Text _text;

    public void Init(Vector3 pos, int damage) 
    {
        if (_camera == null)
            _camera = Camera.main;

        if (_text == null)
            _text = transform.GetChild(0).GetComponent<TMP_Text>();

        transform.position = pos;
        _maxYpos = pos.y + 0.5f;
        _isMoving = true;
        _text.text = Convert.ToString(damage);
    }

    void Update()
    {
        if (_camera == null)
            _camera = Camera.main;

        if (_camera == null)
            return;

        Vector3 dir = _camera.transform.position;
        dir.y = 0;
        transform.LookAt(dir);

        if (_maxYpos <= transform.position.y)
            Destroy(gameObject);

        if (_isMoving)
        {
            transform.position += Vector3.up * _damageSpeed;
        }
    }
}
