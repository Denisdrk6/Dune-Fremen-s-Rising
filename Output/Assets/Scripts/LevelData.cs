using System;
using RagnarEngine;

[System.Serializable]
public class LevelData
{
    public float timer;
    public float[] posCam;
    public float[] rotCam;
    public float angle;
    public bool[] abilities;

    public LevelData(float t, Vector3 pos, Quaternion rot, float horizontalAngle, bool[] ret)
    {
        timer = t;
        posCam = new float[3];
        posCam[0] = pos.x;
        posCam[1] = pos.y;
        posCam[2] = pos.z;

        rotCam = new float[4];
        rotCam[0] = rot.x;
        rotCam[1] = rot.y;
        rotCam[2] = rot.z;
        rotCam[3] = rot.w;

        angle = horizontalAngle;
        abilities = ret;
    }
}