using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scrolling : MonoBehaviour {


    VoxelRender voxelRender;


    // Use this for initialization
    void Start () {

        //Creat new VoxelRender
        voxelRender = new VoxelRender(128, 128, 64, 2, 2);
        //Fill VoxelRender palette with random colors
        voxelRender.GenerateRandomPalette();

        //Create random cubes as level.       
        for (int cube = 0; cube < 128; cube++)
        {
            voxelRender.DrawBox(Random.Range(0, 128 * 2), 0, Random.Range(0, 128 * 2), Random.Range(8, 32), Random.Range(4, 24), Random.Range(8, 32), (byte)Random.Range(1, 254),Layer.Static);
        }


    }

    //Sin counter.
    float count = 0;

    // Update is called once per frame
    void Update () {

        //Begin draw.
        voxelRender.BeginDraw();

        //Scroll by sin.
        float xView = 64 + 64 * Mathf.Sin(count);
        float zView = 64 + 64 * Mathf.Cos(count);
        //Update count
        count += 0.02f;
        //Update viewpos
        voxelRender.SetViewportPosition(xView, zView);


        //End draw, it starts a multithreading geometry building.
        voxelRender.EndDraw();

        //Wait geometry.
        voxelRender.WaitGeometry();




    }
}
