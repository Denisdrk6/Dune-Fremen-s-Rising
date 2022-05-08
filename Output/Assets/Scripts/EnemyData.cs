using System;
using RagnarEngine;

[System.Serializable]
public class EnemyData
{
    public EnemyType type;
    public EnemyState state;
    public float[] position;

    public EnemyData(Enemies enemy)
    {
        position = new float[3];
        position[0] = enemy.pos.x;
        position[1] = enemy.pos.y;
        position[2] = enemy.pos.z;

        state = enemy.state;
        type = enemy.type;
    }

}