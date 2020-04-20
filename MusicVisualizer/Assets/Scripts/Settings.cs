using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Option
{
    Microphone, //bool
                //add other options here.
}

public interface Settings
{


    string getOptionString(Option o);
    void setOptionString(Option o, string s);
    bool getOptionBool(Option o);
    void setOptionBool(Option o, bool b);
    int getOptionInt(Option o);
    void setOptionInt(Option o, int i);
    float getOptionFloat(Option o);
    void setOptionFloat(Option o, float f);
    bool saveSettings();
    bool loadSettings();
    void defaultSettings();

}
