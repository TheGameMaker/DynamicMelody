using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SettingsPlayerPref : Settings
{ 
    public string getOptionString(Option o)
    {
        return PlayerPrefs.GetString(o.ToString());
    }
    public void setOptionString(Option o, string s)
    {
        PlayerPrefs.SetString(o.ToString(), s);
    }
    public bool getOptionBool(Option o)
    {
        int v = PlayerPrefs.GetInt(o.ToString());
        return v == 1 ? true : false;
    }
    public void setOptionBool(Option o, bool b)
    {
        int v = b ? 1 : 0;
        PlayerPrefs.SetInt(o.ToString(), v);
    }
    public int getOptionInt(Option o)
    {
        return PlayerPrefs.GetInt(o.ToString());
    }
    public void setOptionInt(Option o, int i)
    {
        PlayerPrefs.SetInt(o.ToString(), i);
    }
    public float getOptionFloat(Option o)
    {
        return PlayerPrefs.GetFloat(o.ToString());
    }
    public void setOptionFloat(Option o, float f)
    {
        PlayerPrefs.SetFloat(o.ToString(), f);
    }
    public bool saveSettings()
    {
        PlayerPrefs.Save();
        return true;
    }
    public bool loadSettings()
    {
        return true;
    }
    
    public void defaultSettings()
    {
        setOptionInt(Option.Version, 1);
        setOptionBool(Option.Microphone, true);
    }
}
