using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Paradoxlost.UX.WinForms.Theme
{
	using StringDictionary = Dictionary<string, string>;
	using ThemeList = List<ThemeStyle>;

	internal class ThemeParser
	{
		private static void Tokenize(string content, char d1, char d2,
			Action<string, string> action)
		{
			Tokenize(content,
				d1, d2,
				d1p => d1p < 0,
				d2p => d2p < 0,
				action);
		}

		private static void Tokenize(string content, char d1, char d2,
			Func<int, bool> startFail,
			Func<int, bool> endFail,
			Action<string, string> action)
		{
			if (string.IsNullOrWhiteSpace(content))
				return;

			int pos = 0;
			while (pos < content.Length)
			{
				int d1pos = content.IndexOf(d1, pos);
				if (startFail(d1pos))
					break;
				int d2pos = content.IndexOf(d2, d1pos);
				if (endFail(d2pos))
					break;

				string t1 = content.Substring(pos, d1pos - pos).Trim();
				d1pos++;
				string t2 = content.Substring(d1pos, d2pos - d1pos).Trim();

				action(t1, t2);

				pos = d2pos + 1;
			}
		}

		public static ThemeStyle[] ParseResource(string name)
		{
			return ParseResource(name, Assembly.GetCallingAssembly());
		}

		public static ThemeStyle[] ParseResource(string name, Assembly assembly)
		{
			using (Stream stream = assembly.GetManifestResourceStream(name))
			{
				return Parse(stream);
			}
		}

		public static ThemeStyle[] Parse(Stream stream)
		{
			ThemeList styles = new ThemeList();
			StringDictionary variables = new StringDictionary();

			string content = null;
			using (StreamReader reader = new StreamReader(stream))
			{
				content = Sanitize(reader.ReadToEnd());
			}

			if (!string.IsNullOrEmpty(content))
			{
				Tokenize(content, '{', '}',
					(name, rules) =>
					{
						if (name[0] == '@')
						{
							ProcessInstruction(name, rules, styles, variables);
						}
						else
						{
							ThemeStyle style = ProcessStyle(name, rules);
							if (style != null)
							{
								style.Variables = variables;
								styles.Add(style);
							}
						}
					});
			}

			return styles.ToArray();
		}

		private const string CommentsAndLineBreaks = "(/\\*(.|[\r\n])*?\\*/)|(//.*)|([\r\n])*";
		private static string Sanitize(string content)
		{
			Regex replace = new Regex(CommentsAndLineBreaks);
			return replace.Replace(content, string.Empty);
		}

		private static void ProcessInstruction(string name, string content,
			ThemeList styles, StringDictionary variables)
		{
		}

		private static void ParseVariables(StringDictionary variables, string content)
		{
		}

		private static ThemeStyle ProcessStyle(string name, string content)
		{
			if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(content))
				return null;

			ThemeStyle style = new ThemeStyle(name);

			Tokenize(content, ':', ';', (t1, t2) => style.AddProperty(t1, t2));

			return style;
		}
	}
}
