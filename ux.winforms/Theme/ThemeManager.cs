using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Paradoxlost.UX.WinForms.Theme
{
	public static class ThemeManager
	{
        private static ThemeMessageFilter messageFilter = null;

        public static void ApplyGlobalTheme(string themeName = "default.theme")
        {
            if (messageFilter == null)
            {
                messageFilter = new ThemeMessageFilter(themeName, false);
                //Application.AddMessageFilter(messageFilter);
            }
        }

        public static void ApplyGlobalThemeResource(string themeName = null)
        {
            if (messageFilter == null)
            {
                messageFilter = new ThemeMessageFilter(themeName, true);
                //Application.AddMessageFilter(messageFilter);
            }
        }

        public static void DisableGlobalTheme()
        {
            if (messageFilter != null)
            {
                messageFilter.Dispose();
            }
        }

        public static void ApplyTheme(this Form window, string themeName = null)
		{
            if (string.IsNullOrEmpty(themeName))
                themeName = "default.theme";

            if (!Path.IsPathRooted(themeName))
            {
                themeName = Path.Combine(Application.StartupPath, themeName);
            }
            using (FileStream fs = File.OpenRead(themeName))
            {
                ThemeStyle[] styles = ThemeParser.Parse(fs);

                foreach (ThemeStyle style in styles)
                {
                    //System.Diagnostics.Debug.Print(style.TargetClass.Name);
                    if (style.TargetClass.IsAssignableFrom(window.GetType()))
                    {
                        style.Apply(window);
                    }
                }
            }
		}

        public static void ApplyThemeResource(this Form window, string themeName = null)
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
