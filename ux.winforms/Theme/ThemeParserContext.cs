using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Paradoxlost.UX.WinForms.Theme
{
    class ThemeParserContext
    {
        public static ThemeParserContext Default = new ThemeParserContext(Assembly.GetAssembly(typeof(System.Windows.Forms.Control)));

        public Assembly Assembly { get; set; }

        public ThemeParserContext()
        {
        }

        public ThemeParserContext(Assembly assembly)
        {
            Assembly = assembly;
        }

        public Type GetTarget(string className)
        {
            // how to handle namespace resolution??
            // for now assume all types are in System.Windows.Forms
            List<Type> targets = new List<Type>();
            foreach (Module mod in Assembly.Modules)
            {
                Type[] modTargets = mod.FindTypes(
                    (t, c) =>
                    {
                        bool result = false;
                        if (t.IsClass)
                        {
                            result = ((t.IsNested) ? t.FullName.Substring(t.Namespace.Length + 1) : t.Name) == (string)c;
                        }
                        return result;
                    }, className);

                targets.AddRange(modTargets);
            }

            if (targets.Count != 1)
            {
                throw new ArgumentException("Not a valid Control class", "className");
            }

            return targets[0];
        }
    }
}
