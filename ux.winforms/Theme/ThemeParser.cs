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
    using ContextDictionary = Dictionary<string, ThemeParserContext>;
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
            ContextDictionary context = new ContextDictionary();
            context.Add("Default", ThemeParserContext.Default);

			string content = null;
			using (StreamReader reader = new StreamReader(stream))
			{
				content = Sanitize(reader.ReadToEnd());
			}

			if (!string.IsNullOrEmpty(content))
			{
                ProcessRuleset(content, context, styles, null);
			}

			return styles.ToArray();
		}

		private const string CommentsAndLineBreaks = "(/\\*(.|[\r\n])*?\\*/)|(//.*)|([\r\n])*";
		private static string Sanitize(string content)
		{
			Regex replace = new Regex(CommentsAndLineBreaks);
			return replace.Replace(content, string.Empty);
		}

        private static void ProcessRuleset(string content, ContextDictionary context, ThemeList styles, ThemeStyle parent)
        {
            StringDictionary variables = new StringDictionary();

            content.Tokenize('{', '}',
                (name, rules) =>
                {
                    if (name[0] == '@')
                    {
                        ProcessInstruction(name, rules, styles, variables, context);
                    }
                    else
                    {
                        // nested rules?
                        int brace = rules.IndexOf('{');
                        int lastSemiColon = rules.Length;
                        if (brace >= 0)
                        {
                            lastSemiColon = rules.LastIndexOf(';', brace, brace - 1);
                            if (lastSemiColon >= 0)
                            {
                                lastSemiColon++;
                            }
                        }

                        ThemeStyle style = ProcessStyle(name, rules.Substring(0, lastSemiColon), variables, context);
                        if (style != null)
                        {
                            style.Parent = parent != null ? parent.Target : null;
                            style.Variables = variables;
                            styles.Add(style);
                        }
                        if (brace >= 0)
                        {
                            ProcessRuleset(rules.Substring(lastSemiColon, rules.Length - lastSemiColon), context, styles, style);
                        }
                    }
                });
        }

		private static void ProcessInstruction(string name, string content,
            ThemeList styles, StringDictionary variables, ContextDictionary context)
		{
            switch (name)
            {
                case "@vals":
                    ParseVariables(variables, content);
                    break;

                case "@include":
                    break;

                case "@modules":
                    ParseContext(content, context);
                    break;
            }
		}

        private static ThemeParserContext ParseContext(string content, ContextDictionary context)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new InvalidOperationException("@modules section cannot be empty");

            ThemeParserContext result = null;

            content.ParseKeyValuePair(
                (key, value) =>
                {
                    try
                    {
                        context.Add(key, new ThemeParserContext(Assembly.Load(value)));
                    }
                    catch
                    {
                    }
                    //if (string.Compare(key, "Assembly", true) == 0)
                    //{
                    //    result = new ThemeParserContext(Assembly.Load(value));
                    //}
                });
            return result;
        }

		private static void ParseVariables(StringDictionary variables, string content)
		{
            if (string.IsNullOrWhiteSpace(content))
                return;
            content.ParseKeyValuePair((t1, t2) => variables.Add(t1, t2));
		}

        private static ThemeStyle ProcessStyle(string name, string content, StringDictionary variables, ContextDictionary context)
		{
			if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(content))
				return null;

            StringDictionary values = new StringDictionary();
            content.ParseKeyValuePair((t1, t2) => values.Add(t1, t2));

            string module = null;
            ThemeParserContext styleContext = ThemeParserContext.Default;

            if (values.TryGetValue("$module", out module))
            {
                context.TryGetValue(module, out styleContext);
                values.Remove("$module");
            }

            ThemeStyle style = new ThemeStyle(styleContext, name);
            style.UpdateProperties(values);

			return style;
		}
	}
}
