using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    void Awake()
    {
        GameObject network = new GameObject();
        network.name = "Network";
        network.AddComponent<Network>();
    }
}
