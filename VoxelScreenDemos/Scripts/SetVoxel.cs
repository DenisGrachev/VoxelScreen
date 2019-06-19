using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetVoxel : MonoBehaviour
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

    }

    // Update is called once per frame.    

    //Sin counters.
    float count = 0.0f;
    float count2 = 0.0f;

    void Update()
    {        
          //Begin draw.
          voxelRender.BeginDraw();

            //Inc counters.
            count += 0.02f;
            count2 += 0.1f;

            //Calculate some voxels.      
            int x = (int)(96 + 80 * Mathf.Sin(count * 0.6f - count2 * 0.3f));
            int z = (int)(80 + 80 * Mathf.Sin(-0.4f * count + count2 * 0.2f));
            int y = (int)(8 + 8 * Mathf.Sin(0.4f * count + count2 * 0.4f+z*0.1f));
            byte color = (byte)Random.Range(1, 255);
            //Put it to level.
            for (int i = 4; i < 16; i++)
            {
                voxelRender.SetVoxel(x, y + i, z, color,Layer.Static);
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
