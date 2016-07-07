using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Paradoxlost.UX.WinForms.Theme
{
	class ThemeStyle
	{
		protected static Assembly FormsAssembly { get; private set; }
		protected static Module FormsModule { get; private set; }
		static ThemeStyle()
		{
			Type controlType = typeof(System.Windows.Forms.Control);
			FormsAssembly = controlType.Assembly;
			FormsModule = controlType.Module;
		}

		public Type TargetClass { get; protected set; }

		public ThemeStyle()
		{
		}

		public ThemeStyle(string className)
			: base()
		{
			// how to handle namespace resolution??
			// for now assume all types are in System.Windows.Forms
			Type[] targets = FormsModule.FindTypes((t, c) => t.Name == (string)c, className);

			if (targets.Length != 1)
			{
				throw new ArgumentException("Not a valid Control class", "className");
			}

			TargetClass = targets[0];
		}
	}
}
