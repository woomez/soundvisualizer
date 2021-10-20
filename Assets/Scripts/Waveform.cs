using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//-----------------------------------------------------------------------------
// name: Waveform.cs
// desc: set up and draw the audio waveform
//-----------------------------------------------------------------------------
public class Waveform : MonoBehaviour
{
    // prefab reference
    public GameObject the_pfCube;
    // array of game objects
    public GameObject[] the_cubes = new GameObject[256];
    // controllable scale
    public float MY_SCALE = 300;

    // Start is called before the first frame update
    void Start()
    {
        float x = -256, y = 250, z = 0;
        // calculate
        float xIncrement = 0.4f*(the_pfCube.transform.localScale.x);

        for( int i = 0; i < the_cubes.Length; i++ )
        {
            // instantiate a prefab game object
            GameObject go = Instantiate(the_pfCube);

            // default position
            go.transform.position = new Vector3(x, y, z);

            go.transform.localScale = new Vector3(2, 2, 2);
            // increment the x position
            x += xIncrement;
            // give a name!
            go.name = "cube" + i;
            // set a child of this waveform
            go.transform.parent = this.transform;
            // put into array
            the_cubes[i] = go;
        }

        // position this
        this.transform.position = new Vector3(this.transform.position.x, 120, this.transform.position.z);
    }

    //Update is called once per frame
    void Update()
    {
        // local reference to the time domain waveform
        float[] wf = ChunityAudioInput.the_waveform;
        //float[] new_wave = new float[256];
        ////average out
        //for (int i = 0; i < 256; i++)
        //{
        //    new_wave[i] = (wf[4 * i] + wf[4 * i + 1] + wf[4 * i + 2] + wf[4 * i + 3]) / 4;
        //}

        // position the cubes
        for (int i = 0; i < the_cubes.Length; i++)
        {
            the_cubes[i].transform.localPosition =
            new Vector3(the_cubes[i].transform.localPosition.x,
                        MY_SCALE * wf[i],
                        the_cubes[i].transform.localPosition.z);



        }
    }
}
