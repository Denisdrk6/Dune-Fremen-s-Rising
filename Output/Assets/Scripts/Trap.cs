using System;
using RagnarEngine;

public class Trap : RagnarComponent
{
    public bool placingTrap = true;
    public float timer = 4f;
    public bool canReload = false;
	public bool pendingToDelete = false;

    GameObject player;
    PlayerManager playerManagerScript;

	public void Start()
	{
		player = GameObject.Find("Player_3");

		Vector3 pos = player.transform.globalPosition;

        gameObject.GetComponent<Rigidbody>().SetBodyPosition(pos);
		gameObject.transform.localPosition = pos;

        playerManagerScript = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();
        GameObject.Find("ElectricParticles").GetComponent<ParticleSystem>().Pause();
        player.GetComponent<Player>().PlayAudioClip("WPN_TRAPACTIVE");
    }
	public void Update()
	{
        if (!canReload) ReloadCondition();
        if (canReload) ReloadTrap();
        if (pendingToDelete) InternalCalls.Destroy(gameObject);
    }

    private void ReloadCondition()
    {
        double distance = CalculateDistance();

        if (distance > 2.0f)
        {
            canReload = true;
        }
    }
	private void ReloadTrap()
    {
        double distance = CalculateDistance();

        if (distance < 1.0f)
        {
            playerManagerScript.characters[2].abilities[2].charges = 1;
            pendingToDelete = true;
            canReload = false;
        }
    }

    private double CalculateDistance()
    {
        float xDiff = player.transform.globalPosition.x - gameObject.transform.globalPosition.x;
        float zDiff = player.transform.globalPosition.z - gameObject.transform.globalPosition.z;
        double distance = Math.Sqrt(zDiff * zDiff + xDiff * xDiff);

        return distance;
    }
}