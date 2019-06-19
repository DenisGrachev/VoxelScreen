using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicaVoxelMesh : MonoBehaviour
{

    //VoxelRender.
    VoxelRender voxelRender;
    //Voxel mesh
    VoxelMesh voxelMesh;

    // Use this for initialization.
    void Start()
    {

        //Creat new VoxelRender with 192x192x64 resolution.
        voxelRender = new VoxelRender(192, 192, 64);
        //Load voxel mesh from file.
        voxelMesh = new VoxelMesh("Knight");
        //Load palette from mesh to VoxelRender.
        voxelRender.LoadPaletteFromMesh(voxelMesh);

    }

    // Update is called once per frame.    

    //Sin counters.
    float count = 0.0f;


    void Update()
    {

        //Begin draw.
        voxelRender.BeginDraw();

            count += 0.02f;
            //Draw voxel meshes.
            for (int i = 0; i < 8; i += 2)
            {
                float x = 96 + 64 * Mathf.Sin(count + i * 0.262f);
                float y = 32 + 8 * Mathf.Sin(count * 4 + i * 0.262f);
                float z = 96 + 64 * Mathf.Cos(count + i * 0.262f);
                voxelRender.DrawMesh(voxelMesh, new Vector3(x, y, z), Layer.Dynamic);
            }

            //And put a static mesh to level.        
            float xx = 96 + 86 * Mathf.Sin(count * 0.5f);
            float zz = 96 + 86 * Mathf.Cos(count * 0.5f);
            voxelRender.DrawMesh(voxelMesh,new Vector3(xx, voxelMesh.height / 2, zz),  Layer.Static);        


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

