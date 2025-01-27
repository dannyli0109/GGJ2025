using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
	public class Math2D
	{
		//public static float AirDragDistanceToSpeed(float distance, float k, float approximate = 0.01f)
		//{
		//	return approximate / Mathf.Exp(-drag * distance);
		//}

		//public static float AirDragTimeToSpeed(float time, float k, float approximate = 0.01f)
		//{
		//	return approximate * Mathf.Exp(-k * time);
		//}

		//public static float AirDragTimeToDistance(float time, float k, float approximate = 0.01f)
		//{
		//	return approximate * Mathf.Exp(-k * time);
		//}

		public static float CalAirDragFactor(float distance, float speed, float approximate = 0.01f)
		{
			return speed / distance;
		}

		public static float CalAirDrag(float speed, float factor)
		{
			return factor * speed;
		}
	}
}