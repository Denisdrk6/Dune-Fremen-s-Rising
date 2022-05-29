using System;
using RagnarEngine;

[System.Serializable]
public class GameData
{
    public bool vsync = false;
    public bool shadowsEnabled = false;
    public bool fullScreen = false;
    public int language = 0;
    public int actualControlOption = 1;

    public GameData(bool vs, bool shadows, bool fs, int languageType, int controlPresset)
    {
        vsync = vs;
        shadowsEnabled = shadows;
        fullScreen = fs;
        language = languageType;
        actualControlOption = controlPresset;
    }

}