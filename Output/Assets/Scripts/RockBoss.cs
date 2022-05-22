using System;
using RagnarEngine;

public class RockBoss : RagnarComponent
{
    private GameObject player;
    float deathTimer = 6.0f;
    public void Start()
	{
        player = GameObject.Find("Player");

        gameObject.GetComponent<ParticleSystem>().Pause();
    }

    public void Update()
    {
        if (deathTimer >= 0)
        {
            deathTimer -= Time.deltaTime;
            if (deathTimer <= 0)
            {
                deathTimer = -1f;
                InternalCalls.Destroy(gameObject);
            }
        }
    }
	
    public void OnCollision (Rigidbody other)
    {
        if (other.gameObject == player)
        {
            InternalCalls.Destroy(gameObject);
        }
    }
}