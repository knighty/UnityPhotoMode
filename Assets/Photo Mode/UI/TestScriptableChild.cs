using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PhotoMode.UI
{
	[CreateAssetMenu(fileName = "Test Child", menuName = "Photo Mode/Test Child")]
	public class TestScriptableChild :ScriptableObject
	{
		public int num = 5;
	}
}
