using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;


public enum Layer { Dynamic, Static };

public struct MagicaVoxelData
{
    public byte x;
    public byte y;
    public byte z;
    public byte color;

    public MagicaVoxelData(BinaryReader stream, bool subsample)
    {
        x = (byte)(subsample ? stream.ReadByte() / 2 : stream.ReadByte());
        y = (byte)(subsample ? stream.ReadByte() / 2 : stream.ReadByte());
        z = (byte)(subsample ? stream.ReadByte() / 2 : stream.ReadByte());
        color = stream.ReadByte();
    }
}

//Voxel class
public struct Voxel
{
    public int x,y,z;
    public Vector3 position;
    public byte color;

    public Voxel(int x, int y, int z, byte lColor)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.position = new Vector3(x, y, z);

        color = lColor;
    }

    public Voxel(Vector3 position, byte lColor)
    {
        this.position = position;
        this.x = (int)position.x;
        this.y = (int)position.y;
        this.z = (int)position.z;
        color = lColor;
    }

}


        //Voxel mesh class
    public class VoxelMesh
    {

        public int width, height, depth = 0;
        public List<Voxel> voxels = new List<Voxel>();
        public Color32[] colors = new Color32[256];
        
        public VoxelMesh()
        {

        }
        //load mesh from file
        public VoxelMesh(string fileName)
        {
            loadFromFile(fileName);
        }

        public void MakeSphere(int radius,byte color)
        {
            voxels.Clear();
            for (int x = -radius; x < radius; x++)
                for (int y = -radius; y < radius; y++)
                    for (int z = -radius; z < radius; z++)
                        {
                        Vector3 voxelPosition = new Vector3(x, y, z);
                        float distance = Vector3.Distance(Vector3.zero, voxelPosition);
                        if (distance < radius)                        
                            voxels.Add(new Voxel(voxelPosition, color));
                        }

        width = radius * 2;
        height = radius * 2;
        depth = radius * 2;
        }

    public void MakeCube(int size, byte color)
    {
        voxels.Clear();
        for (int x = -size/2; x < size/2; x++)
            for (int y = -size/2; y < size/2; y++)
                for (int z = -size/2; z < size/2; z++)
                {                    
                        voxels.Add(new Voxel(x,y,z, color));
                }

        width = size;
        height = size;
        depth = size;
    }

    //////loading magica voxel//////////////


        public void loadFromFile(string fileName)
        {
        //using (BinaryReader stream = new BinaryReader(new FileStream(Application.dataPath + "/StreamingAssets/"+fileName, FileMode.Open)))        
        TextAsset asset = Resources.Load(fileName) as TextAsset;
        Stream s = new MemoryStream(asset.bytes);
        //BinaryReader br = new BinaryReader(s);

        using (BinaryReader stream = new BinaryReader(s))
        {           
                MagicaVoxelData[] voxelData = null;

                string magic = new string(stream.ReadChars(4));
                //int version = 
                stream.ReadInt32();


                // a MagicaVoxel .vox file starts with a 'magic' 4 character 'VOX ' identifier
                if (magic == "VOX ")
                {
                    int width = 0, height = 0, depth = 0;
                    bool subsample = false;

                    while (stream.BaseStream.Position < stream.BaseStream.Length)
                    {
                        // each chunk has an ID, size and child chunks
                        char[] chunkId = stream.ReadChars(4);
                        int chunkSize = stream.ReadInt32();
                        //int childChunks = 
                        stream.ReadInt32();
                        string chunkName = new string(chunkId);

                        // there are only 2 chunks we only care about, and they are SIZE and XYZI
                        if (chunkName == "SIZE")
                        {
                            width = stream.ReadInt32();
                            depth = stream.ReadInt32();
                            height = stream.ReadInt32();

                            //if (width > 32 || height > 32) subsample = true;

                            stream.ReadBytes(chunkSize - 4 * 3);
                        }
                        else if (chunkName == "XYZI")
                        {
                            // XYZI contains n voxels
                            int numVoxels = stream.ReadInt32();
                            // int div = (subsample ? 2 : 1);

                            // each voxel has x, y, z and color index values
                            voxelData = new MagicaVoxelData[numVoxels];
                            for (int i = 0; i < voxelData.Length; i++)
                                voxelData[i] = new MagicaVoxelData(stream, subsample);
                        }
                        else if (chunkName == "RGBA")
                        {

                            for (int i = 0; i < 256; i++)
                            {
                                byte r = stream.ReadByte();
                                byte g = stream.ReadByte();
                                byte b = stream.ReadByte();
                                byte a = stream.ReadByte();

                                // convert RGBA to our custom voxel format (16 bits, 0RRR RRGG GGGB BBBB)                                
                                colors[i] = new Color32(r, g, b, a);
                            }
                        }
                        else stream.ReadBytes(chunkSize);   // read any excess bytes
                    }

                    if (voxelData.Length == 0) return; // failed to read any valid voxel data


                    // sizes
                    this.width = width;
                    this.height = height;
                    this.depth = depth;


                    foreach (MagicaVoxelData v in voxelData)
                    {
                        try
                        {
                            voxels.Add(new Voxel(v.x, v.z, v.y, (byte)(v.color - 1)));
                        }
                        catch (Exception)
                        {

                            // Console.WriteLine(e);
                            //Debug.Log(v.x + " " + v.y + " " + v.z);
                        }

                    }


                }
            }
        }




    }


