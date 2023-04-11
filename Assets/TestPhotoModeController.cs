using PhotoMode;
using PhotoMode.UI;
using UnityEngine;

public class TestPhotoModeController : MonoBehaviour
{
    [SerializeField] PhotoModeUI ui;
    [SerializeField] Camera camera;
	[SerializeField] FlyCamera flyCamera;
	[SerializeField] ScriptableOptions options;

	void Awake()
	{
	}

	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		if (Input.GetKeyDown(KeyCode.F2))
		{
			ui.ToggleVisible();
		}
		flyCamera.Speed = options.CameraSpeed.Value;
		flyCamera.ControlMode = options.ControlMode.Value;
	}
}
