using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PhotoMode.UI
{
	[CreateAssetMenu(fileName = "Test Scriptable", menuName = "Photo Mode/Test Scriptable")]
	public class TestScriptable : ScriptableObject
	{
		public TestScriptableChild child;
	}
}
