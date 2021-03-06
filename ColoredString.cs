/*!
	Copyright (C) 2006-2013 Kody Brown (kody@bricksoft.com).

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
using System.Text;
using System.Text.RegularExpressions;

namespace Bricksoft.PowerCode
{
	/// <summary>
	/// Provides a way to write a line to the console using several colors, indicated by color tags
	/// For instance: "{Blue}This is blue, {Green}while this is green"
	/// </summary>
	public class ColoredString
	{
		internal ConsoleColor? NotSet = null;

		internal StringBuilder _value { get; set; }
		internal Dictionary<int, ConsoleColor?> _foreColors { get; set; }
		internal Dictionary<int, ConsoleColor?> _backColors { get; set; }

		/// <summary>
		/// Gets the length of the current instance, (without the color info).
		/// </summary>
		public int Length { get { return _value != null ? _value.Length : 0; } }

		#region ----- Constructor(s) -----

		public ColoredString()
		{
			_value = new StringBuilder();
			_foreColors = new Dictionary<int, ConsoleColor?>();
			_backColors = new Dictionary<int, ConsoleColor?>();
		}

		public ColoredString( string Value ) : this() { Append(Value); }

		#endregion

		#region ----- Clear() -----

		public ColoredString Clear()
		{
			_value.Clear();
			_foreColors.Clear();
			_backColors.Clear();
			return this;
		}

		#endregion

		#region ----- Append() -----

		public ColoredString Append( string Format, params object[] Parameters )
		{
			return Append((object)string.Format(Format, Parameters));
		}

		public ColoredString Append( ConsoleColor? ForeColor, string Format, params object[] Parameters )
		{
			return Append((object)string.Format(Format, Parameters), ForeColor, NotSet);
		}

		public ColoredString Append( ConsoleColor? ForeColor, ConsoleColor? BackColor, string Format, params object[] Parameters )
		{
			return Append((object)string.Format(Format, Parameters), ForeColor, BackColor);
		}

		public ColoredString Append( object Value, ConsoleColor? ForeColor = null, ConsoleColor? BackColor = null )
		{
			int len;

			if (Value == null || Value.ToString().Length == 0) {
				return this;
			}

			len = _value.Length;
			_foreColors.Add(len, ForeColor);
			_backColors.Add(len, BackColor);
			_value.Append(Value.ToString());

			return this;
		}

		#endregion

		#region ----- AppendParse() -----

		//public ColoredString AppendParse( string Value )
		//{
		//	if (Value == null || Value.ToString().Length == 0) {
		//		return this;
		//	}

		//	// TODO

		//	return this;
		//}

		#endregion

		#region ----- Insert() -----

		//public ColoredString Insert( int Index, string Value ) { return Insert(Index, Value, NotSet, NotSet); }

		//public ColoredString Insert( int Index, string Value, ConsoleColor? ForeColor ) { return Insert(Index, Value, ForeColor, NotSet); }

		//public ColoredString Insert( int Index, string Value, ConsoleColor? ForeColor, ConsoleColor? BackColor )
		//{
		//	if (Value == null || Value.Length == 0) {
		//		return this;
		//	}

		//	// TODO 
		//	_value.Insert(Index, Value);
		//	_fc.Add(Index, ForeColor);
		//	_bc.Add(Index, BackColor);

		//	return this;
		//}

		#endregion

		#region ----- Parse() -----

		public static ColoredString Parse( string Format, params object[] Parameters )
		{
			string Value;

			if (Parameters != null && Parameters.Length > 0) {
				Value = string.Format(Format, Parameters);
			} else {
				Value = Format;
			}

			return Parse(Value);
		}

		public static ColoredString Parse( string Value )
		{
			ColoredString cs;
			List<string> sections;
			String[] parts;
			int len;

			cs = new ColoredString();

			foreach (String color in Enum.GetNames(typeof(ConsoleColor))) {
				Value = Regex.Replace(Value, @"\{" + color + @"\}", "\r\n@={" + color + @"}", RegexOptions.IgnoreCase);
			}

			sections = new List<string>(Value.Split(new string[] { "\r\n@=" }, StringSplitOptions.RemoveEmptyEntries));

			foreach (String val in sections) {
				parts = val.Split(new Char[] { '}' }, 2);
				parts[0] = parts[0].Substring(1); // remove the preceding '{'

				//coloredStrings.Add(parts[1], (ConsoleColor)Enum.Parse(typeof(ConsoleColor), parts[0])));
				len = cs._value.Length;
				cs._foreColors.Add(len, (ConsoleColor)Enum.Parse(typeof(ConsoleColor), parts[0]));
				cs._backColors.Add(len, null);
				cs._value.Append(parts[1]);
			}

			return cs;
		}

		#endregion

		#region ----- Write() -----

		public void Write()
		{
			ColoredString.Write(this);
		}

		public static void Write( ColoredString coloredString )
		{
			if (coloredString == null || coloredString.Length == 0) {
				return;
			}

			ConsoleColor backupForeColor = Console.ForegroundColor,
						 backupBackColor = Console.BackgroundColor;
			int count = 0;
			string value = coloredString._value.ToString();

			foreach (KeyValuePair<int, ConsoleColor?> pair in coloredString._foreColors) {
				if (pair.Value.HasValue) {
					Console.ForegroundColor = pair.Value.Value;
				}
				if (coloredString._backColors[pair.Key].HasValue) {
					Console.BackgroundColor = coloredString._backColors[pair.Key].Value;
				}

				if (count == 0 && pair.Key > 0) {
					Console.Write(value.Substring(0, pair.Key - 1));
				}

				Console.Write(value.Substring(0, pair.Key));
			}

			Console.ForegroundColor = backupForeColor;
			Console.BackgroundColor = backupBackColor;
		}

		public void WriteLine()
		{
			ColoredString.Write(this);
			Console.WriteLine();
		}

		public static void WriteLine( ColoredString coloredString )
		{
			ColoredString.Write(coloredString);
			Console.WriteLine();
		}

		/* ----- ConsoleColor ----- */

		/// <summary>
		/// Writes the text representation of the specified object and a line terminator to the standard output stream using the specified format information.
		/// </summary>
		/// <param name="foreColor">The ConsoleColor used to display the text. This color only affects this output of text; successive outputs will use the ForegroundColor.</param>
		/// <param name="format">The format string.</param>
		/// <param name="parameters">The object or collection of objects to apply to the format string.</param>
		public static void Write( ConsoleColor foreColor, string format, params object[] parameters ) { Write(foreColor, string.Format(format, parameters)); }

		/// <summary>
		/// Writes the text representation of the specified object and a line terminator to the standard output stream using the specified format information.
		/// </summary>
		/// <param name="foreColor">The ConsoleColor used to display the text. This color only affects this output of text; successive outputs will use the ForegroundColor.</param>
		/// <param name="value">The string to write to the console.</param>
		public static void Write( ConsoleColor foreColor, string value ) { Write(foreColor, ConsoleColor.Black, "{0}", value); }

		/// <summary>
		/// Writes the text representation of the specified object and a line terminator to the standard output stream using the specified format information.
		/// </summary>
		/// <param name="foreColor">The ConsoleColor used to display the text. This color only affects this output of text; successive outputs will use the <seealso cref="ForegroundColor"/>.</param>
		/// <param name="backColor">The ConsoleColor used for the background. This color only affects this output of text; successive outputs will use the <seealso cref="BackgroundColor"/>.</param>
		/// <param name="format">The format string.</param>
		/// <param name="parameters">The object or collection of objects to apply to the format string.</param>
		public static void Write( ConsoleColor foreColor, ConsoleColor backColor, string format, params object[] parameters )
		{
			ConsoleColor backupForeColor;
			ConsoleColor backupBackColor;

			backupForeColor = Console.ForegroundColor;
			backupBackColor = Console.BackgroundColor;
			Console.ForegroundColor = foreColor;
			Console.BackgroundColor = backColor;

			Console.Out.Write(format, parameters);

			Console.ForegroundColor = backupForeColor;
			Console.BackgroundColor = backupBackColor;
		}

		/// <summary>
		/// Writes the text representation of the specified object and a line terminator to the standard output stream using the specified format information.
		/// </summary>
		/// <param name="foreColor">The ConsoleColor used to display the text. This color only affects this output of text; successive outputs will use the ForegroundColor.</param>
		/// <param name="format">The format string.</param>
		/// <param name="parameters">The object or collection of objects to apply to the format string.</param>
		public static void WriteLine( ConsoleColor foreColor, string format, params object[] parameters ) { WriteLine(foreColor, string.Format(format, parameters)); }

		/// <summary>
		/// Writes the text representation of the specified object and a line terminator to the standard output stream using the specified format information.
		/// </summary>
		/// <param name="foreColor">The ConsoleColor used to display the text. This color only affects this output of text; successive outputs will use the ForegroundColor.</param>
		/// <param name="value">The string to write to the console.</param>
		public static void WriteLine( ConsoleColor foreColor, string value ) { WriteLine(foreColor, ConsoleColor.Black, "{0}", value); }

		/// <summary>
		/// Writes the text representation of the specified object and a line terminator to the standard output stream using the specified format information.
		/// </summary>
		/// <param name="foreColor">The ConsoleColor used to display the text. This color only affects this output of text; successive outputs will use the <seealso cref="ForegroundColor"/>.</param>
		/// <param name="backColor">The ConsoleColor used for the background. This color only affects this output of text; successive outputs will use the <seealso cref="BackgroundColor"/>.</param>
		/// <param name="format">The format string.</param>
		/// <param name="parameters">The object or collection of objects to apply to the format string.</param>
		public static void WriteLine( ConsoleColor foreColor, ConsoleColor backColor, string format, params object[] parameters )
		{
			ConsoleColor backupForeColor;
			ConsoleColor backupBackColor;

			backupForeColor = Console.ForegroundColor;
			backupBackColor = Console.BackgroundColor;
			Console.ForegroundColor = foreColor;
			Console.BackgroundColor = backColor;

			Console.Out.WriteLine(format, parameters);

			Console.ForegroundColor = backupForeColor;
			Console.BackgroundColor = backupBackColor;
		}

		//// Note: The ColoredString Write methods are now in the ColoredStringConsoleExtensions class.
		//public void WriteLine( ColoredString value ) { WriteLine(value); }

		#endregion

		#region ColoredStringWrapExtensions

		internal static char[] lineBreaks = new char[] { ' ', '.', ',', '!', '?', ')' };

		/// <summary>
		/// Wraps the specified <paramref name="Value"/> at <paramref name="WrapWidth"/>.
		/// </summary>
		/// <param name="Value">The text to wrap.</param>
		/// <param name="WrapWidth">The maximum number of characters per line.</param>
		/// <returns></returns>
		public string Wrap( int WrapWidth ) { return Wrap(WrapWidth, 0); }

		/// <summary>
		/// Wraps the specified <paramref name="Value"/> at <paramref name="WrapWidth"/>, 
		/// while indenting each line by <paramref name="Indentation"/> spaces.
		/// </summary>
		/// <param name="Value">The text to wrap.</param>
		/// <param name="WrapWidth">The maximum number of characters per line.</param>
		/// <param name="Indentation">The number of spaces to prepend onto each line.</param>
		/// <returns></returns>
		public string Wrap( int WrapWidth, int Indentation ) { return Wrap(WrapWidth, Indentation, false); }

		/// <summary>
		/// Wraps the specified <paramref name="Value"/> at <paramref name="WrapWidth"/>, 
		/// while indenting each line by <paramref name="Indentation"/> spaces.
		/// </summary>
		/// <param name="Value">The text to wrap.</param>
		/// <param name="WrapWidth">The maximum number of characters per line.</param>
		/// <param name="Indentation">The number of spaces to prepend onto each line.</param>
		/// <param name="Reflow">Indicates whether to reflow the text within the string. TODO</param>
		/// <returns></returns>
		public string Wrap( int WrapWidth, int Indentation, bool Reflow )
		{
			List<string> ar;
			string temp;
			int brk;
			int pos;
			string padding;

			WrapWidth = WrapWidth - Indentation;

			if (Reflow) {
				throw new NotImplementedException();
			}

			brk = -1;
			ar = new List<string>(_value.ToString().Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None));
			pos = default(int);
			padding = new string(' ', Indentation);

			for (int i = 0; i < ar.Count; i++) {
				if (ar[i].Length > WrapWidth) {
					temp = ar[i];
					brk = temp.Substring(0, WrapWidth).LastIndexOfAny(lineBreaks) + 1;

					//ar[i] = StringExtensions.RemoveStart(ref temp, space);
					ar[i] = temp.Substring(0, brk);
					temp = temp.Substring(brk);

					pos = 0;
					while (temp.Length > 0) {
						if (temp.Length > WrapWidth) {
							brk = temp.Substring(0, WrapWidth).LastIndexOfAny(lineBreaks) + 1;
							if (brk == 0) {
								// The string could not be broken up nicely, 
								// so just cut the line at the wrap width..
								brk = WrapWidth;
							}
						} else {
							brk = temp.Length;
						}

						ar.Insert(i + (++pos), temp.Substring(0, brk));
						temp = temp.Substring(brk).TrimStart(' ');
					}
				}
			}

			return string.Join("\n" + padding, ar);
		}

		#endregion
	}
}
