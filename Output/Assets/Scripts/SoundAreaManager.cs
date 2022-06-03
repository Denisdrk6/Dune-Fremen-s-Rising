using System;
using System.Collections.Generic;
using RagnarEngine;

public class SoundAreaManager : RagnarComponent
{
	public float stablishedTimer = 3f;
	private bool timerDestroy = false;
	private float timer = 0f;

	public void Start()
	{
		Light light = gameObject.childs[0].GetComponent<Light>();

		light.intensity = 1f;
		light.constant = 0.15f;
		light.linear = -0.2f;
		light.quadratic = 0f;

		if (gameObject.GetParent().name == SceneManager.currentSceneName)
			timerDestroy = true;
	}

    public void Update()
	{
		if (timerDestroy)
		{
			timer += Time.deltaTime;
			if (timer > stablishedTimer)
			{
				gameObject.childs[0].GetComponent<Light>().intensity = 0f;
				InternalCalls.Destroy(gameObject);
			}
		}
	}

	public void UpdateRadius(float radius)
    {
		gameObject.GetComponent<Rigidbody>().SetRadiusSphere(radius);

		if (radius <= 0)
			gameObject.childs[0].GetComponent<Light>().intensity = 0;
		else
		{
			gameObject.childs[0].GetComponent<Light>().intensity = 1f;
			gameObject.childs[0].GetComponent<Light>().linear = -1.1f / radius;
		}

	}
}