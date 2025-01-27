using System;

namespace Utils
{
	/// <summary>
	/// Easing Lerp Implmentation to create effective lerp functions 
	/// </summary>
	public class EasingLerps
	{
		/// <summary>
		/// The in and out of lerp function 
		/// </summary>
		public enum EasingInOutType
		{
			EaseIn,
			EaseOut,
			EaseInOut
		}

		/// <summary>
		/// The type of ease function
		/// </summary>
		public enum EasingLerpsType
		{
			Sine,
			Quad,
			Cubic,
			Quint,
			Expo,
			Circ,
			Back,
			Elastic,
			Bounce
		}

		/// <summary>
		/// Easy use of all Easing lerp function   
		/// </summary>
		/// <param name="inOutType">Which type of effect</param>
		/// <param name="lerpType">The type of Lerp</param>
		/// <param name="time">Current time</param>
		/// <param name="a">Starting Value</param>
		/// <param name="b">Ending Value</param>
		/// <param name="totalTime">Lerp total time</param>
		/// <returns>Lerp Value</returns>
		public static float EasingLerp(EasingInOutType inOutType, EasingLerpsType lerpType, float time, float a = 0, float b = 1, float totalTime = 1)
		{
			float v = 0;
			switch (lerpType)
			{
				case EasingLerpsType.Sine:
					v = EaseSine(inOutType, time, totalTime);
					break;
				case EasingLerpsType.Quad:
					v = EaseQuad(inOutType, time, totalTime);
					break;
				case EasingLerpsType.Cubic:
					v = EaseCubic(inOutType, time, totalTime);
					break;
				case EasingLerpsType.Quint:
					v = EaseQuint(inOutType, time, totalTime);
					break;
				case EasingLerpsType.Expo:
					v = EaseExpo(inOutType, time, totalTime);
					break;
				case EasingLerpsType.Circ:
					v = EaseCirc(inOutType, time, totalTime);
					break;
				case EasingLerpsType.Back:
					v = EaseBack(inOutType, time, totalTime);
					break;
				case EasingLerpsType.Elastic:
					v = EaseElastic(inOutType, time, totalTime);
					break;
				case EasingLerpsType.Bounce:
					v = EaseBounce(inOutType, time, totalTime);
					break;
				default:
					v = EaseSine(inOutType, time, totalTime);
					break;
			}
			return a + (b - a) * v;
		}

		//////////////////////////////////////////////
		// Sine Function
		/////////////////////////////////////////////
		#region Sine

		/// <summary>
		/// 
		/// </summary>
		/// <param name="inOutType">How should use sine lerp </param>
		/// <param name="time">Current Time of lerp</param>
		/// <param name="totalTime">Total time between lerps</param>
		/// <returns>Lerp Value</returns>
		static float EaseSine(EasingInOutType inOutType, float time, float totalTime = 1.0f)
		{
			switch (inOutType)
			{
				case EasingInOutType.EaseIn:
					{
						return EaseInSine(time, totalTime);
					}
				case EasingInOutType.EaseOut:
					{
						return EaseOutSine(time, totalTime);
					}
				case EasingInOutType.EaseInOut:
					{
						return EaseInOutSine(time, totalTime);
					}
				default:
					{
						return EaseInSine(time, totalTime);
					}
			}
		}

		/// <summary>
		/// Ease In Sine
		/// </summary>
		/// <param name="time">Current Time of lerp</param>
		/// <param name="totalTime">Total time between lerps</param>
		/// <returns>Lerp Value</returns>
		static float EaseInSine(float time, float totalTime = 1.0f)
		{

			return 1 - (float)Math.Cos(time / totalTime * (Math.PI / 2));
		}


		/// <summary>
		/// Ease out Sine
		/// </summary>
		/// <param name="time">Current Time of lerp</param>
		/// <param name="totalTime">Total time between lerps</param>
		/// <returns>Lerp Value</returns>
		static float EaseOutSine(float time, float totalTime = 1.0f)
		{

			return (float)Math.Sin(time / totalTime * (Math.PI / 2));
		}



		/// <summary>
		/// Ease int out Sine
		/// </summary>
		/// <param name="time">Current Time of lerp</param>
		/// <param name="totalTime">Total time between lerps</param>
		/// <returns>Lerp Value</returns>
		static float EaseInOutSine(float time, float totalTime = 1.0f)
		{

			return -0.5f * ((float)Math.Cos(Math.PI * time / totalTime) - 1);
		}

		#endregion

		//////////////////////////////////////////////
		// Quad Function
		/////////////////////////////////////////////
		#region Quad


		/// <summary>
		/// 
		/// </summary>
		/// <param name="inOutType">How should use sine lerp </param>
		/// <param name="time">Current Time of lerp</param>
		/// <param name="totalTime">Total time between lerps</param>
		/// <returns>Lerp Value</returns>
		static float EaseQuad(EasingInOutType inOutType, float time, float totalTime = 1.0f)
		{
			switch (inOutType)
			{
				case EasingInOutType.EaseIn:
					{
						return EaseInQuad(time, totalTime);
					}
				case EasingInOutType.EaseOut:
					{
						return EaseOutQuad(time, totalTime);
					}
				case EasingInOutType.EaseInOut:
					{
						return EaseInOutQuad(time, totalTime);
					}
				default:
					{
						return EaseInQuad(time, totalTime);
					}
			}
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="time">Current Time of lerp</param>
		/// <param name="totalTime">Total time between lerps</param>
		/// <returns>Lerp Value</returns>
		static float EaseInQuad(float time, float totalTime = 1.0f)
		{

			return (time /= totalTime) * time;
		}


		/// <summary>
		///  
		/// </summary>
		/// <param name="time">Current Time of lerp</param>
		/// <param name="totalTime">Total time between lerps</param>
		/// <returns>Lerp Value</returns>
		static float EaseOutQuad(float time, float totalTime = 1.0f)
		{

			return -(time /= totalTime) * (time - 2);
		}



		/// <summary>
		///  
		/// </summary>
		/// <param name="time">Current Time of lerp</param>
		/// <param name="totalTime">Total time between lerps</param>
		/// <returns>Lerp Value</returns>
		static float EaseInOutQuad(float time, float totalTime = 1.0f)
		{

			if ((time /= totalTime / 2) < 1)
			{
				return 0.5f * time * time;
			}

			return -0.5f * (--time * (time - 2) - 1);
		}
		#endregion


		//////////////////////////////////////////////
		// Cubic Function
		/////////////////////////////////////////////
		#region Cubic

		/// <summary>
		/// 
		/// </summary>
		/// <param name="inOutType">How should use sine lerp </param>
		/// <param name="time">Current Time of lerp</param>
		/// <param name="totalTime">Total time between lerps</param>
		/// <returns>Lerp Value</returns>
		static float EaseCubic(EasingInOutType inOutType, float time, float totalTime = 1.0f)
		{
			switch (inOutType)
			{
				case EasingInOutType.EaseIn:
					{
						return EaseInCubic(time, totalTime);
					}
				case EasingInOutType.EaseOut:
					{
						return EaseOutCubic(time, totalTime);
					}
				case EasingInOutType.EaseInOut:
					{
						return EaseInOutCubic(time, totalTime);
					}
				default:
					{
						return EaseInCubic(time, totalTime);
					}
			}
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="time">Current Time of lerp</param>
		/// <param name="totalTime">Total time between lerps</param>
		/// <returns>Lerp Value</returns>
		static float EaseInCubic(float time, float totalTime = 1.0f)
		{

			return (time /= totalTime) * time * time;
		}


		/// <summary>
		///  
		/// </summary>
		/// <param name="time">Current Time of lerp</param>
		/// <param name="totalTime">Total time between lerps</param>
		/// <returns>Lerp Value</returns>
		static float EaseOutCubic(float time, float totalTime = 1.0f)
		{

			return (time = time / totalTime - 1) * time * time + 1;
		}



		/// <summary>
		///  
		/// </summary>
		/// <param name="time">Current Time of lerp</param>
		/// <param name="totalTime">Total time between lerps</param>
		/// <returns>Lerp Value</returns>
		static float EaseInOutCubic(float time, float totalTime = 1.0f)
		{

			if ((time /= totalTime / 2) < 1)
			{
				return 0.5f * time * time * time;
			}
			return 0.5f * ((time -= 2) * time * time + 2);
		}
		#endregion

		//////////////////////////////////////////////
		// Quart  Function
		/////////////////////////////////////////////
		#region Quart 

		/// <summary>
		/// 
		/// </summary>
		/// <param name="inOutType">How should use sine lerp </param>
		/// <param name="time">Current Time of lerp</param>
		/// <param name="totalTime">Total time between lerps</param>
		/// <returns>Lerp Value</returns>
		static float EaseQuart(EasingInOutType inOutType, float time, float totalTime = 1.0f)
		{
			switch (inOutType)
			{
				case EasingInOutType.EaseIn:
					{
						return EaseInQuart(time, totalTime);
					}
				case EasingInOutType.EaseOut:
					{
						return EaseOutQuart(time, totalTime);
					}
				case EasingInOutType.EaseInOut:
					{
						return EaseInOutQuart(time, totalTime);
					}
				default:
					{
						return EaseInQuart(time, totalTime);
					}
			}
		}
		/// <summary>
		///
		/// </summary>
		/// <param name="time">Current Time of lerp</param>
		/// <param name="totalTime">Total time between lerps</param>
		/// <returns>Lerp Value</returns>
		static float EaseInQuart(float time, float totalTime = 1.0f)
		{

			return (time /= totalTime) * time * time * time;
		}


		/// <summary>
		///  
		/// </summary>
		/// <param name="time">Current Time of lerp</param>
		/// <param name="totalTime">Total time between lerps</param>
		/// <returns>Lerp Value</returns>
		static float EaseOutQuart(float time, float totalTime = 1.0f)
		{

			return -((time = time / totalTime - 1) * time * time * time - 1);
		}



		/// <summary>
		///  
		/// </summary>
		/// <param name="time">Current Time of lerp</param>
		/// <param name="totalTime">Total time between lerps</param>
		/// <returns>Lerp Value</returns>
		static float EaseInOutQuart(float time, float totalTime = 1.0f)
		{

			if ((time /= totalTime / 2) < 1)
			{
				return 0.5f * time * time * time * time;
			}
			return -0.5f * ((time -= 2) * time * time * time - 2);
		}
		#endregion

		//////////////////////////////////////////////
		// Quint  Function
		/////////////////////////////////////////////
		#region Quint 

		/// <summary>
		/// 
		/// </summary>
		/// <param name="inOutType">How should use sine lerp </param>
		/// <param name="time">Current Time of lerp</param>
		/// <param name="totalTime">Total time between lerps</param>
		/// <returns>Lerp Value</returns>
		static float EaseQuint(EasingInOutType inOutType, float time, float totalTime = 1.0f)
		{
			switch (inOutType)
			{
				case EasingInOutType.EaseIn:
					{
						return EaseInQuint(time, totalTime);
					}
				case EasingInOutType.EaseOut:
					{
						return EaseOutQuint(time, totalTime);
					}
				case EasingInOutType.EaseInOut:
					{
						return EaseInOutQuint(time, totalTime);
					}
				default:
					{
						return EaseInQuint(time, totalTime);
					}
			}
		}
		/// <summary>
		///
		/// </summary>
		/// <param name="time">Current Time of lerp</param>
		/// <param name="totalTime">Total time between lerps</param>
		/// <returns>Lerp Value</returns>
		static float EaseInQuint(float time, float totalTime = 1.0f)
		{

			return (time /= totalTime) * time * time * time * time;
		}


		/// <summary>
		///  
		/// </summary>
		/// <param name="time">Current Time of lerp</param>
		/// <param name="totalTime">Total time between lerps</param>
		/// <returns>Lerp Value</returns>
		static float EaseOutQuint(float time, float totalTime = 1.0f)
		{

			return (time = time / totalTime - 1) * time * time * time * time + 1;
		}



		/// <summary>
		///  
		/// </summary>
		/// <param name="time">Current Time of lerp</param>
		/// <param name="totalTime">Total time between lerps</param>
		/// <returns>Lerp Value</returns>
		static float EaseInOutQuint(float time, float totalTime = 1.0f)
		{

			if ((time /= totalTime / 2) < 1)
			{
				return 0.5f * time * time * time * time * time;
			}
			return 0.5f * ((time -= 2) * time * time * time * time + 2);
		}
		#endregion

		//////////////////////////////////////////////
		// Expo   Function
		/////////////////////////////////////////////
		#region Expo  
		/// <summary>
		/// 
		/// </summary>
		/// <param name="inOutType">How should use sine lerp </param>
		/// <param name="time">Current Time of lerp</param>
		/// <param name="totalTime">Total time between lerps</param>
		/// <returns>Lerp Value</returns>
		static float EaseExpo(EasingInOutType inOutType, float time, float totalTime = 1.0f)
		{
			switch (inOutType)
			{
				case EasingInOutType.EaseIn:
					{
						return EaseInExpo(time, totalTime);
					}
				case EasingInOutType.EaseOut:
					{
						return EaseOutExpo(time, totalTime);
					}
				case EasingInOutType.EaseInOut:
					{
						return EaseInOutExpo(time, totalTime);
					}
				default:
					{
						return EaseInExpo(time, totalTime);
					}
			}
		}
		/// <summary>
		///
		/// </summary>
		/// <param name="time">Current Time of lerp</param>
		/// <param name="totalTime">Total time between lerps</param>
		/// <returns>Lerp Value</returns>
		static float EaseInExpo(float time, float totalTime = 1.0f)
		{
			if (time == 0)
			{
				return 0;
			}
			else
			{
				return (float)Math.Pow(2, 10 * (time / totalTime - 1));
			}
		}


		/// <summary>
		///  
		/// </summary>
		/// <param name="time">Current Time of lerp</param>
		/// <param name="totalTime">Total time between lerps</param>
		/// <returns>Lerp Value</returns>
		static float EaseOutExpo(float time, float totalTime = 1.0f)
		{


			if (time >= totalTime)
			{
				return 1;
			}
			else
			{
				return -(float)Math.Pow(2, -10 * time / totalTime) + 1;
			}

		}



		/// <summary>
		///  
		/// </summary>
		/// <param name="time">Current Time of lerp</param>
		/// <param name="totalTime">Total time between lerps</param>
		/// <returns>Lerp Value</returns>
		static float EaseInOutExpo(float time, float totalTime = 1.0f)
		{

			if (time == 0)
			{
				return 0;
			}
			else if (time >= totalTime)
			{
				return 1;
			}
			if ((time /= totalTime / 2) < 1)
			{
				return 0.5f * (float)Math.Pow(2, 10 * (time - 1));
			}
			return 0.5f * (-(float)Math.Pow(2, -10 * (time - 1)) + 2);
		}
		#endregion


		//////////////////////////////////////////////
		// Circ Function
		/////////////////////////////////////////////
		#region Circ 
		/// <summary>
		/// 
		/// </summary>
		/// <param name="inOutType">How should use sine lerp </param>
		/// <param name="time">Current Time of lerp</param>
		/// <param name="totalTime">Total time between lerps</param>
		/// <returns>Lerp Value</returns>
		static float EaseCirc(EasingInOutType inOutType, float time, float totalTime = 1.0f)
		{
			switch (inOutType)
			{
				case EasingInOutType.EaseIn:
					{
						return EaseInCirc(time, totalTime);
					}
				case EasingInOutType.EaseOut:
					{
						return EaseOutCirc(time, totalTime);
					}
				case EasingInOutType.EaseInOut:
					{
						return EaseInOutCirc(time, totalTime);
					}
				default:
					{
						return EaseInCirc(time, totalTime);
					}
			}
		}
		/// <summary>
		/// Ease In Sine
		/// </summary>
		/// <param name="time">Current Time of lerp</param>
		/// <param name="totalTime">Total time between lerps</param>
		/// <returns>Lerp Value</returns>
		static float EaseInCirc(float time, float totalTime = 1.0f)
		{

			return -((float)Math.Sqrt(1 - (time /= totalTime) * time) - 1);
		}


		/// <summary>
		/// Ease out Sine
		/// </summary>
		/// <param name="time">Current Time of lerp</param>
		/// <param name="totalTime">Total time between lerps</param>
		/// <returns>Lerp Value</returns>
		static float EaseOutCirc(float time, float totalTime = 1.0f)
		{

			return (float)Math.Sqrt(1 - (time = time / totalTime - 1) * time);
		}



		/// <summary>
		/// Ease int out Sine
		/// </summary>
		/// <param name="time">Current Time of lerp</param>
		/// <param name="totalTime">Total time between lerps</param>
		/// <returns>Lerp Value</returns>
		static float EaseInOutCirc(float time, float totalTime = 1.0f)
		{

			if ((time /= totalTime / 2) < 1)
			{
				return -0.5f * ((float)Math.Sqrt(1 - time * time) - 1);
			}

			return 0.5f * ((float)Math.Sqrt(1 - (time -= 2) * time) + 1);
		}

		#endregion

		//////////////////////////////////////////////
		// Back  Function
		/////////////////////////////////////////////
		#region Back 

		/// <summary>
		/// 
		/// </summary>
		/// <param name="inOutType">How should use sine lerp </param>
		/// <param name="time">Current Time of lerp</param>
		/// <param name="totalTime">Total time between lerps</param>
		/// <returns>Lerp Value</returns>
		static float EaseBack(EasingInOutType inOutType, float time, float totalTime = 1.0f)
		{
			switch (inOutType)
			{
				case EasingInOutType.EaseIn:
					{
						return EaseInBack(time, totalTime);
					}
				case EasingInOutType.EaseOut:
					{
						return EaseOutBack(time, totalTime);
					}
				case EasingInOutType.EaseInOut:
					{
						return EaseInOutBack(time, totalTime);
					}
				default:
					{
						return EaseInBack(time, totalTime);
					}
			}
		}
		/// <summary>
		///
		/// </summary>
		/// <param name="time">Current Time of lerp</param>
		/// <param name="s">overshoot value</param>
		/// <param name="totalTime">Total time between lerps</param>
		/// <returns>Lerp Value</returns>
		static float EaseInBack(float time, float totalTime = 1.0f, float s = 1.70158f)
		{
			return (time /= totalTime) * time * ((s + 1) * time - s);
		}

		/// <summary>
		///  
		/// </summary>
		/// <param name="time">Current Time of lerp</param>
		/// <param name="s">overshoot value</param>
		/// <param name="totalTime">Total time between lerps</param>
		/// <returns>Lerp Value</returns>
		static float EaseOutBack(float time, float totalTime = 1.0f, float s = 1.70158f)
		{

			return (time = time / totalTime - 1) * time * ((s + 1) * time + s) + 1;
		}



		/// <summary>
		///  
		/// </summary>
		/// <param name="time">Current Time of lerp</param>
		/// <param name="s">overshoot value</param>
		/// <param name="totalTime">Total time between lerps</param>
		/// <returns>Lerp Value</returns>
		static float EaseInOutBack(float time, float totalTime = 1.0f, float s = 1.70158f)
		{

			if ((time /= totalTime / 2) < 1)
			{
				return 0.5f * (time * time * (((s *= 1.525f) + 1) * time - s));
			}
			return 0.5f * ((time -= 2) * time * (((s *= 1.525f) + 1) * time + s) + 2);
		}
		#endregion


		//////////////////////////////////////////////
		// Elastic  Function
		/////////////////////////////////////////////
		#region Elastic 

		/// <summary>
		/// 
		/// </summary>
		/// <param name="inOutType">How should use sine lerp </param>
		/// <param name="time">Current Time of lerp</param>
		/// <param name="totalTime">Total time between lerps</param>
		/// <returns>Lerp Value</returns>
		static float EaseElastic(EasingInOutType inOutType, float time, float totalTime = 1.0f)
		{
			switch (inOutType)
			{
				case EasingInOutType.EaseIn:
					{
						return EaseInElastic(time, totalTime);
					}
				case EasingInOutType.EaseOut:
					{
						return EaseOutElastic(time, totalTime);
					}
				case EasingInOutType.EaseInOut:
					{
						return EaseInOutElastic(time, totalTime);
					}
				default:
					{
						return EaseInElastic(time, totalTime);
					}
			}
		}
		/// <summary>
		///
		/// </summary>
		/// <param name="time">Current Time of lerp</param>
		/// <param name="a">Beginning value of lerp</param>
		/// <param name="b">Final value of lerp</param>
		/// <param name="totalTime">Total time between lerps</param>
		/// <returns>Lerp Value</returns>
		static float EaseInElastic(float time, float totalTime = 1.0f)
		{
			//if (t == 0) return b; if ((t /= d) == 1) return b + c;
			//float p = d * .3f;
			//float a = c;
			//float s = p / 4;
			//float postFix = a * pow(2, 10 * (t -= 1)); // this is a fix, again, with post-increment operators
			//return -(postFix * sin((t * d - s) * (2 * PI) / p)) + b;

			if (time == 0)
			{
				return 0;
			}
			if ((time /= totalTime) >= 1)
			{
				return 1;
			}
			float p = totalTime * .3f;
			float s = p / 4;
			float postFix = (float)Math.Pow(2, 10 * (time -= 1));

			return -(postFix * (float)Math.Sin((time * totalTime - s) * (2 * Math.PI) / p));
		}


		/// <summary>
		///  
		/// </summary>
		/// <param name="time">Current Time of lerp</param>
		/// <param name="totalTime">Total time between lerps</param>
		/// <returns>Lerp Value</returns>
		static float EaseOutElastic(float time, float totalTime = 1.0f)
		{

			if (time == 0)
			{
				return 0;
			}
			if ((time /= totalTime) >= 1)
			{
				return 1;
			}
			float p = totalTime * .3f;
			float s = p / 4;
			return (float)Math.Pow(2, -10 * time) * (float)Math.Sin((time * totalTime - s) * (2 * (float)Math.PI) / p) + 1;
		}



		/// <summary>
		///  
		/// </summary>
		/// <param name="time">Current Time of lerp</param>
		/// <param name="totalTime">Total time between lerps</param>
		/// <returns>Lerp Value</returns>
		static float EaseInOutElastic(float time, float totalTime = 1.0f)
		{
			if (time == 0)
			{
				return 0;
			}
			if ((time /= totalTime / 2) >= 2)
			{
				return 1;

			}

			float p = totalTime * (.3f * 1.5f);
			float s = p / 4;

			if (time < 1)
			{
				return -.5f * ((float)Math.Pow(2, 10 * (time -= 1)) * (float)Math.Sin((time * totalTime - s) * (2 * (float)Math.PI) / p));
			}
			return (float)Math.Pow(2, -10 * (time -= 1)) * (float)Math.Sin((time * totalTime - s) * (2 * (float)Math.PI) / p) * .5f;
		}
		#endregion

		//////////////////////////////////////////////
		// Bounce   Function
		/////////////////////////////////////////////
		#region Bounce  
		/// <summary>
		/// 
		/// </summary>
		/// <param name="inOutType">How should use sine lerp </param>
		/// <param name="time">Current Time of lerp</param>
		/// <param name="totalTime">Total time between lerps</param>
		/// <returns>Lerp Value</returns>
		static float EaseBounce(EasingInOutType inOutType, float time, float totalTime = 1.0f)
		{
			switch (inOutType)
			{
				case EasingInOutType.EaseIn:
					{
						return EaseInBounce(time, totalTime);
					}
				case EasingInOutType.EaseOut:
					{
						return EaseOutBounce(time, totalTime);
					}
				case EasingInOutType.EaseInOut:
					{
						return EaseInOutBounce(time, totalTime);
					}
				default:
					{
						return EaseInBounce(time, totalTime);
					}
			}
		}
		/// <summary>
		///
		/// </summary>
		/// <param name="time">Current Time of lerp</param>
		/// <param name="totalTime">Total time between lerps</param>
		/// <returns>Lerp Value</returns>
		static float EaseInBounce(float time, float totalTime = 1.0f)
		{

			return 1 - EaseOutBounce(totalTime - time, totalTime);
		}


		/// <summary>
		///  
		/// </summary>
		/// <param name="time">Current Time of lerp</param>
		/// <param name="totalTime">Total time between lerps</param>
		/// <returns>Lerp Value</returns>
		static float EaseOutBounce(float time, float totalTime = 1.0f)
		{

			if ((time /= totalTime) < 1 / 2.75f)
			{
				return 7.5625f * time * time;
			}
			else if (time < 2 / 2.75f)
			{
				return 7.5625f * (time -= 1.5f / 2.75f) * time + .75f;
			}
			else if (time < 2.5 / 2.75)
			{
				return 7.5625f * (time -= 2.25f / 2.75f) * time + .9375f;
			}
			else
			{
				return 7.5625f * (time -= 2.625f / 2.75f) * time + .984375f;
			}
		}



		/// <summary>
		///  
		/// </summary>
		/// <param name="time">Current Time of lerp</param>
		/// <param name="totalTime">Total time between lerps</param>
		/// <returns>Lerp Value</returns>
		static float EaseInOutBounce(float time, float totalTime = 1.0f)
		{

			if (time < totalTime / 2)
			{
				return EaseInBounce(time * 2, totalTime) * .5f;
			}
			else
			{
				return EaseOutBounce(time * 2 - totalTime, totalTime) * .5f + .5f;
			}
		}
		#endregion

	}
}