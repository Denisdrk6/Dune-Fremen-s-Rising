using System;
using RagnarEngine;

[System.Serializable]
public class PlayerData
{
    public int hitPoints;
    public float[] position;
    public float[] rotation;

    public PlayerData (Player player)
    {
        hitPoints = player.hitPoints;
        position = new float[3];
        position[0] = player.gameObject.transform.globalPosition.x;
        position[1] = player.gameObject.transform.globalPosition.y;
        position[2] = player.gameObject.transform.globalPosition.z;

        rotation = new float[4];
        rotation[0] = player.gameObject.transform.globalRotation.x;
        rotation[1] = player.gameObject.transform.globalRotation.y;
        rotation[2] = player.gameObject.transform.globalRotation.z;
        rotation[3] = player.gameObject.transform.globalRotation.w;
    }

}