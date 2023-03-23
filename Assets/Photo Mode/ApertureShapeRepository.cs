using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PhotoMode
{
	[Serializable]
	[CreateAssetMenu(fileName = "Aperture Repository", menuName = "Photo Mode/Apertures/Repository")]
	public class ApertureShapeRepository : ScriptableObject
	{
		[SerializeField]
		List<Entry> defaultApertures;

		[SerializeField]
		string directory = "/";

		[Serializable]
		public class Entry
		{
			[SerializeField]
			private ApertureShape shape;
			[SerializeField]
			private string name;

			public ApertureShape ApertureShape { get => shape; set => shape = value; }
			public string Name { get => name; set => name = value; }

			public Entry() { }

			public Entry(string name, ApertureShape apertureShape)
			{
				ApertureShape = apertureShape;
				Name = name;
			}
		}

		public IEnumerable<Entry> GetShapes()
		{
			List<Entry> shapes = new List<Entry>(defaultApertures);

			DirectoryInfo d = new DirectoryInfo(directory); //Assuming Test is your Folder
			FileInfo[] Files = d.GetFiles("*.png"); //Getting Text files
			foreach (FileInfo file in Files)
			{
				ImageAperture image = ScriptableObject.CreateInstance<ImageAperture>();
				shapes.Add(new Entry(file.Name, image));
			}

			return shapes;
		}
	}
}