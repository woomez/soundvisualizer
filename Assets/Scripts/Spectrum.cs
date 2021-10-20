using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//-----------------------------------------------------------------------------
// name: Spectrum.cs
// desc: set up and draw thw spectrum
//-----------------------------------------------------------------------------
public class Spectrum : MonoBehaviour
{
    // prefab reference
    public GameObject the_pfCube;
    //// array of cubes
    public GameObject[,] the_cubes = new GameObject[64, 512];
    // spectrum history matrix
    public float[,] history = new float[64, 512];
    public float[] curr_hist = new float[512];

    // Start is called before the first frame update
    void Start()
    {

        // place the cubes initially
        for (int i = 0; i < the_cubes.GetLength(0); i++)
        {
            float x = -2 * the_cubes.GetLength(1), y = 5 * i, z = 5 * i;
            // increment
            float xIncrement = the_pfCube.transform.localScale.x;
            for (int j = 0; j < the_cubes.GetLength(1); j++)

            {
                // instantiate
                GameObject go = Instantiate(the_pfCube);
                // color material
                //go.GetComponent<Renderer>().material.SetColor("_BaseColor", new Color(.5f, 0.01f * i, 0.01f * i));
                // transform it
                go.transform.position = new Vector3(x, 0, z);
                // increment x

                x += xIncrement;

                // scale it to be 2x wider
                go.transform.localScale = new Vector3(5, 5, 5);
                // give it name
                go.name = "bin" + i;
                // set this as a child of this Spectrum
                go.transform.parent = this.transform;
                // put into array
                the_cubes[i, j] = go;
            }
        }
        //// position this
        this.transform.position = new Vector3(this.transform.position.x, -100, this.transform.position.z);
        StartCoroutine(StoreHistory());
    }

    IEnumerator StoreHistory()
    {
        //float[] spectrum = ChunityAudioInput.the_spectrum;
        //yield return new WaitForSeconds(10f);
        //for (int i = 0; i < curr_hist.GetLength(0); i++)
        //{
        //    curr_hist[i] = spectrum[i];
        //}

        //yield return new WaitForSeconds(.001f);

        //for (int i = 0; i < history.GetLength(0); i++)
        //{
        //    float[] spectrum = ChunityAudioInput.the_spectrum;
        //    for (int j = 0; j < history.GetLength(1); j++)
        //    {
        //        history[i, j] = spectrum[j];
        //    }
        //    yield return new WaitForSeconds(.01f);
        //}
        ////yield return new WaitForSeconds(.001f);
        ///
        while(true)
        {
            for (int i = 0; i < history.GetLength(0); i++)
            {
                float[] spectrum = ChunityAudioInput.the_spectrum;
                for (int j = 0; j < history.GetLength(1); j++)
                {
                    history[i, j] = spectrum[j];
                    float y = 500 * Mathf.Sqrt(history[i, j]);
                    the_cubes[i, j].transform.localScale =
                        new Vector3(the_cubes[i, j].transform.localScale.x,
                        y,
                        the_cubes[i, j].transform.localScale.z);
                    the_cubes[i, j].transform.localPosition =
                        new Vector3(the_cubes[i, j].transform.localPosition.x,
                        y / 2,
                        the_cubes[i, j].transform.localPosition.z);
                    //yield return null;
                }
                yield return new WaitForSeconds(0.5f);
                Debug.Log("Seconds passed");
            }
        }
        
        ////yield return new WaitForSeconds(.001f);
    }


    // Update is called once per frame
    void Update()
    {
        
        //for (int i = 0; i < history.GetLength(0); i++)
        //{
        //    StopCoroutine(StoreHistory());
        //    StartCoroutine(StoreHistory());
        //    for (int j = 0; j < history.GetLength(1); j++)
        //    {
        //        //float y = 2500 * Mathf.Sqrt(history[i, j]);
        //        float y = 2500 * Mathf.Sqrt(curr_hist[j]);
        //        the_cubes[i, j].transform.localScale =
        //            new Vector3(the_cubes[i, j].transform.localScale.x,
        //            y,
        //            the_cubes[i, j].transform.localScale.z);
        //        the_cubes[i, j].transform.localPosition =
        //            new Vector3(the_cubes[i, j].transform.localPosition.x,
        //            y / 2,
        //            the_cubes[i, j].transform.localPosition.z);
        //    }
        //}
    }
}
