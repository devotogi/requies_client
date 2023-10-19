using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Update is called once per frame
    int[,] map = new int[257, 257];
    HashSet<Vector3> keys = new HashSet<Vector3>();

    private void Start()
    {
        transform.position = new Vector3(0, 5, 0);

        for (int z = 0; z <= 256; z++)
            for (int x = 0; x <= 256; x++)
                map[z,x] = 0;


        for (int z = 0; z <= 256; z++)
        {
            RaycastHit hit;
            for (int x = 0; x <= 256; x++)
            {
                transform.position = new Vector3(x, 30, z);

                bool isHit = Physics.BoxCast(transform.position, transform.localScale / 2, Vector3.down, out hit, transform.rotation, 100f, LayerMask.GetMask("WALL"));

                if (isHit) 
                {
                    map[z,x] = 1;
                    Vector3 debugCube = new Vector3(hit.point.x, 0, hit.point.z);
                    keys.Add(debugCube);
                }
            }
        }

        byte[] buffer = new byte[264208 + 1];
        MemoryStream ms = new MemoryStream(buffer);
        ms.Position = 0;
        BinaryWriter bw = new BinaryWriter(ms);

        bw.Write((Int32)264208);
        bw.Write((Int32)256); // z 4
        bw.Write((Int32)256); // x 4
        for (int z = 0; z <= 256; z++)
        {
            for (int x = 0; x <= 256; x++) 
            {
                bw.Write((Int32)map[z, x]); // 4 * 257 * 257 = 264,196
            }
        }
        FileStream fs;
        fs = File.Create("C:\\Users\\jgkang\\Desktop\\map\\map.dat");
        fs.Write(buffer);
        fs.Close();
    }


    void OnDrawGizmos()
    {

        //for (int i = 0; i <= 256; i++)
        //{
        //    Debug.DrawLine(new Vector3(i, 0, 0), new Vector3(i, 0, 256), Color.blue);
        //    Debug.DrawLine(new Vector3(0, 0, i), new Vector3(256, 0, i), Color.blue);
        //}

        foreach (var v3 in keys) 
        {
            Gizmos.DrawCube(v3, new Vector3(1, 1, 1));
        }
    }
}