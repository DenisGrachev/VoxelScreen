using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lines3D : MonoBehaviour
{

    //VoxelRender.
    VoxelRender voxelRender;

    // Use this for initialization.
    void Start()
    {

        //Creat new VoxelRender with 192x192x64 resolution.
        voxelRender = new VoxelRender(192, 192, 64);
        //Fill VoxelRender palette with random colors.
        voxelRender.GenerateRandomPalette();
        //We have only dynamic voxels in scene.
        voxelRender.isDynamicOnly = true;

    }

    // Update is called once per frame.    

    //Sin counters.
    float count = 0.0f;


    void Update()
    {

            //Begin draw.
            voxelRender.BeginDraw();

            //Update count.
            count += 0.05f;

            //Draw Lines.
            for (int i = 0; i < 72; i++)
            {
                float x1 = 96 + 84 * Mathf.Sin(count + i * 5 * 0.0174f);
                float y1 = 24 + 8 * Mathf.Sin(count * 0.5f + i * 5 * 0.0174f);
                float z1 = 96 + 84 * Mathf.Cos(count + i * 5 * 0.0174f);

                float x2 = 96 + 84 * Mathf.Sin(count * 1.2f + (i - 1) * 5 * 0.0174f);
                float y2 = 24 + 16 * Mathf.Sin(count * 0.8f + (i - 1) * 5 * 0.0174f);
                float z2 = 96 + 84 * Mathf.Cos(count * 1.2f + (i - 1) * 5 * 0.0174f);

                //Vector3 center = new Vector3(x, y, z);        
                voxelRender.DrawLine(new Vector3(x1, y1, z1), new Vector3(x2, y2, z2), (byte)(i + 1),Layer.Dynamic);
            }



            //End draw, it starts a multithreading geometry building.
            voxelRender.EndDraw();
       
            //Wait geometry.
            voxelRender.WaitGeometry();
       

    }

    //Draw some stats
    void OnGUI()
    {
        GUI.Label(new Rect(0, 20, 400, 400), voxelRender.lastRenderStat);
    }

}
