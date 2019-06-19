using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicVoxels : MonoBehaviour
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
        //Dynamic layers clears every voxel frame.
        voxelRender.isDynamicOnly = true;

    }

    // Update is called once per frame.    

    //Sin counters.
    float count = 0.0f;
    //swap frames
    bool frame;

    void Update()
    {

        //Swap frames, so we update everything at 30 FPS, for descrete voxel space it still nice.
        //And we have a massive perfomance boost.
        frame = !frame;

        if (frame)
        {
            //Begin draw.
            voxelRender.BeginDraw();


            //Draw some dynamic voxels.
            //Update count.
            count += 0.02f;

            for (int x = 0; x < voxelRender.Width; x++)
                for (int z = 0; z < voxelRender.Depth; z++)
                {
                    float height = 16 + 8 * Mathf.PerlinNoise((float)x * 0.025f, (float)z * 0.025f) + 8 * Mathf.Sin(count + x * 0.01f + z * 0.01f);
                    voxelRender.SetVoxel(x, (int)height, z, (byte)(height + 1), Layer.Dynamic);
                }


            //End draw, it starts a multithreading geometry building.
            voxelRender.EndDraw();
        }
        else
        {
            //Wait geometry.
            voxelRender.WaitGeometry();
        }

    }

    //Draw some stats
    void OnGUI()
    {             
        GUI.Label(new Rect(0, 20, 400, 400), voxelRender.lastRenderStat);
    }

}
