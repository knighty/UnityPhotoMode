using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WindowBackground : MonoBehaviour
{
    [SerializeField] private Graphic image;

    void Update()
    {
        image.material.SetFloat("_UnscaledTime", Time.unscaledTime);
        image.material.SetVector("_MousePosition", new Vector2(Input.mousePosition.x / (float)Screen.width, Input.mousePosition.y / (float)Screen.height));
	}
}
