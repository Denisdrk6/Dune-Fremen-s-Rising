using System;
using RagnarEngine;

[System.Serializable]
public class EnemyData
{
    public EnemyType type;
    public EnemyState state;

    public EnemyData(Enemies enemy)
    {
        state = enemy.state;
        type = enemy.type;
    }

}