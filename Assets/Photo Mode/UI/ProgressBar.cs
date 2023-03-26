using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
	[SerializeField] float progress = 0;
	[SerializeField] AccumulationCameraController accumulationCameraController;
	[SerializeField] RectTransform bar;
	[SerializeField] Text text;

	public float Progress
	{
		get => progress;
		set
		{
			progress = value;
			UpdateBar();
		}
	}

	private void UpdateBar()
	{
		bar.anchorMax = new Vector2(progress, 1);
	}

	private void OnValidate()
	{
		UpdateBar();
	}

	private void Update()
	{
		if (accumulationCameraController.accumulator != null)
		{
			Progress = Mathf.Min(1, (float)accumulationCameraController.accumulator.Accumulation / (float)accumulationCameraController.accumulator.Total);
			text.text = $"Rendering - {accumulationCameraController.accumulator.Accumulation} / {accumulationCameraController.accumulator.Total}";
		}
	}
}
