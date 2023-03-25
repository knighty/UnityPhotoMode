using PhotoMode;
using UnityEngine;

public class Clarity : MonoBehaviour
{
	[SerializeField] private float sigma = 10;
	[SerializeField] private ShaderFactory shaderFactory;

	[Range(-1, 3)]
	[SerializeField] private float clarity = 1;

	[Range(-1, 1)]
	[SerializeField] private float vibrance = 0;

	private Shader clarityShader;
	private Material blurMaterial;
	private Material clarityMaterial;

	void Start()
    {
		Init();
    }

	private void Init()
	{
		/*blurMaterial = new Material(blurShader);
		blurMaterial.EnableKeyword("LITTLE_KERNEL");*/

		clarityMaterial = new Material(shaderFactory.ClarityShader);
	}

	private void OnRenderImage(RenderTexture input, RenderTexture output)
	{
		/*RenderTexture temp = RenderTexture.GetTemporary(input.width / 4, input.height / 4, 0);
		blurMaterial.SetFloat("_Sigma", sigma);
		Graphics.Blit(input, temp, blurMaterial);

		clarityMaterial.SetTexture("_BlurTex", temp);*/
		clarityMaterial.SetFloat("_Clarity", clarity);
		clarityMaterial.SetFloat("_Vibrance", vibrance);
		Graphics.Blit(input, output, clarityMaterial);

		//RenderTexture.ReleaseTemporary(temp);
	}
}
