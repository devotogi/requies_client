using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureController : MonoBehaviour
{
    protected Type.Dir _dir = Type.Dir.NONE;
    protected Type.State _state = Type.State.IDLE;
    protected Animator _animator;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public virtual void Attacked()
    {
   
    }

    public virtual void Destory()
    {
        Destroy(gameObject);
    }
}
