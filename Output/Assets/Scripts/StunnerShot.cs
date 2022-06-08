using System;
using RagnarEngine;

public class StunnerShot : RagnarComponent
{
    private float force = 2500;
    private bool pendingToDelete = false;
    private GameObject sceneAudio;
    GameObject stunner;
    public void Start()
    {
        sceneAudio = GameObject.Find("AudioLevel1");
        sceneAudio.GetComponent<AudioSource>().PlayClip("WPN_STUNNERGUNSHOT");
        AimMethod();

        stunner = GameObject.Find("Stunner");
        stunner.isActive = true;
        stunner.GetComponent<Animation>().PlayAnimation("Attack");
    }

    private void AimMethod()
    {
        GameObject player = GameObject.Find("Player_3");
        NavAgent agent = player.GetComponent<NavAgent>();

        Vector3 pos = player.transform.globalPosition;
        pos.y += 1;
        gameObject.transform.localPosition = pos;

        Vector3 direction = HitEnemy(agent, player);

        Rigidbody goRB = gameObject.GetComponent<Rigidbody>();
        goRB.SetBodyPosition(pos);
        goRB.IgnoreCollision(player, true);
        goRB.ApplyCentralForce(direction.normalized * force);

        agent.hitPosition = player.transform.globalPosition;

        Vector3 newForward = direction.normalized;
        double angle = Math.Atan2(newForward.x, newForward.z);
        Quaternion rot = new Quaternion(0, (float)(1 * Math.Sin(angle / 2)), 0, (float)Math.Cos(angle / 2));
        player.GetComponent<Rigidbody>().SetBodyRotation(rot);
        player.GetComponent<Animation>().PlayAnimation("Ability2");
    }

    public void Update()
    {
        if (pendingToDelete) InternalCalls.Destroy(gameObject);
    }

    public void OnCollisionEnter(Rigidbody other)
    {
        sceneAudio.GetComponent<AudioSource>().PlayClip("WPN_STUNNERHIT");
        pendingToDelete = true;
    }

    private Vector3 HitEnemy(NavAgent agent, GameObject player)
    {
        GameObject obj = RayCast.HitToTag(agent.rayCastA, agent.rayCastB, "Enemies");

        if (obj != null)
        {
            //Debug.Log(obj.name.ToString());
            return obj.transform.globalPosition - player.transform.globalPosition;
        }

        return agent.hitPosition - player.transform.globalPosition;
    }
}