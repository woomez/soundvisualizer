using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//-----------------------------------------------------------------------------
// name: ChunityRunCode.cs
// desc: run ChucK code from here, either as text or from a .ck file
//-----------------------------------------------------------------------------
public class ChunityRunCode : MonoBehaviour
{
    // the chuck subinstance
    ChuckSubInstance chuck;

    // Start is called before the first frame update
    void Start()
    {
        // get the chuck subinstance on the object this script is attached to
        chuck = GetComponent<ChuckSubInstance>();
        // run code
        
    }

    //void MicSet()
    //{
    //    // run code -- this is the microphone input from chuck
    //    float mic_gain = 0;
    //    if (Input.GetKey(KeyCode.M))
    //    {
    //        mic_gain = 0;
    //    }
    //    if (Input.GetKey(KeyCode.K))
    //    {
    //        mic_gain = 0.5f;
    //    }
    //    GetComponent<ChuckSubInstance>().SetFloat("micg", mic_gain);

    //    chuck.RunCode(@"fun void runMic(float mic_gain)
    //    {
    //        adc => Gain g => dac;
    //        mic_gain => g.gain;
    //        mic_gain => adc.gain;
    //        while (true) 1::second => now;

    //    }
    //    global float micg;

    //    while(true)
    //    {
    //        runMic(micg);
    //    }
    //    ");

    //    //if (Input.GetKey(KeyCode.K))
    //    //{
    //    //    mic_gain = 0f;
    //    //    chuck.RunCode(
    //    //    string.Format(@"adc => Gain g => dac; {0} => g.gain; while( true ) 1::second => now;", mic_gain)
    //    // );
    //    //}
    //}

    void turnonMic()
    {
        // run code -- this is the microphone input from chuck
        chuck.RunCode(
            @"adc => Gain g => dac; while (true) 1::second => now; "
        );
        
    }

    void runFile()
    {
        // run code -- this constructs a sound loop
        chuck.RunCode(
            @"SndBuf buffy => dac;
             me.dir() + ""zhu.wav"" => buffy.read;
             0 => buffy.pos;
            0.4 => buffy.gain;
            0.3 => dac.gain;
            buffy.length() => now;
        "
        );
    }

    void runChuck()
    {
        // run chuck code -- this constructs a sound loop
        chuck.RunCode(
            @"SndBuf buffy => dac;
             me.dir() + ""mixed2.wav"" => buffy.read;
             0 => buffy.pos;
            0.75 => buffy.gain;
            0.75 => dac.gain;
            buffy.length() => now;
        "
        );
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.M))
            { turnonMic(); }
        
        if (Input.GetKey(KeyCode.N))
        {
            runFile();
        }

        if (Input.GetKey(KeyCode.L))
        { runChuck(); }
    }
}
