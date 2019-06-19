using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomBoxes : MonoBehaviour {

    //VoxelRender.
    VoxelRender voxelRender;

	// Use this for initialization.
	void Start () {

    //Creat new VoxelRender with 192x192x64 resolution.
    voxelRender = new VoxelRender(192, 192, 64);
    //Fill VoxelRender palette with random colors.
    voxelRender.GenerateRandomPalette();

    }

    // Update is called once per frame.
    //swap frames
    bool frame;
    void Update () {

        //Swap frames, so we update everything at 30 FPS, for descrete voxel space it still nice.
        //And we have a massive perfomance boost.
        frame = !frame;


        if (frame)
        {

            //Begin draw.
            voxelRender.BeginDraw();            
                //Draw random boxes to static layer.
                //Random position.
                int x = Random.Range(0, voxelRender.Width);
                int y = 0;
                int z = Random.Range(0, voxelRender.Width);
                //Random size.
                int width = Random.Range(1, 10);
                int height = Random.Range(1, 64);
                int depth = Random.Range(1, 10);
                //Random color from palette.
                byte color = (byte)Random.Range(1, 255);
                //And put it to static layer.
                voxelRender.DrawBox(x, y, z, width, height, depth, color, Layer.Static);            
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
