using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutVoxels : MonoBehaviour {

    //VoxelRender.
    VoxelRender voxelRender;

    // Use this for initialization
    void Start () {

        //Creat new VoxelRender with 192x192x64 resolution.
        voxelRender = new VoxelRender(192, 192, 64);
        //Fill VoxelRender palette with random colors.
        voxelRender.GenerateRandomPalette();

        //Create Ground
        byte color = 106;
        for (int x = 0; x < 16; x++)
            for (int z = 0; z < 16; z++)
            {
                voxelRender.DrawBox(x * 12, 0, z * 12, 12, 4, 12, (byte)(color + 8 * ((x + z) % 2)),Layer.Static);
            }



    }

    // Update is called once per frame
    //Sin counters.
    float count = 0.0f;
    void Update () {


        //Begin draw.
        voxelRender.BeginDraw();

        count += 0.02f;

        //Draw Cut Spheres
        for (int i=0;i<8;i++)
        {
            float x = 96 + Mathf.Cos(i * 0.2f + count * 2) * 80;
            float z = 96 + (Mathf.Sin(i * 0.7f + count*1.8f) + Mathf.Sin(i * 0.3f + count * 3)) * 40;
            float y = 4 + 2*Mathf.Cos(i * 0.4f + count*4);
            Vector3 position = new Vector3(x, y, z);

            //Cut From Static Level, if color=0
            voxelRender.DrawSphere(position, 4, 0, Layer.Static);
            //Draw Dynamic
            voxelRender.DrawSphere(position, 4, (byte)(i + 1), Layer.Dynamic);
        }

        //End draw, it starts a multithreading geometry building.
        voxelRender.EndDraw();

        //Wait geometry.
        voxelRender.WaitGeometry();


    }
}
