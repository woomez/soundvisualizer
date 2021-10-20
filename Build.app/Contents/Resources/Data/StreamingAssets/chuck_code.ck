// @title starterModified1.ck
// @author Chris Chafe (cc@ccrma), Hongchan Choi (hongchan@ccrma), Madeline Huberth (mhuberth@ccrma)
// @desc A starter code for homework 2, Music220a-fall-2021
// @note a demonstration/template for auditory streaming

// -------------------------------------------------------------
// This code creates three FM instruments that, when played
// at a slow rate, create one auditory stream. When the tempo is sped up, however,
// different rhythms and melodic groupings pop out of the texture, due to your mind's 
// grouping of the sounds. Outputs to dac and .wav file.

// -------------------------------------------------------------
// array to hold midi pitches (key numbers) 
// these will be converted into the carrier frequencies
[50, 44, 48, 52, 47] @=> int keyNum[];

// how many pitches are in the array
keyNum.cap() => int nPitches;

// -------------------------------------------------------------
// against a cycle of a different length which we'll use to vary 
// instrument parameters
nPitches - 1 => int nInstruments;

<<<nInstruments>>>;
// arrays to hold modulation frequency, timing of modulation envelope, 
// and timing/gain of carrier ADSRs
// really, anything that we want to use to break the repeating pitches
// into multiple streams

// FMFS is a custom class, "FM From Scratch"
// instantiate nInstruments instances of the class
// make arrays for carrier amplitude & carrier amplitude envelope breakpoints, 
// and modulation frequency ratio & index & modulation envelope breakpoints
// ADSR stands for "Attack-Decay-Sustain-Release"
FMFS fm[nInstruments];
float carrierAmp[nInstruments];
float carrierADSR[nInstruments][0]; // 2d array, second dimension will hold an array of float values for ADSR
float pitchRatio[nInstruments];
float modulationRatio[nInstruments];
float modulationIndex[nInstruments];
float modulatorADSR[nInstruments][0];
for (0 => int i; i < nInstruments; i++) 
{
    [1.0,1.0, 3.0, 2.0, 1.0] @=> pitchRatio;
    0.5 + Math.pow(i,3.1) => modulationRatio[i];
    Math.pow(i,3) => modulationIndex[i];
    [.01,.4,.5,.1] @=> carrierADSR[i];
    [.01,.4,1.0,.1] @=> modulatorADSR[i];
    fm[i].out => dac.chan(i%2); // or to hocket channels use, dac.chan(i%2); 
    <<<"instrument",i,": pitchRatio", pitchRatio[i],"  modulationRatio", modulationRatio[i],"  modulationIndex", modulationIndex[i]>>>;
}

// -------------------------------------------------------------
// global parameters

// set a common note duration
800::ms => dur duration;
// initial interOnsetInterval (inverse of tempo)
2500::ms => dur interOnsetInterval;
// accelerate to this smallest interOnsetInterval 
400::ms => dur minInterOnsetInterval;
// index for which pitch is next
0 => int p;
// index for which instrument is next                         
0 => int i;


// loop for 20 seconds
now => time beg;
beg + 40::second => time end;
dac => WvOut2 w => blackhole;
me.sourceDir() + "/mixed2.wav" => string filename;
w.wavFilename(filename);

while (now < end) {
    <<< "Pitch (",p,") keyNum =", keyNum[p], "\tInstrument(", i, ")">>>;
    Std.mtof(keyNum[p]+12.0) => float carrierFreq;
    // play the note
    spork ~fm[i].playFM(duration, carrierFreq, pitchRatio[i], carrierADSR[i], modulationRatio[i], modulationIndex[i], modulatorADSR[i]);
    // increment pitch and instrument
    p++;
    i++;
    // cycle pitches through full array
    nPitches %=> p;
    // cycle instruments through full array
    nInstruments %=> i;
    
    // advance time by interOnsetInterval and calculate the next interval
    interOnsetInterval => now;
    // accelerate
    if (interOnsetInterval > minInterOnsetInterval) 
        interOnsetInterval * 0.78 => interOnsetInterval;
    else
        // can't go faster than minInterOnsetInterval 
        minInterOnsetInterval => interOnsetInterval;
}

// -------------------------------------------------------------
// @class FMFS
// fm implementation from scratch with envelopes
// @author 2015 Madeline Huberth, 2021 version by CC
class FMFS
{ // two typical uses of the ADSR envelope unit generator...
    Step unity => ADSR envM => blackhole; //...as a separate signal
    SinOsc mod => blackhole;
    SinOsc car => ADSR envC => Gain out;  //...as an inline modifier of a signal
    car.gain(0.3);
    float freq, index, ratio; // the parameters for our FM patch
    
    fun void fm() // this patch is where the work is
    {
        while (true)
        {
            envM.last() * index => float currentIndex; // time-varying index
            mod.gain( freq * currentIndex );    // modulator gain (index depends on frequency)
            mod.freq( freq * ratio );           // modulator frequency (a ratio of frequency) 
            car.freq( freq + mod.last() );      // frequency + modulator signal = FM 
            1::samp => now;
        }
    }
    spork ~fm(); // run the FM patch
    
    // function to play a note on our FM patch
    fun void playFM( dur length, float pitch, float pitchRatio, float carrierADSR[], float modulationRatio, float mGain, float modulatorADSR[] ) 
    {
        // set patch values
        pitchRatio * pitch => freq;
        modulationRatio => ratio;
        mGain => index;
        // run the envelopes
        spork ~ playEnv( envC, length, carrierADSR );
        spork ~ playEnv( envM, length, modulatorADSR );
        length => now; // wait until the note is done
    }
    
    fun void playEnv( ADSR env, dur length, float adsrValues[] )
    {
        // set values for ADSR envelope depending on length
        length * adsrValues[0] => dur A;
        length * adsrValues[1] => dur D;
        adsrValues[2] => float S;
        length * adsrValues[3] => dur R;
        
        // set up ADSR envelope for this note
        env.set( A, D, S, R );
        // start envelope (attack is first segment)
        env.keyOn();
        // wait through A+D+S, before R
        length-env.releaseTime() => now;
        // trigger release segment
        env.keyOff();
        // wait for release to finish
        env.releaseTime() => now;
    }
    
} 
// END OF CLASS: FM