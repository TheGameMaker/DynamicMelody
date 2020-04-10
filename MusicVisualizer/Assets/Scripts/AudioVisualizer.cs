using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent (typeof (AudioSource))]
public class AudioVisualizer : MonoBehaviour {
	AudioSource audiosource;

    //Microphone input
    [SerializeField] private AudioClip audioClip;
    [SerializeField] private bool useMicrophone;
    [SerializeField] private string selectedDevice;
    [SerializeField] private AudioMixerGroup mixerGroupMicrophone, mixerGroupMaster;

    

    //FFT values
    public static float[] samples = new float[512];
     float[] freqBand = new float[8];
     float[] bandBuffer = new float[8];
    float[] bufferDecrease = new float[8];
   
    float[] freqBandHigh = new float[8];

    //audio band values
    public static float[] audioBand = new float[8];
    public static float[] audioBandBuffer = new float[8];

    //amplitude variables
    public static float amplitude, amplitudeBuffer;
    float amplitudeHigh;

	// Use this for initialization
	void Start () {
        audiosource = GetComponent<AudioSource>();
        
        //Microphone Input
        if (useMicrophone)
        {
            if(Microphone.devices.Length > 0)
            {
                selectedDevice = Microphone.devices[0].ToString(); //Change this to select different devices
                audiosource.outputAudioMixerGroup = mixerGroupMicrophone;
                audiosource.clip = Microphone.Start(selectedDevice, true, 3599, AudioSettings.outputSampleRate);
                audiosource.Play();
            }
            else
            {
                useMicrophone = false;
            }
        }else if (!useMicrophone)
        {
            audiosource.outputAudioMixerGroup = mixerGroupMaster;
            audiosource.clip = audioClip;//example could put microphone not working audio here
            audiosource.Play();
        }
        //audiosource.Play();
	}
	
	// Update is called once per frame
	void Update () {
        GetSpectrumAudioSource();
        MakeFrequencyBands();
        BandBuffer();
        CreateAudioBands();
        GetAmplitude();
	}

    void GetAmplitude()
    {
        float currentAmplitude = 0;
        float currentAmplitudeBuffer = 0;
        for(int i = 0; i < 8; i++)
        {
            currentAmplitude += audioBand[i];
            currentAmplitudeBuffer += audioBandBuffer[i];
        }
        if (currentAmplitude > amplitudeHigh)
        {
            amplitudeHigh = currentAmplitude;
        }
        amplitude = currentAmplitude / amplitudeHigh;
        amplitudeBuffer = currentAmplitudeBuffer / amplitudeHigh;
    }

    void CreateAudioBands()
    {
        for(int i = 0; i < 8; i++)
        {
            if(freqBand[i] > freqBandHigh[i])
            {
                freqBandHigh[i] = freqBand[i];
            }
            audioBand[i] = (freqBand[i] / freqBandHigh[i]);
            audioBandBuffer[i] = (bandBuffer[i] / freqBandHigh[i]);
        }
    }

    void GetSpectrumAudioSource()
    {
        audiosource.GetSpectrumData(samples, 0, FFTWindow.Blackman);
    }

    void BandBuffer()
    {
        for (int i = 0; i < 8; i++)
        {
            if (freqBand[i] > bandBuffer[i])
            {
                bandBuffer[i] = freqBand[i];
                bufferDecrease[i] = 0.005f;
            }

            if (freqBand[i] < bandBuffer[i])
            {
                bandBuffer[i] -= bufferDecrease[i];
                bufferDecrease[i] *= 1.2f;
            }
        }
    }

    void MakeFrequencyBands()
    {
        /*
         * 22050 / 512 = 43 hertz per sample
         * 
         * 20 - 60 hertz
         * 60 - 250 hertz
         * 250 - 500 hertz
         * 500 - 2000 hertz
         * 2000 - 4000 hertz
         * 4000 - 6000 hertz
         * 6000 - 20000 hertz
         * 
         * 0 - 2 = 86 hertz (2 * 43 hertz per sample)
         * 1 - 4 = 172 hertz - 87-258
         * 2 - 8 = 344 hertz - 259-602
         * 3 - 16 = 688 hertz - 603-1290
         * 4 - 32 = 1376 hertz - 1291-2666
         * 5 - 64 = 2752 hertz - 2667-5418
         * 6 - 128 = 5584 hertz - 5419-10922
         * 7 - 256 = 11008 hertz - 10923-21950
         * 510 samples must add 2 to the end to reach 512
         */

        int count = 0;

        for (int i = 0; i < 8; i++)
        {
            float average = 0;
            int sampleCount = (int)Mathf.Pow(2, i) * 2;

            //adds 2 to the end
            if(i == 7)
            {
                sampleCount += 2;
            }
            for (int j = 0; j < sampleCount; j++)
            {
                average += samples[count] * (count + 1);
                count++;
            }

            average /= count;
            freqBand[i] = average * 10;
        }
    }
}
