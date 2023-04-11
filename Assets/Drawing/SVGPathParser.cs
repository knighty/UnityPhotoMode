using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;

namespace SVG
{
	public class SVGPathParser
	{
		interface Token
		{

		}

		class CommandToken : Token
		{
			public string command;

			public override string ToString()
			{
				return command.ToString();
			}
		}

		class CommaToken : Token
		{
		}

		class NumberToken : Token
		{
			public float number;

			public override string ToString()
			{
				return number.ToString();
			}
		}

		public SVGPath Parse(string path)
		{
			List<Token> tokens = new List<Token>();
			MatchCollection matches = Regex.Matches(path, @"(?<Number>[+\-]?[ ]?(?:0|[1-9]\d*)(?:\.\d+)?(?:[eE][+\-]?\d+)?)|(?<Command>\w)", RegexOptions.ExplicitCapture);
			foreach (Match m in matches)
			{
				if (!string.IsNullOrEmpty(m.Groups["Number"].Value))
					tokens.Add(new NumberToken() { number = float.Parse(m.Groups["Number"].Value.Replace(" ", "")) * 10 });
				if (!string.IsNullOrEmpty(m.Groups["Command"].Value))
					tokens.Add(new CommandToken() { command = m.Groups["Command"].Value });
			}

			int i = 0;
			SVGPath svgPath = new SVGPath();

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

			bool TokensRemaining()
			{
				return i < tokens.Count;
			}

			string lastCommand = "";

			T NextToken<T>()
			{
				if (!(tokens[i] is T))
					throw new System.Exception($"Invalid svg path. Last command: {tokens[i]}");
				return (T)tokens[i++];
			}

			float NextNumber()
			{
				return NextToken<NumberToken>().number;
			}

			void AddSegment(SVGPathSegment segment, bool relative = false)
			{
				segment.Relative = relative;
				svgPath.Segments.Add(segment);
			}

			while (TokensRemaining())
			{
				string command = "";
				bool incremental = false;
				if (PeekToken<CommandToken>() != null)
				{
					command = NextToken<CommandToken>().command;
				}
				else
				{
					command = lastCommand;
					incremental = true;
				}
				lastCommand = command;
				bool relative = command == command.ToLower();
				switch (command)
				{
					case "M":
					case "m":
						{
							AddSegment(new SVGMoveToSegment(NextNumber(), NextNumber()), relative);
						}
						break;
					case "L":
					case "l":
						{
							AddSegment(new SVGLineSegment(NextNumber(), NextNumber()), relative);
						}
						break;
					case "H":
					case "h":
						{
							AddSegment(new SVGHorizontalLineSegment(NextNumber()), relative);
						}
						break;
					case "V":
					case "v":
						{
							AddSegment(new SVGVerticalLineSegment(NextNumber()), relative);
						}
						break;
					case "C":
					case "c":
						{
							AddSegment(new SVGBezierSegment(NextNumber(), NextNumber(), NextNumber(), NextNumber(), NextNumber(), NextNumber()), relative);
						}
						break;
					case "S":
					case "s":
						{
							AddSegment(new SVGBezierSegment(NextNumber(), NextNumber(), NextNumber(), NextNumber()), relative);
						}
						break;
					case "z":
					case "Z":
						{
							AddSegment(new SVGEndPathSegment());
						}
						break;
					default:
						{
							while (TokensRemaining() && PeekToken<CommandToken>() == null)
							{
								AdvanceToken();
							}
						}
						break;
				}
			}

			return svgPath;
		}
	}
}
