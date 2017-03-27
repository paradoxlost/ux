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
	using Util;

	internal class ThemeParser
	{
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
            ThemeParserContext context = ThemeParserContext.Default;

			string content = null;
			using (StreamReader reader = new StreamReader(stream))
			{
				content = Sanitize(reader.ReadToEnd());
			}

			if (!string.IsNullOrEmpty(content))
			{
				content.Tokenize('{', '}',
					(name, rules) =>
					{
						if (name[0] == '@')
						{
							ProcessInstruction(name, rules, styles, variables, ref context);
						}
						else
						{
							ThemeStyle style = ProcessStyle(name, rules, variables, context);
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
			ThemeList styles, StringDictionary variables, ref ThemeParserContext context)
		{
            switch (name)
            {
                case "@var":
                    ParseVariables(variables, content);
                    break;

                case "@include":
                    break;

                case "@module":
                    ThemeParserContext tmp = ParseContext(content);
                    if (tmp != null && tmp != context)
                        context = tmp;
                    break;
            }
		}

        private static ThemeParserContext ParseContext(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new InvalidOperationException("@module section cannot be empty");

            ThemeParserContext result = null;

            content.ParseKeyValuePair(
                (key, value) =>
                {
                    if (string.Compare(key, "Assembly", true) == 0)
                    {
                        result = new ThemeParserContext(Assembly.Load(value));
                    }
                });
            return result;
        }

		private static void ParseVariables(StringDictionary variables, string content)
		{
            if (string.IsNullOrWhiteSpace(content))
                return;
            content.ParseKeyValuePair((t1, t2) => variables.Add(t1, t2));
		}

		private static ThemeStyle ProcessStyle(string name, string content, StringDictionary variables, ThemeParserContext context)
		{
			if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(content))
				return null;

			ThemeStyle style = new ThemeStyle(context, name);

            content.ParseKeyValuePair((t1, t2) => style.AddProperty(t1, t2));

			return style;
		}
	}
}
