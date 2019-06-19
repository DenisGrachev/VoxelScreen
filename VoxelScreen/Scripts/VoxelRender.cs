using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Text;



public class VoxelRender
{
  
    

    public bool isDynamicOnly = false;
    
    int voxels = 0;

    //Dynamic voxels list for draw directly on voxel screen.
    public List<Voxel> dynamicVoxels = new List<Voxel>();

    //Camera position.
    Vector3 viewPos = Vector3.zero;

    //Render stats
    string renderStat = "";
    public string lastRenderStat = "";

    //Eight render threads.
    Thread t1, t2, t3, t4, t5, t6, t7, t8;

    //Resolution    
    public int Width;
    public int Depth;
    public int Height;
    int resMult = 24;
    //
    int Width2, Depth2, Height2,  Depth2Height2;

    //Main voxel screen - array of bytes, every byte is number of color in palette.
    byte[] voxelScreen;
    //Level size.
    int scaleWidth = 1;
    int scaleDepth = 1;
    //Level array.
    public byte[] levelSpace;

    //Arrays for dynamic meshes
    List<Vector3>[] vertices = new List<Vector3>[8];
    List<int>[] triangles = new List<int>[8];
    List<Color>[] colors = new List<Color>[8];
        

    //one palette for all meshes
    public Color32[] palette = new Color32[256];
    
    //8 meshes
    Mesh[] mesh = new Mesh[8];

    public VoxelRender(int Width, int Depth, int Height,int levelWidthScale,int levelDepthScale)
    {
        scaleDepth = levelDepthScale;
        scaleWidth = levelWidthScale;

        //Fix resolution by 8        
        if (Width < 32)
            Width = 32;
        this.Width = ((Width) / 8) * 8;

        if (Height < 8)
            Height = 8;
        this.Height = ((Height) / 8) * 8;

        if (Depth < 8)
            Depth = 8;
        this.Depth = ((Depth) / 8) * 8;

        InitRender();

    }

    public VoxelRender(int Width,int Depth, int Height)
    {
        scaleDepth = 1;
        scaleWidth = 1;

        //Fix resolution by 8        
        if (Width < 32)
            Width = 32;
        this.Width = ((Width) / 8) * 8;

        if (Height < 8)
            Height = 8;
        this.Height = ((Height) / 8) * 8;

        if (Depth < 8)
            Depth = 8;
        this.Depth = ((Depth) / 8) * 8;

        InitRender();

    }

    void InitRender()
    {
        //Set multiplyer
        resMult = Width / 8;

        //Set some variables.        
        Width2 = Width + 2;        
        Depth2 = Depth + 2;        
        Height2 = Height + 2;


        Depth2Height2 = Depth2 * Height2;

        //Init voxel and level spaces.
        voxelScreen = new byte[Width2 * Depth2 * Height2];
        levelSpace = new byte[Width * scaleWidth * Depth * scaleDepth * Height];

        //Create lists.
        for (int i = 0; i < 8; i++)
        {
            vertices[i] = new List<Vector3>();
            triangles[i] = new List<int>();
            colors[i] = new List<Color>();

        }

        GameObject voxelRenderer = GameObject.Find("VoxelChunks");

        voxelRenderer.transform.position = new Vector3((-Width / 2) * 0.1f, 0, (-Depth / 2) * 0.1f);

        //Attach chunk meshes to our buffer meshes.
        for (int i = 0; i < 8; i++)
        {
            mesh[i] = voxelRenderer.transform.GetChild(i).gameObject.GetComponent<MeshFilter>().mesh;
            mesh[i].indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            mesh[i].MarkDynamic();
        }

        //Build geometry one time
        RunGeom();
    }

    public void GenerateRandomPalette()
    {
        for (int i = 0; i < 256; i++)
        {
            palette[i] = new Color32((byte)Random.Range(0, 255), (byte)Random.Range(0, 255), (byte)Random.Range(0, 255),1);
        }
    }


    public void LoadPaletteFromMesh(VoxelMesh paletteMesh)
    {
        //fill base pallete from any mesh
        System.Array.Copy(paletteMesh.colors, palette, 256);
    }
        
    private void RunGeom()
    {
        renderStat = "Total Voxels: "+voxels.ToString();
        voxels = 0;

        t1 = new Thread(buildMesh1);
        t1.Priority = System.Threading.ThreadPriority.Highest;
        t1.IsBackground = true;
        t1.Start();

        t2 = new Thread(buildMesh2);
        t2.Priority = System.Threading.ThreadPriority.Highest;
        t2.IsBackground = true;
        t2.Start();

        t3 = new Thread(buildMesh3);
        t3.Priority = System.Threading.ThreadPriority.Highest;
        t3.IsBackground = true;
        t3.Start();

        t4 = new Thread(buildMesh4);
        t4.Priority = System.Threading.ThreadPriority.Highest;
        t4.IsBackground = true;
        t4.Start();

        t5 = new Thread(buildMesh5);
        t5.Priority = System.Threading.ThreadPriority.Highest;
        t5.IsBackground = true;
        t5.Start();

        t6 = new Thread(buildMesh6);
        t6.Priority = System.Threading.ThreadPriority.Highest;
        t6.IsBackground = true;
        t6.Start();

        t7 = new Thread(buildMesh7);
        t7.Priority = System.Threading.ThreadPriority.Highest;
        t7.IsBackground = true;
        t7.Start();

       t8 = new Thread(buildMesh8);
       t8.Priority = System.Threading.ThreadPriority.Highest;
        t8.IsBackground = true;
        t8.Start();
        

        //Run 8 parallel threads
        /*
        Stopwatch sw = new Stopwatch();
        sw.Start();

        var result = Parallel.For(0, 8, (k) =>
          {
              buildMesh(k);            
          }
          

        );


        sw.Stop();
        renderStat = sw.ElapsedMilliseconds.ToString();
        */
    }

    void buildMesh1()
    {
        buildMesh(0);               
    }

    void buildMesh2()
    {
        buildMesh(1);
    }

    void buildMesh3()
    {
        buildMesh(2);
    }

    void buildMesh4()
    {
        buildMesh(3);
    }

    void buildMesh5()
    {
        buildMesh(4);
    }

    void buildMesh6()
    {
       buildMesh(5);
    }

    void buildMesh7()
    {
        buildMesh(6);
    }

    void buildMesh8()
    {
        buildMesh(7);
    }

    


    //private Object thisLock = new Object();

    //k  - whick mesh and vertices array to use
    //every call gets k+4 array size
    void buildMesh(int k)
    {

        int xstart = (resMult) * k + 1;
        
        //Clears lists
        
            if (vertices[k].Count > 0)
            {
                vertices[k].Clear();
                //normals[k+i].Clear();
                triangles[k].Clear();
                colors[k].Clear();
            }        

 

        int totalVoxels = 0;

        int vertCount = 0;        
        Color32 color;

        //for (int z = zstart; z < zend + 1; z++)
        //   for (int x = xstart; x < xend + 1; x++)
        //     for (int y = 1; y < Height + 1; y++)    

        int startVoxel = (xstart) * Height2 * Depth2;
        int endVoxel = startVoxel + resMult*Height2*Depth2;

        int x = xstart;
        int z = Depth2;
        int y = 0;       

        for (int voxel = startVoxel; voxel < endVoxel; voxel++)
        //z-y x-z y-x
        {
            z++;
            if (z >= Depth2)
            {
                z = 0;
                y++;
            }

            if (y >= Height2)
            {
                y = 0;
                x++;
            }
            if (voxelScreen[voxel] == 0)
                continue;

                        totalVoxels++;

                        //Front vertices
                        Vector3 topLeftFront = new Vector3(x - 0.5f, y + 0.5f, z - 0.5f);
                        Vector3 topLeftBack = new Vector3(x - 0.5f, y + 0.5f, z + 0.5f);
                        Vector3 topRightFront = new Vector3(x + 0.5f, y + 0.5f, z - 0.5f);
                        Vector3 topRightBack = new Vector3(x + 0.5f, y + 0.5f, z + 0.5f);

                        //back vertices
                        Vector3 btmLeftFront = new Vector3(x - 0.5f, y - 0.5f, z - 0.5f);
                        Vector3 btmLeftBack = new Vector3(x - 0.5f, y - 0.5f, z + 0.5f);
                        Vector3 btmRightFront = new Vector3(x + 0.5f, y - 0.5f, z - 0.5f);
                        Vector3 btmRightBack = new Vector3(x + 0.5f, y - 0.5f, z + 0.5f);
                        
                        color = palette[voxelScreen[voxel]];                        

                //front                                                          
                //  if (voxelScreen[baseCoords - Width2] == 0)
                    if (voxelScreen[voxel - 1] == 0)
                    {
                            //front                            
                            vertices[k].Add(btmLeftFront); vertices[k].Add(btmRightFront); vertices[k].Add(topLeftFront); vertices[k].Add(topRightFront);
                            vertCount += 4;
                            triangles[k].Add(vertCount - 4); triangles[k].Add(vertCount - 2); triangles[k].Add(vertCount - 3);
                            triangles[k].Add(vertCount - 2); triangles[k].Add(vertCount - 1); triangles[k].Add(vertCount - 3);
                            colors[k].Add(color); colors[k].Add(color); colors[k].Add(color);colors[k].Add(color);                      
                            

                        }


                
                        if (voxelScreen[voxel + Depth2] == 0)
                //top
                {
                            vertices[k].Add(topLeftFront); vertices[k].Add(topRightFront); vertices[k].Add(topLeftBack); vertices[k].Add(topRightBack);
                            vertCount += 4;
                            triangles[k].Add(vertCount - 4); triangles[k].Add(vertCount - 2); triangles[k].Add(vertCount - 3);
                            triangles[k].Add(vertCount - 2); triangles[k].Add(vertCount - 1); triangles[k].Add(vertCount - 3);
                            colors[k].Add(color); colors[k].Add(color); colors[k].Add(color); colors[k].Add(color);                    

                }


                  //  if (x>Width2/2)
                    if (voxelScreen[voxel - Depth2Height2] == 0)
                {
                            //left
                            vertices[k].Add(btmLeftBack); vertices[k].Add(btmLeftFront); vertices[k].Add(topLeftBack); vertices[k].Add(topLeftFront);
                            vertCount += 4;
                            triangles[k].Add(vertCount - 4); triangles[k].Add(vertCount - 2); triangles[k].Add(vertCount - 3);
                            triangles[k].Add(vertCount - 2); triangles[k].Add(vertCount - 1); triangles[k].Add(vertCount - 3);
                            colors[k].Add(color); colors[k].Add(color); colors[k].Add(color); colors[k].Add(color);                                   

                }




                // if (voxelScreen[baseCoords + 1] == 0)
                //if (x < Width2 / 2)
                    if (voxelScreen[voxel + Depth2Height2] == 0)
                {
                            //right
                            vertices[k].Add(btmRightFront); vertices[k].Add(btmRightBack); vertices[k].Add(topRightFront); vertices[k].Add(topRightBack);
                            vertCount += 4;
                            triangles[k].Add(vertCount - 4); triangles[k].Add(vertCount - 2); triangles[k].Add(vertCount - 3);
                            triangles[k].Add(vertCount - 2); triangles[k].Add(vertCount - 1); triangles[k].Add(vertCount - 3);
                            colors[k].Add(color); colors[k].Add(color); colors[k].Add(color); colors[k].Add(color);                                     

                }
                                             
                      

                   
                }


        lock (this)
        {          
            voxels += totalVoxels;
        }
   }


    public void BeginDraw()
    {
        //Clear dynamic voxels.
        dynamicVoxels.Clear();

        
    }

    public void EndDraw()
    {
        //Run Geom Building in parallel threads
        RunGeom();
    }

     

    public void WaitGeometry()
    {            

        //Wait Geometry
        while (t1.IsAlive || t2.IsAlive || t3.IsAlive || t4.IsAlive || t5.IsAlive || t6.IsAlive || t7.IsAlive || t8.IsAlive)
            {
            //return;
            System.Threading.Thread.Sleep(1);
                  continue;
            }

        ApplyGeom();
               
    }


    void ApplyGeom()
    {



        //System.GC.Collect();

        //statText = geomText;
        lastRenderStat = renderStat;
        renderStat = "";


        //Ok attach pre mesh to our rendered mesh.            
        for (int zz = 0; zz < 8; zz++)
        {

            if (mesh[zz].vertexCount > 0)
            {
                mesh[zz].Clear();
            }

            if (vertices[zz].Count > 0)
            {
                mesh[zz].SetVertices(vertices[zz]);
                mesh[zz].SetTriangles(triangles[zz], 0);
                mesh[zz].SetColors(colors[zz]);
                mesh[zz].RecalculateNormals();
                //mesh[zz].SetNormals(normals[zz]);
            }


        }



        //Clear a voxel screen.
        System.Array.Clear(voxelScreen, 0, voxelScreen.Length);

        //If dynamic scene only then not copy static leves
        if (!isDynamicOnly)
        {
            //Copy static voxels.
            //Fix boundares
            viewPos.x = Mathf.Clamp(viewPos.x, 0, Width * scaleWidth - Width);
            viewPos.z = Mathf.Clamp(viewPos.z, 0, Depth * scaleDepth - Depth);
            int copyPosx = (int)viewPos.x;
            int copyPosz = (int)viewPos.z;



            //for (int y=0;y<Height;y++)
            //System.Array.Copy(levelSpace, 0+y*Depth , voxelScreen, 1 + 1 * Depth2 * Height2+(y+1)*Depth2, Depth);

            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    // System.Array.Copy(levelSpace, copyPosz+(x+copyPosx)*Height*Depth*scale + y*Depth*scale , voxelScreen, 1 + (1+x) * Depth2 * Height2+(y+1)*Depth2, Depth);                               
                    System.Buffer.BlockCopy(levelSpace, copyPosz + (x + copyPosx) * Height * Depth * scaleDepth + y * Depth * scaleDepth, voxelScreen, 1 + (1 + x) * Depth2 * Height2 + (y + 1) * Depth2, Depth);

        }

        //Draw dynamic voxels.            
        foreach (Voxel voxel in dynamicVoxels)
        {
            //check if voxel visible.            
            if (voxel.x >= viewPos.x && voxel.x < viewPos.x + Width)
                if (voxel.z >= viewPos.z && voxel.z < viewPos.z + Depth)
                    if (voxel.y >= 0 && voxel.y < Height)
                    {
                        int x = voxel.x - (int)viewPos.x;
                        int y = voxel.y;
                        int z = voxel.z - (int)viewPos.z;
                        voxelScreen[(z + 1) + (y + 1) * Depth2 + (x + 1) * Depth2 * Height2] = voxel.color;
                    }

        }


        //Run rebuild again and again.
        //    RunGeom();

    }

    //================VOLUME PROCEDURES============

    public void ClearAll()
    {
        System.Array.Clear(levelSpace, 0, levelSpace.Length);
        System.Array.Clear(voxelScreen, 0, voxelScreen.Length);
        dynamicVoxels.Clear();
    }

    public void ClearStaticLayer()
    {
        System.Array.Clear(levelSpace, 0, levelSpace.Length);
    }

    public void ClearDynamicLayer()
    {
        dynamicVoxels.Clear();

    }

    public void SetViewportPosition(Vector3 position)
    {
        SetViewportPosition((int)position.x, (int)position.z);
    }
    public void SetViewportPosition(int x,int z)
    {
        viewPos.x = x;
        viewPos.z = z;
    }

    public void SetViewportPosition(float x, float z)
    {
        viewPos.x = x;
        viewPos.z = z;
    }

    //================VOXEL PROCEDURES============


    //Set voxel on specific layer.    
    public void SetVoxel(Vector3 position, byte color, Layer layer)
    {
        SetVoxel((int)position.x, (int)position.y, (int)position.z, color, layer);
    }

    public void SetVoxel(int x,int y,int z,byte color,Layer layer)
    {
        if (layer == Layer.Dynamic)
        {
            dynamicVoxels.Add(new Voxel(x,y,z, color));
        }
        else
        {            
            if (x >= 0)
                if (z >= 0)
                    if (y >= 0)
                        if (x < Width * scaleWidth)
                            if (z < Depth * scaleDepth)
                                if (y < Height)
                                {
                                    levelSpace[z + y * Depth * scaleDepth + x * Height * Depth * scaleDepth] = color;
                                }
        }
    }
    


    //Get voxel from specific layer

    public byte GetVoxel(Vector3 position,Layer layer)
    {
        return GetVoxel((int)position.x, (int)position.y, (int)position.z, layer);
    }

    public byte GetVoxel(int x,int y,int z, Layer layer)
    {        
        if (layer == Layer.Static)
        {
            


            byte res = 0;

            if (x >= 0)
                if (z >= 0)
                    if (y >= 0)
                        if (x < Width * scaleWidth)
                            if (z < Depth * scaleDepth)
                                if (y < Height)
                                {
                                    res = levelSpace[z + y * Depth * scaleDepth + x * Height * scaleDepth * Depth];
                                }

            return res;
        }

        else
        {
            //It super slow, but i dont't think you need it :)
            foreach (Voxel voxel in dynamicVoxels)
            {
                if (x==voxel.x)
                    if (z==voxel.z)
                        if (y==voxel.y)
                        {
                            return voxel.color;
                        }
            }
        }


        return 0;
    }
    //================DIRTY LINE3D====================

    static void Swap<T>(ref T x, ref T y)
    {
        T tmp = y;
        y = x;
        x = tmp;
    }

    public void DrawLine(Vector3 start, Vector3 end, byte color,Layer layer)
    {
        DrawLine((int)start.x, (int)start.y, (int)start.z, (int)end.x, (int)end.y, (int)end.z, color,layer);
    }

    public void DrawLine(int x0, int y0, int z0, int x1, int y1, int z1, byte color,Layer layer)
    {
        bool steepXY = Mathf.Abs(y1 - y0) > Mathf.Abs(x1 - x0);
        if (steepXY) { Swap(ref x0, ref y0); Swap(ref x1, ref y1); }

        bool steepXZ = Mathf.Abs(z1 - z0) > Mathf.Abs(x1 - x0);
        if (steepXZ) { Swap(ref x0, ref z0); Swap(ref x1, ref z1); }

        int deltaX = Mathf.Abs(x1 - x0);
        int deltaY = Mathf.Abs(y1 - y0);
        int deltaZ = Mathf.Abs(z1 - z0);

        int errorXY = deltaX / 2, errorXZ = deltaX / 2;

        int stepX = (x0 > x1) ? -1 : 1;
        int stepY = (y0 > y1) ? -1 : 1;
        int stepZ = (z0 > z1) ? -1 : 1;

        int y = y0, z = z0;

        // Check if the end of the line hasn't been reached.
        for (int x = x0; x != x1; x += stepX)
        {
            int xCopy = x, yCopy = y, zCopy = z;

            if (steepXZ) Swap(ref xCopy, ref zCopy);
            if (steepXY) Swap(ref xCopy, ref yCopy);

            SetVoxel(xCopy, yCopy, zCopy, color,layer);

            errorXY -= deltaY;
            errorXZ -= deltaZ;

            if (errorXY < 0)
            {
                y += stepY;
                errorXY += deltaX;
            }

            if (errorXZ < 0)
            {
                z += stepZ;
                errorXZ += deltaX;
            }
        }
    }


    //================SOME MESH PROCEDURES============
    public void DrawBox(Vector3 position,Vector3 size,byte color,Layer layer)
    {
        DrawBox((int)position.x, (int)position.y, (int)position.z, (int)size.x, (int)size.y, (int)size.z, color, layer);
    }

    public void DrawBox(int x, int y, int z, int width, int height, int depth, byte color,Layer layer)
    {
        for (int xx = x; xx < x + width; xx++)
            for (int zz = z; zz < z + depth; zz++)
                for (int yy = y; yy < y + height; yy++)
                    SetVoxel(xx,yy,zz,color,layer);

    }

    public void DrawSphere(Vector3 position,int radius, byte color, Layer layer)
    {
        DrawSphere((int)position.x, (int)position.y, (int)position.z, radius, color, layer);
    }

    public void DrawSphere(int x,int y,int z, int radius, byte color,Layer layer)
    {
        Vector3 center = new Vector3(x, y, z);
        for (int xx = -radius; xx < radius; xx++)
            for (int yy = -radius; yy < radius; yy++)
                for (int zz = -radius; zz < radius; zz++)
                {
                    Vector3 voxelPosition = new Vector3(xx, yy, zz);
                    float distance = Vector3.Distance(Vector3.zero, voxelPosition);
                    if (distance < radius)
                        SetVoxel(center+voxelPosition, color, layer);
                }
    }

    public void DrawMesh(VoxelMesh voxelMesh,int x,int y,int z, Layer layer)
    {
        DrawMesh(voxelMesh,new Vector3(x, y, z), layer);
    }

    public void DrawMesh(VoxelMesh voxelMesh,Vector3 position,Layer layer)
    {
        foreach (Voxel voxel in voxelMesh.voxels)
        {
            Vector3 v3 = voxel.position - new Vector3(voxelMesh.width / 2, voxelMesh.height / 2, voxelMesh.depth / 2);
            SetVoxel(v3 + position, voxel.color,layer);
        }
    }





}
