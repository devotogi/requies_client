using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevCameraController : MonoBehaviour
{
    [SerializeField]
    float _speed = 10.0f;
    MapMaker _mapMaker;
    void Start()
    {
        _mapMaker = GameObject.FindWithTag("MapMaker").GetComponent<MapMaker>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.W)) 
        {
            transform.position += Vector3.forward * Time.deltaTime * _speed;
        }
        if (Input.GetKey(KeyCode.D)) 
        {
            transform.position += Vector3.right * Time.deltaTime * _speed;
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position += Vector3.back * Time.deltaTime * _speed;
        }
        if (Input.GetKey(KeyCode.A)) 
        {
            transform.position += Vector3.left * Time.deltaTime * _speed;
        }
        if (Input.GetKey(KeyCode.Q)) 
        {
            transform.position += Vector3.up * Time.deltaTime * _speed;
        }
        if (Input.GetKey(KeyCode.E)) 
        {
            transform.position += Vector3.down * Time.deltaTime * _speed;
        }
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity,LayerMask.GetMask("MainGround"))) 
            {
                Vector3 add = new Vector3(((int)hit.point.x) + 0.5f, 0, ((int)hit.point.z) + 0.5f);
                _mapMaker.AddBlock(add);
            }
        }
        if (Input.GetMouseButtonUp(1)) 
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity,LayerMask.GetMask("MainGround")))
            {
                Vector3 remove = new Vector3(((int)hit.point.x) + 0.5f, 0, ((int)hit.point.z) + 0.5f);
                _mapMaker.RemoveBlock(remove);
            }
        }

        if (Input.GetKeyDown(KeyCode.F5)) 
        {
            _mapMaker.MakeFile();
        }
    }
}
