using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Paradoxlost.UX.WinForms.Theme
{
	public static class ThemeManager
	{
		public static void ApplyTheme(this Form window, string themeName = null)
		{
			ThemeStyle[] styles = ThemeParser.ParseResource("Paradoxlost.UX.WinForms.Theme.Sample.theme");

			foreach (ThemeStyle style in styles)
			{
				System.Diagnostics.Debug.Print(style.TargetClass.Name);
				if (style.TargetClass.IsAssignableFrom(window.GetType()))
				{
					style.Apply(window);
				}
			}
		}
	}
}
