using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PhotoMode.UI
{
	public interface TransitionChild
	{
		public string State { get; }
		public void UpdateState(float value);
	}
}
