using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Drawing;
using UnityEngine;
using System.Collections.Specialized;

namespace PhotoMode
{
	public interface FilePhotoModeOutputPathTokenHandler
	{
		string GetToken(string token);
		string[] Tokens { get; }
	}

	public class DateTimeFilePhotoModeOutputPathTokenHandler : FilePhotoModeOutputPathTokenHandler
	{
		public string[] Tokens => new string[] { "date", "time" };

		public string GetToken(string token)
		{
			switch (token)
			{
				case "date":
					{
						DateTime currentTime = DateTime.Now;
						return $"{currentTime.Year}-{currentTime.Month}-{currentTime.Day}";
					}

				case "time":
					{
						DateTime currentTime = DateTime.Now;
						return $"{currentTime.Hour}-{currentTime.Minute}-{currentTime.Second}";
					}
			}

			return token;
		}
	}

	public class FilePhotoModeOutput : PhotoModeOutput
	{
		private string filePath;
		private SaveTextureToFileUtility.SaveTextureFileFormat fileFormat = SaveTextureToFileUtility.SaveTextureFileFormat.PNG;

		public RenderTextureFormat RenderTextureFormat => RenderTextureFormat.Default;

		private Dictionary<string, FilePhotoModeOutputPathTokenHandler> tokenHandlers = new Dictionary<string, FilePhotoModeOutputPathTokenHandler>();

		public string Filename
		{
			get => filePath;
			set => filePath = value;
		}

		public FilePhotoModeOutput(string filename, SaveTextureToFileUtility.SaveTextureFileFormat fileFormat, params FilePhotoModeOutputPathTokenHandler[] tokenHandlers)
		{
			this.filePath = filename;
			this.fileFormat = fileFormat;

			foreach (var handler in tokenHandlers)
			{
				AddTokenHandler(handler);
			}
		}

		public void AddTokenHandler(FilePhotoModeOutputPathTokenHandler handler)
		{
			foreach (var token in handler.Tokens)
			{
				tokenHandlers.Add(token, handler);
			}
		}

		private string MatchToken(Match match)
		{
			string token = match.Groups[1].Value;
			if (tokenHandlers.TryGetValue(token, out var result))
			{
				return result.GetToken(token);
			}
			return "%" + token + "%";
		}

		public void Output(RenderTexture texture)
		{
			string file = Regex.Replace(filePath, "%([a-z\\-]*)%", MatchToken);
			SaveTextureToFileUtility.SaveRenderTextureToFile(texture, file, fileFormat);

			/*Texture2D tex;
			tex = new Texture2D(texture.width, texture.height, TextureFormat.ARGB32, false, true);
			var oldRt = RenderTexture.active;
			RenderTexture.active = texture;
			tex.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
			tex.Apply();

			Stream s = new MemoryStream(texture.width * texture.height);
			byte[] bits = tex.EncodeToJPG();
			s.Write(bits, 0, bits.Length);
			System.Drawing.Image image = Image.FromStream(s);
			Clipboard.SetImage(image);
			s.Dispose();*/

			string path = Path.GetFullPath(file) + ".jpg";
			Debug.Log(path);
			StringCollection strcolFileList = new StringCollection
			{
				path
			};
			Clipboard.SetFileDropList(strcolFileList);
		}
	}
}