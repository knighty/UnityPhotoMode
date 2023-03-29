using PhotoMode;
using UnityEngine;

public class AccumulationCameraState
{
	public int width;
	public int height;
	public float aperture;
	public float fov;
	public float focusDistance;
	public Vector3 position;
	public Quaternion rotation;
	public ApertureShape apertureShape;
	public int accumulation = 0;
	public Vector2 lensShift;
	public Vector2 lensTilt;

	public AccumulationCameraState(Camera camera, float focusDistance, int accumulation)
	{
		this.width = camera.pixelWidth;
		this.height = camera.pixelHeight;
		this.fov = camera.fieldOfView;
		this.focusDistance = focusDistance;
		this.position = camera.transform.position;
		this.rotation = camera.transform.rotation;
		this.accumulation = accumulation;
		this.lensShift = camera.lensShift;
	}

	public override bool Equals(object obj)
	{
		if (obj is AccumulationCameraState)
		{
			if (obj == null) return false;
			AccumulationCameraState other = (AccumulationCameraState)obj;
			return
				width == other.width &&
				height == other.height &&
				aperture == other.aperture &&
				fov == other.fov &&
				focusDistance == other.focusDistance &&
				Vector2.Distance(lensShift, other.lensShift) < 0.00001f &&
				Vector2.Distance(lensTilt, other.lensTilt) < 0.00001f &&
				Vector3.Distance(position, other.position) < 0.000001f &&
				Quaternion.Angle(rotation, other.rotation) < 0.000001f;
		}
		return base.Equals(obj);
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}
}

public class CameraRender : MonoBehaviour
{
    [SerializeField] private Transform lookAt;
    [SerializeField] private RenderTexture renderTexture;
    [SerializeField] private RenderTexture processingRenderTexture;
    [SerializeField] private RenderTexture outputTexture;
    [SerializeField] private Material material;
    [SerializeField] private GameObject ui;

	[SerializeField] float sensorSize = 50f;

    // Start is called before the first frame update
    void Start()
    {
#if UNITY_EDITOR
		Application.targetFrameRate = 120;
#endif
	}

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
		m[0, 0] = x;
		m[0, 1] = 0;
		m[0, 2] = a;
		m[0, 3] = 0;
		m[1, 0] = 0;
		m[1, 1] = y;
		m[1, 2] = b;
		m[1, 3] = 0;
		m[2, 0] = 0;
		m[2, 1] = 0;
		m[2, 2] = c;
		m[2, 3] = d;
		m[3, 0] = 0;
		m[3, 1] = 0;
		m[3, 2] = e;
		m[3, 3] = 0;
		return m;
	}

	//http://graphics.stanford.edu/courses/cs248-02/haeberli-akeley-accumulation-buffer-sig90.pdf
	Matrix4x4 GetProjectionMatrix(RenderTexture texture, Camera camera, int i)
	{
		float focalPlane = Vector3.Distance(lookAt.position, transform.position);
		float fov = camera.fieldOfView * (Mathf.PI / 180.0f);
		float aspect = (float)texture.width / (float)texture.height;

		// Anti aliasing
		Vector2 pixelOffset = new Vector2((i % 16) / 16.0f - 0.5f, (i / 16) / 16.0f - 0.5f);

		// Lens offset
		UnityEngine.Random.InitState(i);
		Vector2 rand = UnityEngine.Random.insideUnitCircle;
		float lensdx = rand.x * (sensorSize / 2.0f) / 1000.0f * aspect + camera.lensShift.x;
		float lensdy = rand.y * (sensorSize / 2.0f) / 1000.0f + camera.lensShift.y;

		float tanFov = Mathf.Tan(fov / 2.0f) * 0.5f;
		float left = -tanFov * aspect;
		float right = tanFov * aspect;
		float top = tanFov;
		float bottom = -tanFov;

		float dx = -(pixelOffset.x * (right - left) / (float)texture.width + lensdx * camera.nearClipPlane / focalPlane);
		float dy = -(pixelOffset.y * (top - bottom) / (float)texture.height + lensdy * camera.nearClipPlane / focalPlane);
		
		Matrix4x4 mat = PerspectiveOffCenter(left + dx, right + dx, bottom + dy, top + dy, camera.nearClipPlane, camera.farClipPlane);
		Matrix4x4 translate = Matrix4x4.Translate(new Vector3(-lensdx, -lensdy, 0));
		Matrix4x4 res = mat * translate;

		return res;
	}

	private int j = 0;

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKey(KeyCode.J)) 
		{
			j++;
		}

        if (lookAt != null)
        {
            Camera camera = GetComponent<Camera>();
            //transform.LookAt(lookAt.transform.position);
            camera.projectionMatrix = GetProjectionMatrix(outputTexture, camera, j % 256);

			if (Input.GetKeyDown(KeyCode.Y))
            {
                RenderTexture oldRT = camera.targetTexture;
				camera.targetTexture = renderTexture;
                ui.SetActive(false);

				RenderTexture rt = RenderTexture.active;
				RenderTexture.active = processingRenderTexture;
				GL.Clear(true, true, Color.clear);
				RenderTexture.active = rt;

				for (int i = 0; i < 256; i++)
                {
					camera.projectionMatrix = GetProjectionMatrix(outputTexture, camera, i);
					camera.Render();

					Graphics.Blit(renderTexture, processingRenderTexture, material);

					/*
					 * genwindow(left, right, bottom,top, nearJar, pixdx,pixdy,
lensdx, lensdyJocalplane)
float left, right, bottom, top, near, far, pixdx, pixdy;
float lensdx, lens@, focalplane;
I
short vxl, vx2, vyl, vy2;
float xwsize, ywsize, dx, dy;
int xpixels, ypixeis;
g etvie wport( & v x l ,& vx2 ,& vy l ,& vy2 ) ;
xpixels = vx2-vxl +l;
ypixels = vy2-vyl+l;
xwsize = right-left;
ywsize = top-bottom;
dx = -(pixdx*xwsize/xpixels + lensdx*near/foealplane);
dy = -(pixdy*ywsize/ypixels + lensdy*near/focalplane);
window(left+dx, right+dx, bottom+dy, top+dy, nearJar);
translate(-lensdx,-lensdy, O.O);
					 */
				}

				Graphics.Blit(processingRenderTexture, outputTexture);

				SaveTextureToFileUtility.SaveRenderTextureToFile(outputTexture, "test", SaveTextureToFileUtility.SaveTextureFileFormat.PNG);

				Debug.Log($"Render Done");

				ui.SetActive(true);

				camera.targetTexture = oldRT;
			}
		}

	}
}

public class SaveTextureToFileUtility
{
	public enum SaveTextureFileFormat
	{
		EXR, JPG, PNG, TGA
	};

	/// <summary>
	/// Saves a Texture2D to disk with the specified filename and image format
	/// </summary>
	/// <param name="tex"></param>
	/// <param name="filePath"></param>
	/// <param name="fileFormat"></param>
	/// <param name="jpgQuality"></param>
	static public void SaveTexture2DToFile(Texture2D tex, string filePath, SaveTextureFileFormat fileFormat, int jpgQuality = 95)
	{
		switch (fileFormat)
		{
			case SaveTextureFileFormat.EXR:
				System.IO.File.WriteAllBytes(filePath + ".exr", tex.EncodeToEXR());
				break;
			case SaveTextureFileFormat.JPG:
				System.IO.File.WriteAllBytes(filePath + ".jpg", tex.EncodeToJPG(jpgQuality));
				break;
			case SaveTextureFileFormat.PNG:
				System.IO.File.WriteAllBytes(filePath + ".png", tex.EncodeToPNG());
				break;
			case SaveTextureFileFormat.TGA:
				System.IO.File.WriteAllBytes(filePath + ".tga", tex.EncodeToTGA());
				break;
		}
	}


	/// <summary>
	/// Saves a RenderTexture to disk with the specified filename and image format
	/// </summary>
	/// <param name="renderTexture"></param>
	/// <param name="filePath"></param>
	/// <param name="fileFormat"></param>
	/// <param name="jpgQuality"></param>
	static public void SaveRenderTextureToFile(RenderTexture renderTexture, string filePath, SaveTextureFileFormat fileFormat = SaveTextureFileFormat.PNG, int jpgQuality = 95)
	{
		Texture2D tex;
		if (fileFormat != SaveTextureFileFormat.EXR)
			tex = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false, false);
		else
			tex = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBAFloat, false, true);
		var oldRt = RenderTexture.active;
		RenderTexture.active = renderTexture;
		tex.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
		tex.Apply();
		RenderTexture.active = oldRt;
		SaveTexture2DToFile(tex, filePath, fileFormat, jpgQuality);
		if (Application.isPlaying)
			UnityEngine.Object.Destroy(tex);
		else
			UnityEngine.Object.DestroyImmediate(tex);

	}

}