using System;
using RagnarEngine;

public class SoundAreaManager : RagnarComponent
{
	public float stablishedTimer = 3f;
	private bool timerDestroy = false;
	private float timer = 0f;

	public void Start()
	{
		if (gameObject.GetParent().name == SceneManager.currentSceneName)
			timerDestroy = true;
	}

    public void Update()
	{
		if (timerDestroy)
		{
			timer += Time.deltaTime;
			if (timer > stablishedTimer)
				InternalCalls.Destroy(gameObject);
		}
	}
}