using System;
using RagnarEngine;

public class StunnerShot : RagnarComponent
{
    private float force = 2500;
    private bool pendingToDelete = false;

    public void Start()
    {
        AimMethod();
    }

    private void AimMethod()
    {
        GameObject player = GameObject.Find("Player_3");
        NavAgent agent = player.GetComponent<NavAgent>();

        Vector3 pos = player.transform.globalPosition;
        pos.y += 1;
        gameObject.transform.localPosition = pos;

        Vector3 direction = HitEnemy(agent, player);
        direction.y = 0;

        Rigidbody goRB = gameObject.GetComponent<Rigidbody>();
        goRB.SetBodyPosition(pos);
        goRB.IgnoreCollision(player, true);
        goRB.ApplyCentralForce(direction.normalized * force);

        agent.hitPosition = player.transform.globalPosition;
    }

    public void Update()
    {
        if (pendingToDelete) InternalCalls.Destroy(gameObject);
    }

    public void OnCollisionEnter(Rigidbody other)
    {
        pendingToDelete = true;
    }

    private Vector3 HitEnemy(NavAgent agent, GameObject player)
    {
        GameObject obj = RayCast.HitToTag(agent.rayCastA, agent.rayCastB, "Enemies");

        if (obj != null)
        {
            Debug.Log(obj.name.ToString());
            return obj.GetComponent<Transform>().globalPosition - player.transform.globalPosition;
        }

        return agent.hitPosition - player.transform.globalPosition;
    }
}