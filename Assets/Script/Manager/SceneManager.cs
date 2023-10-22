using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

public class SceneManager : MonoBehaviour
{
    [SerializeField]
    public Type.SceneType _type = Type.SceneType.Run;

    void Awake()
    {
        switch (_type) 
        {
            case Type.SceneType.Debug:
                Managers.Resource.Instantiate("Object/MapMaker");
                Managers.Resource.Instantiate("Camera/DevCamera");

                break;

            case Type.SceneType.Run:
                GameObject network = new GameObject();
                network.name = "Network";
                network.AddComponent<Network>();

                Managers.Resource.Instantiate("UI/PlayerUI");
                break;
        }
    }
}
