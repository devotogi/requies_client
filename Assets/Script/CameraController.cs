using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private GameObject _player;
    private Vector3 _delta;
    private float _speed = 3.0f;

    public void Init(GameObject player)
    {
        _player = player;
    }

    private void Update()
    {

    }

    void LateUpdate()
    {
        Vector3 targetPos = new Vector3(_player.transform.position.x, _player.transform.position.y + 0.5f, _player.transform.position.z);
        transform.LookAt(targetPos);

        float scroll = Input.GetAxis("Mouse ScrollWheel") * _speed;
        scroll = scroll * -1;

        //�ִ� ����
        if (gameObject.GetComponent<Camera>().fieldOfView <= 20.0f && scroll < 0)
        {
            gameObject.GetComponent<Camera>().fieldOfView = 20.0f;
        }
        // �ִ� �� �ƿ�
        else if (gameObject.GetComponent<Camera>().fieldOfView >= 60.0f && scroll > 0)
        {
            gameObject.GetComponent<Camera>().fieldOfView = 60.0f;
        }
        // ���� �ƿ� �ϱ�.
        else
        {
            gameObject.GetComponent<Camera>().fieldOfView += scroll;
        }

        // ���� ���� ������ ���� ĳ���͸� �ٶ󺸵��� �Ѵ�.
        if (_player && gameObject.GetComponent<Camera>().fieldOfView <= 30.0f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation
                , Quaternion.LookRotation(_player.gameObject.transform.position - transform.position)
                , 0.15f);
        }
    }
}
