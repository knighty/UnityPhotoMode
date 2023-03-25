using UnityEngine.Rendering.PostProcessing;
using UnityEngine;
using PhotoMode;
using System;
using System.Data;
using UnityEngine.Rendering;

public interface AccumulationCameraAccumulator
{
	int Accumulation { get; }
	int Total { get; }
	bool Enabled { get; }
	AccumulationCameraState State { get; }
	void Enable();
	void Disable();
	void Update();
}

public class PostProcessingAccumulationCameraAccumulator : AccumulationCameraAccumulator
{
	int accumulation = 0;
	Camera camera;
	PhotoModeSettings photoModeSettings;
	AccumulationSettings accumulationSettings;
	AccumulationCameraState previousState = null;
	AccumulationCameraState currentState = null;

	public PostProcessingAccumulationCameraAccumulator(Camera camera, PhotoModeSettings photoModeSettings, AccumulationSettings settings)
	{
		this.camera = camera;
		this.photoModeSettings = photoModeSettings;
		accumulationSettings = settings;

		photoModeSettings.OnChange += setting => Restart();
	}

	public int Accumulation => accumulation;

	public int Total => photoModeSettings.Accumulations;

	public bool Enabled => true;

	public AccumulationCameraState State => currentState;

	public void Enable()
	{
		AccumulationSettings.accumulator = this;
		accumulationSettings.enabled.Override(true);
	}

	public void Disable()
	{
		accumulationSettings.enabled.Override(false);
	}

	public void Restart()
	{
		accumulation = 0;
		previousState = null;
	}

	protected void UpdateState()
	{
		AccumulationCameraState s = new AccumulationCameraState(camera, photoModeSettings.Aperture, photoModeSettings.FocusDistance, Accumulation)
		{
			apertureShape = photoModeSettings.ApertureShape,
			lensTilt = photoModeSettings.LensTilt
		};
		if (photoModeSettings.Fov.IsOverriding)
			s.fov = photoModeSettings.Fov;

		currentState = s;
	}

	public void Update()
	{
		accumulation++;
		UpdateState();
		if (!currentState.Equals(previousState))
		{
			accumulation = 0;
			previousState = currentState;
			UpdateState(); 
		}
	}
}

public class RenderAccumulationCameraAccumulator : AccumulationCameraAccumulator
{
	int accumulation = 0;
	Camera camera;
	PhotoModeSettings photoModeSettings;

	public RenderAccumulationCameraAccumulator(Camera camera, PhotoModeSettings photoModeSettings)
	{
		this.camera = camera;
		this.photoModeSettings = photoModeSettings;
	}

	public int Accumulation => accumulation;

	public int Total => photoModeSettings.Accumulations;

	public bool Enabled => true;

	public AccumulationCameraState State
	{
		get
		{
			AccumulationCameraState s = new AccumulationCameraState(camera, photoModeSettings.Aperture, photoModeSettings.FocusDistance, Accumulation)
			{
				apertureShape = photoModeSettings.ApertureShape
			};
			if (photoModeSettings.Fov.IsOverriding)
				s.fov = photoModeSettings.Fov;

			return s;
		}
	}

	public void Enable()
	{
	}

	public void Disable()
	{
	}

	public void Update()
	{
		accumulation++;
	}
}

public class AccumulationCameraController : MonoBehaviour
{
	int j = 0;

	public Action OnFrameRendered;

	public AccumulationCameraAccumulator accumulator;

	public AccumulationCameraState cameraState;

	static Matrix4x4 PerspectiveOffCenter(float left, float right, float bottom, float top, float near, float far)
	{
		float x = 2.0F * near / (right - left);
		float y = 2.0F * near / (top - bottom);
		float a = (right + left) / (right - left);
		float b = (top + bottom) / (top - bottom);
		float c = -(far + near) / (far - near);
		float d = -(2.0F * far * near) / (far - near);
		float e = -1.0F;
		Matrix4x4 m = new Matrix4x4();
		m[0, 0] = x; m[0, 1] = 0; m[0, 2] = a; m[0, 3] = 0;
		m[1, 0] = 0; m[1, 1] = y; m[1, 2] = b; m[1, 3] = 0;
		m[2, 0] = 0; m[2, 1] = 0; m[2, 2] = c; m[2, 3] = d;
		m[3, 0] = 0; m[3, 1] = 0; m[3, 2] = e; m[3, 3] = 0;
		return m;
	}

	//http://graphics.stanford.edu/courses/cs248-02/haeberli-akeley-accumulation-buffer-sig90.pdf
	(Matrix4x4, Vector3) GetProjectionMatrix(AccumulationCameraState state, Camera camera, int total)
	{
		if (state.apertureShape == null)
		{
			Debug.Log("No aperture shape for calculating projection matrix");
		}

		int width = state.width;
		int height = state.height;
		int i = state.accumulation;

		float focalPlane = state.focusDistance;
		float fov = state.fov * (Mathf.PI / 180.0f);
		float aspect = (float)width / (float)height;

		// Anti aliasing
		float sqrtTotal = Mathf.Sqrt(total);
		Vector2 pixelOffset = new Vector2((i % sqrtTotal) / sqrtTotal - 0.5f, (i / sqrtTotal) / sqrtTotal - 0.5f);

		// Lens offset
		Vector3 rand = state.apertureShape.GetRandomPoint(state.accumulation);
		//Debug.Log($"Set matrix at {state.accumulation} {rand}");
		float lensdx = rand.x * (state.aperture / 2.0f) / 1000.0f;// * aspect;
		float lensdy = rand.y * (state.aperture / 2.0f) / 1000.0f;

		float tanFov = Mathf.Tan(fov / 2.0f) * camera.nearClipPlane;
		float left = -tanFov * aspect;
		float right = tanFov * aspect;
		float top = tanFov;
		float bottom = -tanFov;

		float dx = -(pixelOffset.x * (right - left) / (float)width + lensdx * camera.nearClipPlane / focalPlane) + camera.lensShift.x;
		float dy = -(pixelOffset.y * (top - bottom) / (float)height + lensdy * camera.nearClipPlane / focalPlane) + camera.lensShift.y;

		Matrix4x4 mat = PerspectiveOffCenter(left + dx, right + dx, bottom + dy, top + dy, camera.nearClipPlane, camera.farClipPlane);
		//Matrix4x4 translate = Matrix4x4.Translate(new Vector4(-lensdx, -lensdy, 0, 0));
		Matrix4x4 translate = Matrix4x4.Translate(new Vector4(-lensdx, -lensdy, 0, 0));
		Matrix4x4 res = mat * translate;
		//camera.lensShift = new Vector2(-lensdx, -lensdy);
		//camera.usePhysicalProperties = true;

		return (mat, new Vector3(-lensdx, -lensdy, 0));
	}

	Matrix4x4 GetProjectionMatrix2(Camera camera)
	{
		int width = camera.pixelWidth;
		int height = camera.pixelHeight;
		float fov = camera.fieldOfView * (Mathf.PI / 180.0f);
		float aspect = (float)width / (float)height;

		float tanFov = Mathf.Tan(fov / 2.0f) * camera.nearClipPlane;
		float left = -tanFov * aspect;
		float right = tanFov * aspect;
		float top = tanFov;
		float bottom = -tanFov;

		Matrix4x4 mat = PerspectiveOffCenter(left, right, bottom, top, camera.nearClipPlane, camera.farClipPlane);

		return mat;
	}

	public void ResetCamera()
	{
		j = -1;
	}

	public void NextAccumulation()
	{
		j++;
	}

	public int CurrentAccumulation => j;

	public void SetAccmulator(AccumulationCameraAccumulator accmulator)
	{
		this.accumulator?.Disable();
		this.accumulator = accmulator;
		this.accumulator?.Enable();
	}

	int i = 0;

	void OnPreCull()
	{
		//Debug.Log($"Pre Cull {accumulator.Accumulation}");
		//Debug.Log($"Update {accumulator.Accumulation}");
		//RenderPipelineManager.
		Camera camera = GetComponent<Camera>();

		camera.ResetProjectionMatrix();
		camera.ResetWorldToCameraMatrix();

		if (accumulator != null)
		{
			accumulator.Update();
			OnFrameRendered?.Invoke();
			// We translate in view space because translating in projection space breaks almost everything in Unity
			// I'm not sure if this is 100% identical but it seems to work..
			(Matrix4x4 projection, Vector3 translation) = GetProjectionMatrix(accumulator.State, camera, accumulator.Total);
			camera.projectionMatrix = projection;
			//camera.cullingMatrix = GetProjectionMatrix2(camera) * camera.worldToCameraMatrix;
			//camera.nonJitteredProjectionMatrix = GetProjectionMatrix2(camera);
			//Quaternion rot = Quaternion.Euler(0, Mathf.Sin(Time.realtimeSinceStartup / 10.0f) * 30.0f, 0);
			Matrix4x4 rotMatrixY = Matrix4x4.Rotate(Quaternion.Euler(0, accumulator.State.lensTilt.x, 0));
			Matrix4x4 rotMatrixX = Matrix4x4.Rotate(Quaternion.Euler(accumulator.State.lensTilt.y, 0, 0));
			Quaternion rot = Quaternion.Euler(accumulator.State.lensTilt.y, accumulator.State.lensTilt.x, 0);
			Vector4 t = new Vector4(translation.x, translation.y, translation.z, 0);
			//Matrix4x4 translate = Matrix4x4.Translate((rotMatrixY * rotMatrixX) * t);
			Matrix4x4 translate = Matrix4x4.Translate(translation);

			camera.worldToCameraMatrix = translate * camera.worldToCameraMatrix;
			camera.fieldOfView = accumulator.State.fov;
		}

		/*if (accumulator != null && accumulator.Accumulation != 0)
		{
			Time.timeScale = 0;
		}*/
	}
}