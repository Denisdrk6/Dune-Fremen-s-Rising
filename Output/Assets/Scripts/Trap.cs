using System;
using RagnarEngine;

public class Trap : RagnarComponent
{
    public bool placingTrap = true;
    public float timer = 4f;
    public bool canReload = false;
	public bool pendingToDelete = false;

	public void Start()
	{
		GameObject player = GameObject.Find("Player_3");
		Vector3 pos = player.transform.globalPosition;
		pos.y += gameObject.transform.globalPosition.y;
		gameObject.transform.localPosition = pos;

        GameObject.Find("ElectricParticles").GetComponent<ParticleSystem>().Pause();
    }
	public void Update()
	{
        if (placingTrap)
        {
            if (timer > 0f)
            {
                GameObject.Find("PlayerManager").GetComponent<PlayerManager>().characters[2].state = State.POSTCAST;
                timer -= Time.deltaTime;
            }
            else
            {
                placingTrap = false;
                GameObject.Find("PlayerManager").GetComponent<PlayerManager>().characters[2].state = State.NONE;
            }
        }
        if (!placingTrap && !canReload) ReloadCondition();
		if (canReload) ReloadTrap();
		if (pendingToDelete) InternalCalls.Destroy(gameObject);

	}

    private void ReloadCondition()
    {
        double distance = CalculateDistance();

        if (distance > 3.0f)
        {
            canReload = true;
        }
    }
	private void ReloadTrap()
    {
        double distance = CalculateDistance();

        if (distance < 1.0f)
        {
            GameObject.Find("PlayerManager").GetComponent<PlayerManager>().characters[2].abilities[2].charges = 1;
            pendingToDelete = true;
            canReload = false;
        }
    }

    private double CalculateDistance()
    {
        GameObject player = GameObject.Find("Player_3");

        float xDiff = player.transform.globalPosition.x - gameObject.transform.globalPosition.x;
        float zDiff = player.transform.globalPosition.z - gameObject.transform.globalPosition.z;
        double distance = Math.Sqrt(zDiff * zDiff + xDiff * xDiff);

        return distance;
    }
}