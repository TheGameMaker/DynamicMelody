using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Audio;
using UnityEngine.UI;
using Assets.Scripts.WasapiAudio;

[RequireComponent (typeof (AudioSource))]
public class AudioVisualizer : MonoBehaviour {
	AudioSource audiosource;

    //Microphone input
    [SerializeField] private AudioClip audioClip;
    public bool useMicrophone;
    public static string selectedDevice;
    [SerializeField] private AudioMixerGroup mixerGroupMicrophone, mixerGroupMaster;

    public Settings settings;

    public bool streamAudio;
    WasapiAudioSource wasapiSource;
    AudioVisualizationProfile profile;
    AudioVisualizationStrategy strategy = AudioVisualizationStrategy.Raw;

    //FFT values
   // public float[] freqBandHighest = new float[8]; //might not need
  //  public static float[] samples = new float[512];
    public static float[] samplesLeft = new float[512];
    public static float[] samplesRight = new float[512];
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
    public float audioProfile;

    public enum channel { Stereo, Left, Right };
    public channel Channel = new channel();

	// Use this for initialization
	void Awake () {
        audiosource = GetComponent<AudioSource>();
        wasapiSource = GetComponent<WasapiAudioSource>();
        Application.runInBackground = true;
        AudioProfile(audioProfile);
        settings = new SettingsPlayerPref();
        settings.defaultSettings();
        if (streamAudio)
        {
            wasapiSource.Awake();
            Debug.Log("Got Here");
        }
        else
        {
            setSound(settings.getOptionBool(Option.Microphone));
        }
        
        //setSound(useMicrophone);
    }

    public void setClip()
    {
        string p = EditorUtility.OpenFilePanel("Select WAV file", "", "wav");
        WWW audioLoader = new WWW("file://" + p);
        while (!audioLoader.isDone)
            System.Threading.Thread.Sleep(100);

        Debug.Log(audioLoader.GetAudioClip().name);
        audioClip = audioLoader.GetAudioClip();
        Debug.Log(audioClip);
        setSound(useMicrophone);
      
    }

    public void setSound(bool useMic)
    {
        //Microphone Input
        useMicrophone = useMic;
        audiosource.Stop();
        //Destroy(audiosource.clip);
        resetAmplitude();
        if (useMicrophone)
        {
            if (Microphone.devices.Length > 0)
            {
                
                selectedDevice = Microphone.devices[0].ToString(); //Change this to select different devices
                audiosource.outputAudioMixerGroup = mixerGroupMicrophone;
                audiosource.clip = Microphone.Start(selectedDevice, false, 3500, AudioSettings.outputSampleRate);
            }
            else//if  no microphone devices
            {
                useMicrophone = false;
            }
        }
        else if (!useMicrophone)
        {

            Microphone.End(selectedDevice);     
            audiosource.outputAudioMixerGroup = mixerGroupMaster;
            audiosource.clip = audioClip;//example could put microphone not working audio here
        }
        settings.setOptionBool(Option.Microphone, useMicrophone);
        audiosource.Play();
    }
	
	// Update is called once per frame
	void Update () {
        if (streamAudio)
        {
            GetSpectrumStream();
        }
        else
        {
            GetSpectrumAudioSource();
        }
        MakeFrequencyBands();
        BandBuffer();
        CreateAudioBands();
        GetAmplitude();
	}

    void resetAmplitude()
    {
        Debug.Log("reset");
        amplitudeHigh = -2147483648;
        samplesLeft = new float[512];
        samplesRight = new float[512];
        freqBand = new float[8];
        bandBuffer = new float[8];
        bufferDecrease = new float[8];

        freqBandHigh = new float[8];

    //audio band values
        audioBand = new float[8];
        audioBandBuffer = new float[8];
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
        //freqBandHighest = freqBandHigh;
    }

    void GetSpectrumStream()
    {
        samplesLeft = wasapiSource.GetSpectrumData(strategy, false, profile);
    }
    void GetSpectrumAudioSource()
    {
       // audiosource.GetSpectrumData(samples, 0, FFTWindow.Blackman);
        audiosource.GetSpectrumData(samplesLeft, 0, FFTWindow.Blackman);
        audiosource.GetSpectrumData(samplesRight, 1, FFTWindow.Blackman);
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

    void AudioProfile(float audioProfile)
    {
        for(int i = 0; i < 8; i++)
        {
            freqBandHigh[i] = audioProfile;
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
                if (Channel == channel.Stereo)
                {
                    average += samplesLeft[count] + samplesRight[count] * (count + 1);
                    count++;
                }
                if (Channel == channel.Left)
                {
                    average += samplesLeft[count] * (count + 1);
                    count++;
                }
                if (Channel == channel.Right)
                {
                    average += samplesRight[count] * (count + 1);
                    count++;
                }
            }

            average /= count;
            freqBand[i] = average * 10;
        }
    }
}
