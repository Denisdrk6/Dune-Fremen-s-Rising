using System;
using RagnarEngine;

public class Knife : RagnarComponent
{
	public GameObject player;
	// Components
	private AudioSource audioSourceComponent;
	private ParticleSystem particleComponent;

	private float force = 1700;
	public bool canReload;
	private bool pendingToDelete;
	private bool grabOnce;
	GameObject circle;
	GameObject popUp;
	
	public void Start()
    {
		canReload = false;
		grabOnce = false;
		pendingToDelete = false;
        player = GameObject.Find("Player");
        circle = GameObject.Find("circle_ability");
		circle.isInteractuable = false;

		// Get components
		particleComponent = gameObject.GetComponent<ParticleSystem>();
		audioSourceComponent = gameObject.GetComponent<AudioSource>();

        AimMethod();

		particleComponent.Play();

		//gameObject.SubmitOutlineDrawing(new Vector3(0.3f, 0.95f, 0.15f));
	}

    private void AimMethod()
    {
        NavAgent agent = player.GetComponent<NavAgent>();

        Vector3 pos = player.transform.globalPosition;
        pos.y += 1;
        gameObject.transform.localPosition = pos;

        Vector3 direction = HitEnemy(agent, player);
		Debug.Log("A");
		if(direction != null)
        {
			player.GetComponent<Player>().PlayAudioClip("WPN_THORWINGKNIFETHROW");
			//player.GetComponent<Animation>().PlayAnimation("Ability4");

			Rigidbody goRB = gameObject.GetComponent<Rigidbody>();
			goRB.SetBodyPosition(pos);

			Vector3 newForward = direction.normalized;
			double angle = Math.Atan2(newForward.x, newForward.z);
			Quaternion rot = new Quaternion(0, (float)(1 * Math.Sin(angle / 2)), 0, (float)Math.Cos(angle / 2));
			goRB.SetBodyRotation(rot);
			player.GetComponent<Rigidbody>().SetBodyRotation(rot);

			gameObject.isInteractuable = true;
			gameObject.interactuableColor = new Vector3(0, 1, 0);

			goRB.IgnoreCollision(player, true);
			goRB.ApplyCentralForce(newForward * force);
		}
        else
        {
			pendingToDelete = true;
			GameObject.Find("PlayerManager").GetComponent<PlayerManager>().characters[0].abilities[2].charges += 1;
        }

 		agent.hitPosition = player.transform.globalPosition;
	}

    public void Update()
	{
		if (canReload) ReloadKnife();
		if (pendingToDelete)
		{
			//gameObject.UnSubmitOutlineDrawing();
			if(popUp != null) InternalCalls.Destroy(popUp);
			InternalCalls.Destroy(gameObject);
		}
	}
	private void ReloadKnife()
	{
		GameObject player = GameObject.Find("Player");

		float xDiff = player.transform.globalPosition.x - gameObject.transform.globalPosition.x;
		float zDiff = player.transform.globalPosition.z - gameObject.transform.globalPosition.z;
		double distance = Math.Sqrt(zDiff * zDiff + xDiff * xDiff);

		if (distance < 1.0f)
		{
			if (!grabOnce)
			{
				grabOnce = true;
				GameObject.Find("PlayerManager").GetComponent<PlayerManager>().characters[0].abilities[2].counter = GameObject.Find("PlayerManager").GetComponent<PlayerManager>().characters[0].abilities[2].cooldown;
				GameObject.Find("PlayerManager").GetComponent<PlayerManager>().characters[0].abilities[2].charges += 1;
				audioSourceComponent.PlayClip("WPN_THORWINGKNIFERECOVERSAND");
			}

			pendingToDelete = true;
			canReload = false;
			grabOnce = false;
		}
	}

	private Vector3 HitEnemy(NavAgent agent, GameObject player)
	{
		GameObject enemy = RayCast.HitToTag(agent.rayCastA, agent.rayCastB, "Enemies");
		if (enemy != null && Transform.GetDistanceBetween(player.transform.globalPosition, enemy.transform.globalPosition) < 15)
		{
			switch (enemy.GetComponent<BasicEnemy>().state)
			{
				case EnemyState.DEATH:
					GameObject.Find("PlayerManager").GetComponent<PlayerManager>().characters[0].abilities[2].counter = GameObject.Find("PlayerManager").GetComponent<PlayerManager>().characters[0].abilities[2].cooldown;
					return null;
				case EnemyState.IS_DYING:
					GameObject.Find("PlayerManager").GetComponent<PlayerManager>().characters[0].abilities[2].counter = GameObject.Find("PlayerManager").GetComponent<PlayerManager>().characters[0].abilities[2].cooldown;
					return null;
				default:
					return enemy.transform.globalPosition - player.transform.globalPosition;
			}
		}
		GameObject.Find("PlayerManager").GetComponent<PlayerManager>().characters[0].abilities[2].counter = GameObject.Find("PlayerManager").GetComponent<PlayerManager>().characters[0].abilities[2].cooldown;
		return null;
	}

	public void OnCollision(Rigidbody other)
	{		
		if(other.gameObject.tag == "Ground")
        {
			gameObject.DeleteComponent<Rigidbody>(gameObject.GetComponent<Rigidbody>());
			circle.isActive = true;
			gameObject.transform.globalRotation = Quaternion.identity;
			particleComponent.Pause();
			popUp = InternalCalls.InstancePrefab("PopUp", Vector3.one);
			popUp.GetComponent<PopUp>().target = gameObject;
			popUp.childs[1].GetComponent<UIImage>().SetImageGeneralColor(11, 212, 0);
			popUp.childs[2].GetComponent<UIImage>().LoadTexture("Assets/Resources/UI/ui_chani_crysknife.png");
		}
        else
        {
			particleComponent.Pause();
			canReload = true;
			GameObject.Find("Quest System").GetComponent<QuestSystem>().enemiesThrowingKnife++;
		}
	}

}


