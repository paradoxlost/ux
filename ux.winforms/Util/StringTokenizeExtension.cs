using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paradoxlost.UX.WinForms.Util
{
	public static class StringTokenizeExtension
	{
		public static void Tokenize(this string content, char d1, char d2,
			Action<string, string> action)
		{
			Tokenize(content,
				d1, d2,
				d1p => d1p < 0,
				d2p => d2p < 0,
				action);
		}

		public static void Tokenize(this string content, char d1, char d2,
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
	}
}
