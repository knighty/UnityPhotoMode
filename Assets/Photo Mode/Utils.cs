
using System;
using System.Collections;
using UnityEngine;

public class Utils
{
	public delegate bool CoFunc();
	private static IEnumerator CoroutineRunnerSimple(CoFunc func, float delay = 0)
	{
		if (delay > 0)
		{
			yield return new WaitForSecondsRealtime(delay);
		}

		while (true)
		{
			if (func())
			{
				yield return null;
			}
			else
			{
				yield break;
			}
		}
	}

	public static IEnumerator Tween(float state, float to, float speed, Action<float> handler, float delay = 0)
	{
		return CoroutineRunnerSimple(() =>
		{
			float delta = Mathf.Abs(state - to);
			if (delta < 0.0001f)
			{
				state = to;
				handler(state);
				return false;
			}

			state += ((to > state) ? 1 : -1) * Mathf.Min(delta, Time.unscaledDeltaTime * speed);
			handler(state);
			return true;
		}, delay);
	}

	public static IEnumerator TweenTime(float state, float to, float time, Action<float> handler, Func<float, float> interpolator = null)
	{
		float start = state;
		float t = 0;
		return CoroutineRunnerSimple(() =>
		{
			float delta = Mathf.Abs(state - to);
			if (t > 1)
			{
				state = to;
				handler(state);
				return false;
			}

			t += Time.unscaledDeltaTime / time;

			float interpolatedT = interpolator == null ? t : interpolator(t);
			state = start + (to - start) * interpolatedT;
			handler(state);
			return true;
		});
	}

	public static float EaseOutBack(float x)
	{
		float c1 = 1.70158f;
		float c3 = c1 + 1;

		return 1 + c3 * Mathf.Pow(x - 1, 3) + c1 * Mathf.Pow(x - 1, 2);
	}

	public static float EaseOutQuart(float x)
	{
		return 1 - Mathf.Pow(1 - x, 4);
	}
}
