using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private GameObject _player;
    private Vector3 _delta;
    private float _speed = 3.0f;
    private HashSet<Renderer> _renderers = new HashSet<Renderer>();
    public void Init(GameObject player)
    {
        _player = player;
    }

    private void Update()
    {

    }

    void LateUpdate()
    {
        _delta = (_player.transform.position - transform.position);
        Vector3 targetPos = new Vector3(_player.transform.position.x, _player.transform.position.y + 0.5f, _player.transform.position.z);
        transform.LookAt(targetPos);
        Debug.DrawRay(transform.position, _delta, Color.red);

        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position, _delta.normalized, _delta.magnitude, LayerMask.GetMask("WALL"));

        if (hits.Length == 0)
        {
            foreach (var render in _renderers) 
            {
                Material Mat = render.material;
                Color matColor = Mat.color;
                matColor.a = 1f;
                Mat.color = matColor;
            }
            _renderers.Clear();
        }

        foreach (var hit in hits) 
        {
            Renderer ObstacleRenderer = hit.transform.gameObject.GetComponentInChildren<Renderer>();

            if (ObstacleRenderer != null) 
            {
                Material Mat = ObstacleRenderer.material;
                Color matColor = Mat.color;
                matColor.a = 0.2f;
                Mat.color = matColor;

                if (_renderers.Contains(ObstacleRenderer) == false)
                    _renderers.Add(ObstacleRenderer);
            }
        }

        //if (Physics.RaycastAll(transform.position, _delta, out hit, _delta.magnitude, LayerMask.GetMask("WALL")))
        //{
        //    _ObstacleRenderer = hit.transform.gameObject.GetComponentInChildren<Renderer>();
        //    if (_ObstacleRenderer != null)
        //    {
        //        // 3. Metrial의 Aplha를 바꾼다.
        //        Material Mat = _ObstacleRenderer.material;
        //        Color matColor = Mat.color;
        //        matColor.a = 0.5f;
        //        Mat.color = matColor;
        //    }
        //}
        //else
        //{
        //    if (_ObstacleRenderer != null)
        //    {
        //        Material Mat = _ObstacleRenderer.material;
        //        Color matColor = Mat.color;
        //        matColor.a = 1f;
        //        Mat.color = matColor;
        //    }
        //}

        //Debug.DrawRay(transform.position, _delta, Color.red);

        //RaycastHit hit;
        //if (Physics.Raycast(_player.transform.position, _delta, out hit, _delta.magnitude, LayerMask.GetMask("WALL")))
        //{
        //    float dist = (hit.point - _player.transform.position).magnitude * 0.8f;
        //    transform.position = _player.transform.position + (_delta.normalized * dist);
        //}
        //else
        //{
        //    transform.position = _player.transform.position + _delta;
        //}

        //float scroll = Input.GetAxis("Mouse ScrollWheel") * _speed;
        //scroll = scroll * -1;

        ////최대 줌인
        //if (gameObject.GetComponent<Camera>().fieldOfView <= 20.0f && scroll < 0)
        //{
        //    gameObject.GetComponent<Camera>().fieldOfView = 20.0f;
        //}
        //// 최대 줌 아웃
        //else if (gameObject.GetComponent<Camera>().fieldOfView >= 60.0f && scroll > 0)
        //{
        //    gameObject.GetComponent<Camera>().fieldOfView = 60.0f;
        //}
        //// 줌인 아웃 하기.
        //else
        //{
        //    gameObject.GetComponent<Camera>().fieldOfView += scroll;
        //}

        //// 일정 구간 줌으로 들어가면 캐릭터를 바라보도록 한다.
        //if (_player && gameObject.GetComponent<Camera>().fieldOfView <= 30.0f)
        //{
        //    transform.rotation = Quaternion.Slerp(transform.rotation
        //        , Quaternion.LookRotation(_player.gameObject.transform.position - transform.position)
        //        , 0.15f);
        //}
    }
}
