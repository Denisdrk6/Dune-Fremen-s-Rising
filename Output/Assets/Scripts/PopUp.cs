using System;
using RagnarEngine;

public class PopUp : RagnarComponent
{
	public GameObject target;
	public void Start()
	{

	}
	public void Update()
	{
		for (int i = 0; i < gameObject.childs.Length; i++)
		{
			Vector3 pos;
			if (i == 0)
				pos = Camera.WorldToScreen(target.transform.globalPosition) + new Vector3(0, 60, 0);
			else
				pos = Camera.WorldToScreen(target.transform.globalPosition) + new Vector3(0, 75, 0);
			gameObject.childs[i].GetComponent<Transform2D>().position2D = pos;
		}
	}

}