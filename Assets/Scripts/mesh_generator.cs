using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mesh_generator : MonoBehaviour
{
    //Public Properties
    public int Dimension = 512;
    public float[,] history = new float[512, 512];

    //Mesh
    protected MeshFilter MeshFilter;
    protected Mesh Mesh;

    // Start is called before the first frame update
    void Start()
    {
        //Mesh Setup
        Mesh = new Mesh();
        Mesh.name = gameObject.name;

        Mesh.vertices = GenerateVerts();
        Mesh.triangles = GenerateTries();
        Mesh.RecalculateNormals();
        Mesh.RecalculateBounds();

        MeshFilter = gameObject.AddComponent<MeshFilter>();
        MeshFilter.mesh = Mesh;

        StartCoroutine(StoreHistory());
    }

    private Vector3[] GenerateVerts()
    {
        var verts = new Vector3[(Dimension + 1) * (Dimension + 1)];

        //equaly distributed verts
        for (int x = 0; x <= Dimension; x++)
            for (int z = 0; z <= Dimension; z++)
                verts[index(x, z)] = new Vector3(x, 0, z);

        return verts;
    }

    private int[] GenerateTries()
    {
        var tries = new int[Mesh.vertices.Length * 6];

        //two triangles are one tile
        for (int x = 0; x < Dimension; x++)
        {
            for (int z = 0; z < Dimension; z++)
            {
                tries[index(x, z) * 6 + 0] = index(x, z);
                tries[index(x, z) * 6 + 1] = index(x + 1, z + 1);
                tries[index(x, z) * 6 + 2] = index(x + 1, z);
                tries[index(x, z) * 6 + 3] = index(x, z);
                tries[index(x, z) * 6 + 4] = index(x, z + 1);
                tries[index(x, z) * 6 + 5] = index(x + 1, z + 1);
            }
        }

        return tries;
    }

    private int index(int x, int z)
    {
        return x * (Dimension + 1) + z;
    }

    private int index(float x, float z)
    {
        return index((int)x, (int)z);
    }


    IEnumerator StoreHistory()
    {
        while (true)
        {

            var verts = Mesh.vertices;
            for (int x = 0; x <= Dimension; x++)
            {
                float[] spectrum = ChunityAudioInput.the_spectrum;
                for (int z = 0; z <= Dimension; z++)
                {
                    history[x, z] = spectrum[z];
                    var y = 35 * history[x, z];

                    verts[index(x, z)] = new Vector3(x, y, z);
                }
                yield return new WaitForSeconds(0.01f);
                Mesh.vertices = verts;
                Mesh.RecalculateNormals();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

}