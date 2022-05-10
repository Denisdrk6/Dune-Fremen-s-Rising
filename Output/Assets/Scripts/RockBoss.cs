using System;
using RagnarEngine;

public class RockBoss : RagnarComponent
{
    private float force = 1500.0f;

    float deathTimer = -1f;
    public void Start()
	{
        GameObject boss = GameObject.Find("Boss");
        NavAgent agent = boss.GetComponent<NavAgent>();

        Vector3 pos = boss.transform.globalPosition;
        pos.y += 1;
        gameObject.transform.localPosition = pos;

        Vector3 direction = agent.hitPosition - boss.transform.globalPosition;
        direction.y = 0;

        Rigidbody goRB = gameObject.GetComponent<Rigidbody>();
        goRB.SetBodyPosition(pos);
        goRB.IgnoreCollision(boss, true);
        goRB.ApplyCentralForce(direction.normalized * force);

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
        if (deathTimer == -1)
        {
            deathTimer = 0.3f;
            gameObject.GetComponent<ParticleSystem>().Play();
        }
    }
}