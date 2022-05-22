using System;
using System.Collections.Generic;
using RagnarEngine;

public enum EnemyType
{
    BASIC,
    TANK,
    UNDISTRACTABLE,
    AIR
}

public enum EnemyState
{
    IDLE,
    PATROLING,
    CAUTIOUS,
    SEARCHING,
    ANGRY,
    DEATH
}

public class Enemies
{
    // Basic Enemy info
    public string name;
    public EnemyType type;
    public EnemyState state;

    // Position
    public GameObject spawnPoint;

    // Patroling
    public List<GameObject> waypoints = new List<GameObject>();

    // Abilities
    public EnemyAbilities[] abilities;

    public bool coneRotate = true;
}