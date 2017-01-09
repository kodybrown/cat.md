/*!
	Copyright (C) 2008-2013 Kody Brown (kody@bricksoft.com).

	MIT License:

	Permission is hereby granted, free of charge, to any person obtaining a copy
	of this software and associated documentation files (the "Software"), to
	deal in the Software without restriction, including without limitation the
	rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
	sell copies of the Software, and to permit persons to whom the Software is
	furnished to do so, subject to the following conditions:

	The above copyright notice and this permission notice shall be included in
	all copies or substantial portions of the Software.

	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
	FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
	DEALINGS IN THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Bricksoft.PowerCode;

namespace cat.md
{
	public class md : ICataloger
	{
		public string Name { get { return _name; } }
		private string _name = "markdown";

		public string Description { get { return _description; } }
		private string _description = "Formatted Markdown (.md).";

		public bool CanCat( CatOptions catOptions, string fileName )
		{
			return Path.GetExtension(fileName).Equals(".md", StringComparison.CurrentCultureIgnoreCase);
		}

		public bool Cat( CatOptions catOptions, string fileName )
		{
			return Cat(catOptions, fileName, 0, long.MaxValue);
		}

		public bool Cat( CatOptions catOptions, string fileName, int lineStart, long linesToWrite )
		{
			MdConfig mdc = new MdConfig();

			string[] allLines;
			List<string> lines;
			string lastType = "";
			int lineNumber;
			int padLen;
			int width = Console.WindowWidth - 1;
			string colPad;
			Regex urlRegex, boldRegex;
			Match m;

			allLines = File.ReadAllLines(fileName);
			lineStart = Math.Max(lineStart, 0);
			lineNumber = 0;
			padLen = catOptions.showLineNumbers ? 3 : 0;
			if (linesToWrite < 0) {
				linesToWrite = long.MaxValue;
			}
			colPad = string.Empty;

			urlRegex = new Regex(@"\[(?<url>.+)\]\((?<title>.*)\)", RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline);
			boldRegex = new Regex(@"__(?<text>[^_]+)__", RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline);

			//if ((m = urlRegex.Match("some content here.. Here is an [http://google.com/file](url)..")).Success) {
			//	Console.Write("found a match!");
			//	Console.Write(m.Groups.Count);
			//}

			Console.ForegroundColor = mdc.defaultColor;
			Console.BackgroundColor = mdc.defaultBackground;

			lines = new List<string>();
			for (int lineIndex = Math.Min(allLines.Length, Math.Max(0, lineStart)); lineIndex < Math.Min(lineStart + linesToWrite, allLines.Length); lineIndex++) {
				lines.Add(allLines[lineIndex]);
			}

			for (int lineIndex = 0; lineIndex < lines.Count; lineIndex++) {
				string l = lines[lineIndex];
				lineNumber++;

				if (catOptions.showLineNumbers) {
					Console.BackgroundColor = catOptions.lineNumBackColor;
					Console.ForegroundColor = catOptions.lineNumForeColor;
					Console.Write("{0," + padLen + "}", lineNumber);
					Console.BackgroundColor = catOptions.defaultBackColor;
					Console.ForegroundColor = catOptions.defaultForeColor;
				}

				if (l.StartsWith("####")) {
					// h4 and above use the same style..
					Console.ForegroundColor = mdc.h4;
					Console.WriteLine(l);
					Console.ForegroundColor = mdc.defaultColor;
					lastType = "h4";
					continue;
				}
				if (l.StartsWith("###")) {
					Console.ForegroundColor = mdc.h3;
					Console.WriteLine(l);
					Console.ForegroundColor = mdc.defaultColor;
					lastType = "h3";
					continue;
				}
				if (l.StartsWith("##")) {
					Console.ForegroundColor = mdc.h2;
					Console.WriteLine(l);
					Console.ForegroundColor = mdc.defaultColor;
					lastType = "h2";
					continue;
				}
				if (l.StartsWith("#")) {
					Console.ForegroundColor = mdc.h1;
					Console.WriteLine(l);
					Console.ForegroundColor = mdc.defaultColor;
					lastType = "h1";
					continue;
				}

#if false
				ColoredString s = new ColoredString();
				bool modified = true;
				bool urlRegexDone = false;

				//s = ColoredString.Parse(l);

				//while (modified) {
				//	modified = false;

				//	if (!urlRegexDone) {
				//		m = urlRegex.Match(s._value.ToString());
				//		if (m.Success) {
				//			//Console.ForegroundColor = mdc.debugColor;
				//			//Console.WriteLine(">" + l);
				//			//Console.WriteLine();
				//			//for (int i = 0; i < m.Groups.Count; i++) {
				//			//	Console.WriteLine(">" + i + " " + m.Groups[i]);
				//			//}
				//			//Console.WriteLine();
				//			//foreach (Capture c in m.Captures) {
				//			//	Console.WriteLine(">" + c.Index + " " + c.Value);
				//			//}
				//			//Console.WriteLine();
				//			//Console.WriteLine(">" + m.Index + " " + m.Value);

				//			//Console.ForegroundColor = mdc.defaultColor;
				//			//Console.Write(l.Substring(0, m.Index));
				//			//Console.ForegroundColor = mdc.linkSymbols;
				//			//Console.Write("[");
				//			//Console.ForegroundColor = mdc.linkUrl;
				//			//Console.Write(m.Groups[1]);
				//			//Console.ForegroundColor = mdc.linkSymbols;
				//			//Console.Write("](");
				//			//Console.ForegroundColor = mdc.linkTitle;
				//			//Console.Write(m.Groups[2]);
				//			//Console.ForegroundColor = mdc.linkSymbols;
				//			//Console.Write(")");
				//			//Console.ForegroundColor = mdc.defaultColor;
				//			//Console.Write(l.Substring(m.Index + m.Groups[1].Length + m.Groups[2].Length + 4));

				//			//s.Clear()
				//			// .Append("{").Append(mdc.defaultColor).Append("}")
				//			// .Append(l.Substring(0, m.Index))
				//			// .Append('{').Append(mdc.linkSymbols).Append("}")
				//			// .Append('[')
				//			// .Append('{').Append(mdc.linkUrl).Append("}")
				//			// .Append(m.Groups[1])
				//			// .Append('{').Append(mdc.linkSymbols).Append("}")
				//			// .Append("](")
				//			// .Append('{').Append(mdc.linkTitle).Append("}")
				//			// .Append(m.Groups[2])
				//			// .Append('{').Append(mdc.linkSymbols).Append("}")
				//			// .Append(")")
				//			// .Append('{').Append(mdc.defaultColor).Append("}")
				//			// .Append(l.Substring(m.Index + m.Groups[1].Length + m.Groups[2].Length + 4));

				//			s.Clear()
				//			 .Append(l.Substring(0, m.Index), mdc.defaultColor)
				//			 .Append('[', mdc.linkSymbols)
				//			 .Append(m.Groups[1], mdc.linkUrl)
				//			 .Append("](", mdc.linkSymbols)
				//			 .Append(m.Groups[2], mdc.linkTitle)
				//			 .Append(")", mdc.linkSymbols)
				//			 .Append(l.Substring(m.Index + m.Groups[1].Length + m.Groups[2].Length + 4), mdc.defaultColor);

				//			l = s.ToString();
				//			modified = true;
				//		}

				//		urlRegexDone = true;
				//	}
				//}
#endif

				string lt = l.Trim();

				if (lt.Length == 0) {
					if (lastType == "code" && (lineIndex < lines.Count - 1 && lines[lineIndex + 1].StartsWith("    "))) {
						l = "    ";
					} else {
						Console.WriteLine(l);
						lastType = "";
						continue;
					}
				}

				if (l.StartsWith("    ")) {
					Console.BackgroundColor = mdc.codePrefixBackground;
					Console.Write("    ");
					Console.BackgroundColor = mdc.defaultBackground;

					l = l.Substring(4);
					lt = l.Trim();
					//Console.ForegroundColor = mdc.debugColor;
					//Console.WriteLine("'" + l + "'");

					if (lt.StartsWith(">") || lt.StartsWith("$")) {
						int i = l.IndexOfAny(new char[] { '>', '$' });
						Console.ForegroundColor = mdc.codeConsoleCmdSymbol;
						Console.Write(l.Substring(0, i + 1));
						Console.ForegroundColor = mdc.defaultColor;
						Console.WriteLine(l.Substring(i + 1));
					} else {
						Console.WriteLine(l);
					}

					lastType = "code";
					continue;
				}

				m = urlRegex.Match(l);
				if (m.Success) {
					//Console.ForegroundColor = mdc.debugColor;
					//Console.WriteLine(">" + l);
					//Console.WriteLine();
					//for (int i = 0; i < m.Groups.Count; i++) {
					//	Console.WriteLine(">" + i + " " + m.Groups[i]);
					//}
					//Console.WriteLine();
					//foreach (Capture c in m.Captures) {
					//	Console.WriteLine(">" + c.Index + " " + c.Value);
					//}
					//Console.WriteLine();
					//Console.WriteLine(">" + m.Index + " " + m.Value);

					Console.ForegroundColor = mdc.defaultColor;
					Console.Write(l.Substring(0, m.Index));
					Console.ForegroundColor = mdc.linkSymbols;
					Console.Write("[");
					Console.ForegroundColor = mdc.linkUrl;
					Console.Write(m.Groups[1]);
					Console.ForegroundColor = mdc.linkSymbols;
					Console.Write("](");
					Console.ForegroundColor = mdc.linkTitle;
					Console.Write(m.Groups[2]);
					Console.ForegroundColor = mdc.linkSymbols;
					Console.Write(")");
					Console.ForegroundColor = mdc.defaultColor;
					Console.Write(l.Substring(m.Index + m.Groups[1].Length + m.Groups[2].Length + 4));

					lastType = "url";
					continue;
				}

				m = boldRegex.Match(l);
				if (m.Success) {
					//foreach (Capture c in m.Captures) {
					//	Console.WriteLine(">" + c.Index + " " + c.Value);
					//}

					Console.ForegroundColor = mdc.defaultColor;
					Console.Write(Text.Wrap(l.Substring(0, m.Index), width, 0));
					Console.ForegroundColor = mdc.emphasisText;
					Console.Write(m.Groups[1]);
					Console.ForegroundColor = mdc.defaultColor;
					Console.Write(Text.Wrap(l.Substring(m.Index + m.Groups[1].Length + 4), new int[] { width - Console.CursorLeft, width }, new int[] { 0 }));

					lastType = "bold";
					continue;
				}

				int indentation;
				if (l.StartsWith(" ")) {
					indentation = 0;
					foreach (char c in l) {
						if (c == ' ') {
							indentation++;
						} else {
							break;
						}
					}
				} else {
					indentation = 0;
				}

				Console.WriteLine(Text.Wrap(l, width, indentation));
				lastType = "text";
			}

			Console.ForegroundColor = mdc.originalColor;
			Console.BackgroundColor = mdc.originalBackground;

			return true;
		}
	}

	public class MdConfig
	{
		public ConsoleColor debugColor { get; set; }

		public ConsoleColor defaultColor { get; set; }
		public ConsoleColor originalColor { get; set; }
		public ConsoleColor defaultBackground { get; set; }
		public ConsoleColor originalBackground { get; set; }

		public ConsoleColor h1 { get; set; }
		public ConsoleColor h2 { get; set; }
		public ConsoleColor h3 { get; set; }
		public ConsoleColor h4 { get; set; }

		public ConsoleColor linkSymbols { get; set; }
		public ConsoleColor linkUrl { get; set; }
		public ConsoleColor linkTitle { get; set; }

		public ConsoleColor emphasisText { get; set; }

		public ConsoleColor codePrefixBackground { get; set; }
		public ConsoleColor codeConsoleCmdSymbol { get; set; }

		public MdConfig()
		{
			debugColor = ConsoleColor.Yellow;

			originalColor = Console.ForegroundColor;
			originalBackground = Console.BackgroundColor;

			// TODO load these settings from a .config file
			defaultColor = ConsoleColor.Gray;
			defaultBackground = ConsoleColor.Black;

			h1 = ConsoleColor.Cyan;
			h2 = ConsoleColor.DarkCyan;
			h3 = ConsoleColor.DarkCyan;
			h4 = ConsoleColor.DarkCyan;

			linkSymbols = ConsoleColor.DarkBlue;
			linkUrl = defaultColor;
			linkTitle = ConsoleColor.Cyan;

			emphasisText = ConsoleColor.White;

			codePrefixBackground = ConsoleColor.Black;
			codeConsoleCmdSymbol = ConsoleColor.DarkGreen;
		}
	}
}
