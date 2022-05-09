using System;
using RagnarEngine;

public class SoundAreaManager : RagnarComponent
{
	bool timerDestroy = false;
	float timer = 0f;
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
			if (timer > 3f)
				InternalCalls.Destroy(gameObject);
		}
	}
}