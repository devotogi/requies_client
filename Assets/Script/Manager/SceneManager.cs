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

                //for (int z = 0; z <= 256; z += 32)
                //{
                //    for (int x = 0; x <= 256; x++)
                //    {
                //        GameObject gox = Managers.Resource.Instantiate("Object/WALL");
                //        gox.transform.position = new Vector3(x, 0, z);

                //        GameObject goz = Managers.Resource.Instantiate("Object/WALL");
                //        goz.transform.position = new Vector3(z, 0, x);
                //    }
                //}


                break;
        }
    }
}
