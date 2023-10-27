using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MapMaker : MonoBehaviour
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
                transform.position = new Vector3(x + 0.5f, 100, z + 0.5f);

                bool isHit = Physics.BoxCast(transform.position, transform.localScale / 2, Vector3.down, out hit, transform.rotation, 100f, LayerMask.GetMask("WALL") | LayerMask.GetMask("MainGround"));

                if (isHit) 
                {
                    const int ground = 16;
                    if (hit.transform.gameObject.layer == ground && hit.point.y < 1)
                        continue;

                    map[z,x] = 1;
                    Vector3 debugCube = new Vector3(x + 0.5f, 0, z + 0.5f);
                    keys.Add(debugCube);
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        //for (int i = 0; i <= 256; i++) 
        //{
        //    Debug.DrawLine(new Vector3(i, 0, 0), new Vector3(i, 0, 256), Color.blue);
        //    Debug.DrawLine(new Vector3(0, 0, i), new Vector3(256, 0, i), Color.blue);
        //}

        for (int j = 0; j <= 8; j++)
        {
            int x = j * 32;
            Debug.DrawLine(new Vector3(x, 0, 0), new Vector3(x, 0, 256), Color.blue);

        }

        for (int i = 0; i <= 8; i++)
        {
            int z = i * 32;
            Debug.DrawLine(new Vector3(0, 0, z), new Vector3(256, 0, z), Color.blue);
        }

        //foreach (var v3 in keys) 
        //{
        //    Gizmos.DrawCube(v3, new Vector3(1, 1, 1));
        //}
    }

    public void AddBlock(Vector3 pos) 
    {
        if (keys.Contains(pos) == false)
            keys.Add(pos);
    }

    public void RemoveBlock(Vector3 pos) 
    {
        if (keys.Contains(pos))
            keys.Remove(pos);
    }

    public void MakeFile() 
    {
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
}
