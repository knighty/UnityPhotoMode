using UnityEngine;
using UnityEngine.UI;

namespace PhotoMode
{
	[CreateAssetMenu(fileName = "Shader Factory", menuName = "Photo Mode/Shader Factory")]
	public class ShaderFactory : ScriptableObject
	{
		[SerializeField] Shader processingShader;
		[SerializeField] Shader outputShader;
		[SerializeField] Shader clarityShader;	

		public Shader ProcessingShader { get => processingShader; }
		public Shader OutputShader { get => outputShader; }
		public Shader ClarityShader { get => clarityShader; }
	}
}