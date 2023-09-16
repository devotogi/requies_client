using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

public class SceneManager : MonoBehaviour
{ 
    void Awake()
    {
        GameObject network = new GameObject();
        network.name = "Network";
        network.AddComponent<Network>();

        for (int z = 0; z <= 256; z++)
        {
            for (int x = 0; x <= 256; x++)
            {
                if (z % 32 == 0 || x % 32 == 0) 
                {
                    GameObject wallPrefab = Resources.Load<GameObject>($"Prefabs/WALL");
                    GameObject wallgo = Object.Instantiate(wallPrefab);
                    wallgo.transform.position = new Vector3(x, 0, z);
                }
            }
        }
    }
}
