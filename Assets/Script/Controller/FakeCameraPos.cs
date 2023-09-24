using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeCameraPos : MonoBehaviour
{
    private GameObject _player;
    private Vector3 _delta;
    public void Init(GameObject player)
    {
        _player = player;
        _delta = new Vector3(0.0f, 7.09412f, -12.00805f);

    }

    private void Update()
    {

    }

    void LateUpdate()
    {
        transform.position = _player.transform.position + _delta;
    }
}
