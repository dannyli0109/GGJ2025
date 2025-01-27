using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class ManagerCreater : SingletonMono<ManagerCreater>
{
	public List<GameObject> managers;

	protected override void Init()
	{
		foreach (var obj in managers)
		{
			Instantiate(obj, transform);
		}
	}
}
