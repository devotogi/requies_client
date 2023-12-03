using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BufferEffectController : MonoBehaviour
{
    public GameObject Player { set { _player = value; StartCoroutine(CoEffectEnd()); } }
    private GameObject _player;

    IEnumerator CoEffectEnd() 
    {
        yield return new WaitForSeconds(2.0f);
        Managers.Resource.Destory(gameObject);
    }

    void Update()
    {
        transform.position = _player.transform.position;        
    }
}
