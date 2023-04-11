using Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
public class PolygonTest : Graphic
{
	[SerializeField] Shader shader;
	[SerializeField, Range(0, 200)] int bail = 0;
	[SerializeField, Range(0, 20)] float strokeWidth;
	[SerializeField] Color strokeColor;

	class PathCommand
	{
		string command;
	}

	class MoveCommand
	{
		public Vector2 point;
	}

	interface Token
	{

	}

	class CommandToken : Token
	{
		public string command;
	}

	class CommaToken : Token
	{
	}

	class NumberToken : Token
	{
		public float number;
	}

	protected override void OnPopulateMesh(VertexHelper vh)
	{
		PolygonRenderer polygonRenderer = new PolygonRenderer();

		/*Vector2[] vertices = new Vector2[100];
		for(int i = 0; i < 98; i++)
		{
			float t = i / 98.0f;
			vertices[i] = new Vector2(t * 500, Perlin.Sample(t, 0, 4, 0.5f) * 1500 + 500);
		}
		vertices[98] = new Vector2(0, 0);
		vertices[98] = new Vector2(500, 0);*/

		string path = "M620,305.666v-51.333l-31.5-5.25c-2.333-8.75-5.833-16.917-9.917-23.917L597.25,199.5l-36.167-36.75l-26.25,18.083c-7.583-4.083-15.75-7.583-23.916-9.917L505.667,140h-51.334l-5.25,31.5c-8.75,2.333-16.333,5.833-23.916,9.916L399.5,163.333L362.75,199.5l18.667,25.666c-4.083,7.584-7.583,15.75-9.917,24.5l-31.5,4.667v51.333l31.5,5.25c2.333,8.75,5.833,16.334,9.917,23.917l-18.67,26.25l36.167,36.167l26.25-18.667c7.583,4.083,15.75,7.583,24.5,9.917l5.25,30.916h51.333l5.25-31.5c8.167-2.333,16.333-5.833,23.917-9.916l26.25,18.666l36.166-36.166l-18.666-26.25c4.083-7.584,7.583-15.167,9.916-23.917L620,305.666z M480,333.666c-29.75,0-53.667-23.916-53.667-53.666s24.5-53.667,53.667-53.667S533.667,250.25,533.667,280S509.75,333.666,480,333.666z";

		string basketball = "M20.161,2.273C16.233,3.835,12.553,6.2,9.375,9.378C9.19,9.562,9.018,9.755,8.838,9.942		c1.386,3.831,3.126,7.562,5.242,11.133C18.139,15.485,20.168,8.879,20.161,2.273z" +
	"M63.824,28.682c-9.751-1.521-20.067,1.469-27.58,8.981c-1.202,1.202-2.287,2.479-3.258,3.812		c7.363,5.18,15.545,8.678,24.021,10.495C62.398,45.233,64.671,36.827,63.824,28.682z" +
	"M27.05,63.616c9.75,1.519,20.067-1.471,27.58-8.983c0.308-0.308,0.602-0.624,0.894-0.94		c - 8.344 - 1.927 - 16.387 - 5.451 - 23.659 - 10.562C27.924,49.32,26.316,56.563,27.05,63.616z" +
	"M12.771,22.754c-2.172-3.571-3.98-7.297-5.435-11.13c-4.895,5.909-7.34,13.163-7.332,20.416		c3.926 - 1.562,7.607 - 3.928,10.785 - 7.105C11.491,24.232,12.147,23.503,12.771,22.754z" +
	"M13.881,24.53c-0.535,0.62-1.09,1.231-1.678,1.819c-3.554,3.553-7.701,6.146-12.121,7.798		c0.497,7.462,3.59,14.782,9.293,20.485c0.027,0.028,0.057,0.054,0.085,0.082c2.091 - 7.833,5.595 - 15.364,10.528 - 22." +
		"C17.73,29.981,15.691,27.309,13.881,24.53z" +
	"M30.236,41.961c-2.588-1.935-5.074-4.067-7.427-6.42c-0.493-0.493-0.958-1.003-1.433-1.507		c - 4.874,6.828 - 8.298,14.369 - 10.258,22.207c4.102,3.541,8.884,5.869,13.886,6.984C24.372,55.889,26.117,48.385,30.236,41.961z" +
	"M22.583,32.401c0.54,0.579,1.076,1.161,1.641,1.726c2.262,2.262,4.653,4.313,7.14,6.18		c1.033 - 1.42,2.187 - 2.778,3.467 - 4.058c7.829 - 7.829,18.525 - 11.037,28.708 - 9.635c - 1.062 - 6.245 - 3.979 - 12.235 - 8.761 - 17" +
		"c - 9.619,3.111 - 18.671,8.468 - 26.311,16.107C26.321,27.787,24.371,30.052,22.583,32.401z" +
	"M15.215,22.892c1.773,2.78,3.774,5.456,6.007,8.002c1.78-2.312,3.711-4.546,5.831-6.666		c7.594 - 7.595,16.541 - 13.007,26.063 - 16.257c - 8.688 - 7.646 - 20.523 - 9.78 - 30.973 - 6.405C22.307,9.073,19.998,16.62,15.215,22.892z";

		string envelope = "M43.416,35.812c - 0.448 - 0.322 - 0.55 - 0.947 - 0.228 - 1.396c0.321 - 0.448,0.947 - 0.55,1.396 - 0.228L64,48.168V15.475" +
		"L32.597,38.803C32.419,38.935,32.21,39,32,39s - 0.419 - 0.065 - 0.597 - 0.197L0,15.475v32.693l19.416 - 13.979" +
		"c0.449 - 0.321,1.073 - 0.22,1.396,0.228c0.322,0.448,0.221,1.073 - 0.228,1.396L0,50.632V52c0,2.211,1.789,4,4,4h56c2.211,0,4 - 1.789,4 - 4" +
		"v - 1.368L43.416,35.812zM32,36.754l32-23.771V12c0-2.211-1.789-4-4-4H4c-2.211,0-4,1.789-4,4v0.982L32,36.754z";

		string bus = "M64,18.999c0-0.553-0.447-1-1-1h-3V10c0-5.523-4.478-10-10-10H14C8.477,0,4,4.478,4,10v7.999H1" +
		"c - 0.553,0 - 1,0.447 - 1,1v9L0.003,28H0c0,2.209,1.791,4,4,4v20c0,2.211,1.789,4,4,4v7c0,0.553,0.447,1,1,1h8c0.553,0,1 - 0.447,1 - 1v - 7" +
		"h28v7c0,0.553,0.447,1,1,1h8c0.553,0,1 - 0.447,1 - 1v - 7c2.211,0,4 - 1.789,4 - 4V32c2.209,0,4 - 1.791,4 - 4h - 0.003L64,27.999V18.999z M4,30" +
		"c - 1.104,0 - 2 - 0.896 - 2 - 2v - 8.001h2V30z M28,5h8c0.553,0,1,0.447,1,1s - 0.447,1 - 1,1h - 8c - 0.553,0 - 1 - 0.447 - 1 - 1S27.447,5,28,5z M16,62h - 6" +
		"v - 6h6V62z M13,50c - 2.209,0 - 4 - 1.791 - 4 - 4s1.791 - 4,4 - 4s4,1.791,4,4S15.209,50,13,50z M16,36h - 6V12h21v24h - 3c0 - 3.314 - 2.686 - 6 - 6 - 6" +
		"S16,32.686,16,36z M22,32c2.209,0,4,1.791,4,4h - 8C18,33.791,19.791,32,22,32z M44,50c0,0.553 - 0.447,1 - 1,1H21c - 0.553,0 - 1 - 0.447 - 1 - 1" +
		"v - 8c0 - 0.553,0.447 - 1,1 - 1h22c0.553,0,1,0.447,1,1V50z M33,36V12h21v24H33z M54,62h - 6v - 6h6V62z M51,50c - 2.209,0 - 4 - 1.791 - 4 - 4" +
		"s1.791 - 4,4 - 4s4,1.791,4,4S53.209,50,51,50z M62,28c0,1.104 - 0.896,2 - 2,2V19.999h2V28z" +
	"M49.707,14.294c-0.391-0.391-1.023-0.391-1.414,0s-0.391,1.023,0,1.414l2,2" +
		"c0.195,0.195,0.451,0.293,0.707,0.293s0.512 - 0.098,0.707 - 0.293c0.391 - 0.391,0.391 - 1.023,0 - 1.414L49.707,14.294z" +
	"M44.707,14.293c-0.391-0.391-1.023-0.391-1.414,0s-0.391,1.023,0,1.414l7,7C50.488,22.902,50.744,23,51,23" +
		"s0.512 - 0.098,0.707 - 0.293c0.391 - 0.391,0.391 - 1.023,0 - 1.414L44.707,14.293z";

		path = basketball; 

		List<Token> tokens = new List<Token>();
		MatchCollection matches = Regex.Matches(path, @"(?<Number>\-?[ ]*[\d.]+)|(?<Command>\w)", RegexOptions.ExplicitCapture);
		foreach (Match m in matches)
		{
			if (!string.IsNullOrEmpty(m.Groups["Number"].Value))
				tokens.Add(new NumberToken() { number = float.Parse(m.Groups["Number"].Value.Replace(" ", "")) * 10 });
			if (!string.IsNullOrEmpty(m.Groups["Command"].Value))
				tokens.Add(new CommandToken() { command = m.Groups["Command"].Value });
		}

		int i = 0;
		Vector2 pos = Vector2.zero;
		List<Vector2[]> paths = new List<Vector2[]>();
		List<Vector2> vertices = new List<Vector2>();

		T PeekToken<T>()
		{
			if (tokens[i] is T)
				return (T)tokens[i];
			return default(T);
		}

		void AdvanceToken()
		{
			i++;
		}

		T NextToken<T>()
		{
			if (!(tokens[i] is T))
				throw new System.Exception("Invalid svg path");
			return (T)tokens[i++];
		}

		bool TokensRemaining()
		{
			return i < tokens.Count;
		}

		void AddVertex(Vector2 vertex)
		{
			if (vertex != pos)
			{
				pos = vertex;
				vertices.Add(new Vector2(vertex.x, -vertex.y));
			}
		}

		while (TokensRemaining())
		{
			CommandToken token = NextToken<CommandToken>();
			switch (token.command)
			{
				case "M":
					{
						pos = Vector2.zero;
						vertices = new List<Vector2>();
						AddVertex(new Vector2(NextToken<NumberToken>().number, NextToken<NumberToken>().number));
					}
					break;
				case "L":
					{
						AddVertex(new Vector2(NextToken<NumberToken>().number, NextToken<NumberToken>().number));
					}
					break;
				case "l":
					{
						AddVertex(new Vector2(pos.x + NextToken<NumberToken>().number, pos.y + NextToken<NumberToken>().number));
					}
					break;
				case "H":
					{
						AddVertex(new Vector2(NextToken<NumberToken>().number, pos.y));
					}
					break;
				case "h":
					{
						AddVertex(new Vector2(pos.x + NextToken<NumberToken>().number, pos.y));
					}
					break;
				case "V":
					{
						AddVertex(new Vector2(pos.x, NextToken<NumberToken>().number));
					}
					break;
				case "v":
					{
						AddVertex(new Vector2(pos.x, NextToken<NumberToken>().number + pos.y));
					}
					break;
				case "C":
					{
						Vector2 a = pos;
						Vector2 b = new Vector2(NextToken<NumberToken>().number, NextToken<NumberToken>().number);
						Vector2 c = new Vector2(NextToken<NumberToken>().number, NextToken<NumberToken>().number);
						Vector2 d = new Vector2(NextToken<NumberToken>().number, NextToken<NumberToken>().number);

						int numCurvePoints = 1 + (int)((d - a).magnitude / 20);
						for (int ii = 0; ii <= numCurvePoints; ii++)
						{
							float t = ii / (float)numCurvePoints;
							Vector2 aa = Vector2.Lerp(a, b, t);
							Vector2 bb = Vector2.Lerp(b, c, t);
							Vector2 cc = Vector2.Lerp(c, d, t);
							Vector2 aaa = Vector2.Lerp(aa, bb, t);
							Vector2 bbb = Vector2.Lerp(bb, cc, t);
							Vector2 p = Vector2.Lerp(aaa, bbb, t);
							AddVertex(p);
						}
					}
					break;
				case "c":
					{
						Vector2 a = pos;
						Vector2 b = new Vector2(a.x + NextToken<NumberToken>().number, a.y + NextToken<NumberToken>().number);
						Vector2 c = new Vector2(a.x + NextToken<NumberToken>().number, a.y + NextToken<NumberToken>().number);
						Vector2 d = new Vector2(a.x + NextToken<NumberToken>().number, a.y + NextToken<NumberToken>().number);

						int numCurvePoints = 1 + (int)((d - a).magnitude / 20);
						for (int ii = 0; ii <= numCurvePoints; ii++)
						{
							float t = ii / (float)numCurvePoints;
							Vector2 aa = Vector2.Lerp(a, b, t);
							Vector2 bb = Vector2.Lerp(b, c, t);
							Vector2 cc = Vector2.Lerp(c, d, t);
							Vector2 aaa = Vector2.Lerp(aa, bb, t);
							Vector2 bbb = Vector2.Lerp(bb, cc, t);
							Vector2 p = Vector2.Lerp(aaa, bbb, t);
							AddVertex(p);
						}
					}
					break;
				case "S":
					{
						AdvanceToken();
						AdvanceToken();
						AddVertex(new Vector2(NextToken<NumberToken>().number, NextToken<NumberToken>().number));
					}
					break;
				case "s":
					{
						AdvanceToken();
						AdvanceToken();
						AddVertex(new Vector2(pos.x + NextToken<NumberToken>().number, pos.y + NextToken<NumberToken>().number));
					}
					break;
				case "z":
					{
						paths.Add(FilterVertices(vertices));
						//Debug.Log($"Added path with {vertices.Count} vertices");
					}
					break;
				default:
					{
						//Debug.Log($"Unhandled SVG command: {token.command}");
						while (TokensRemaining() && PeekToken<CommandToken>() == null)
						{
							AdvanceToken();
						}
					}
					break;
			}
		}

		vh.Clear();

		FillPaint fill = new FillPaint()
		{
			Color = color
		};
		StrokePaint stroke = new StrokePaint()
		{

		};

		foreach (var p in paths)
		{
			polygonRenderer.RenderOutline(p, vh, strokeWidth * 2, 2);
		}
		foreach (var p in paths)
		{
			polygonRenderer.RenderFill(fill, p, vh, 0);
		}
		foreach (var p in paths)
		{
			//polygonRenderer.RenderOutline(p, vh, strokeWidth, 1);
		}
		//Debug.Log(paths[1].ToArray().Aggregate("", (v, c) => c + v.ToString() + ","));
		//polygonRenderer.Render(null, null, paths[0].ToArray(), vh);
	}

	protected Vector2[] FilterVertices(List<Vector2> vertices)
	{
		List<Vector2> filteredVertices = new List<Vector2>();
		//filteredVertices.Capacity = vertices.Length;
		Vector2 previous = vertices.Last();
		for (int j = 0; j < vertices.Count; j++)
		{
			Vector2 vertex = vertices[j];
			if (vertex == previous) continue;
			filteredVertices.Add(vertex);
			previous = vertex;
		}
		return filteredVertices.ToArray();
	}

	protected override void OnValidate()
	{
		//material = new Material(shader);
		//SetMaterialDirty();
		SetVerticesDirty();

		List<Vector4> layerParams = new List<Vector4>();
		layerParams.Add(new Vector4(0, 0, 0, 0));
		layerParams.Add(new Vector4(1, strokeWidth, 0, 0));
		layerParams.Add(new Vector4(2, strokeWidth * 2, 0, 0));
		material.SetVectorArray("layerParams", layerParams);

		List<Vector4> layerParams2 = new List<Vector4>();
		layerParams2.Add(color);
		layerParams2.Add(strokeColor);
		layerParams2.Add(Color.black);
		material.SetVectorArray("layerParams2", layerParams2);
	}
}

