using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace Paradoxlost.UX.WinForms.Theme
{
    using StringDictionary = Dictionary<string, string>;
    using StringKeyValue = KeyValuePair<string, string>;

    class ThemeStyle
    {
        //protected static Assembly FormsAssembly { get; private set; }
        //protected static Module FormsModule { get; private set; }
        //static ThemeStyle()
        //{
        //    // for now assume all types are in System.Windows.Forms
        //    Type controlType = typeof(Control);
        //    FormsAssembly = controlType.Assembly;
        //    FormsModule = controlType.Module;
        //}
        public ThemeParserContext Context { get; internal set; }
        public Type Target { get; protected set; }
        public Type Parent { get; set; }
        internal StringDictionary Variables { get; set; }
        protected StringDictionary Properties { get; set; }

        public ThemeStyle(ThemeParserContext context)
        {
            Context = context;
            Properties = new StringDictionary();
        }

        public ThemeStyle(ThemeParserContext context, string className)
            : this(context)
        {
            Target = Context.GetTarget(className);
        }

        internal void UpdateProperties(StringDictionary properties)
        {
            Properties = properties;
        }

        public void AddProperty(string name, string value)
        {
            Properties.Add(name, value);
        }

        public bool CanApplyTo(Control control)
        {
            bool result = false;

            if (Target.IsAssignableFrom(control.GetType()))
            {
                if (Parent != null)
                {
                    Control current = control;
                    while (current != null)
                    {
                        current = current.Parent;
                        if (current != null && Parent.IsAssignableFrom(current.GetType()))
                        {
                            result = true;
                            break;
                        }
                    }
                }
                else
                {
                    result = true;
                }
            }

            return result;
        }

        public void Apply(Control control)
        {
            foreach (StringKeyValue pair in this.Properties)
            {
                StyledReflectedProperty property = this.CreateStyledReflectedProperty(control, pair.Key);

                // check for variables
                string pairValue = pair.Value;
                if (pairValue[0] == '$')
                    pairValue = this.Variables[pairValue.Substring(1)];

                string[] args = pairValue.Split(new string[] { ", " }, StringSplitOptions.None);
                property.SetValue(args);
            }
        }

        private StyledReflectedProperty CreateStyledReflectedProperty(Control control, string key)
        {
            var props = key.Split('.');
            PropertyInfo pi = null;
            var currentTarget = this.Target;
            object currentInstance = control;
            object previousInstance = control;
            foreach (var p in props)
            {
                previousInstance = currentInstance;
                pi = currentTarget.GetProperty(p, BindingFlags.Instance | BindingFlags.Public);
                if (pi == null) break;
                currentInstance = pi.GetValue(previousInstance, null);
                currentTarget = pi.PropertyType;
            }
            currentInstance = previousInstance;
            if (pi == null) return null;

            return new StyledReflectedProperty(currentInstance, pi, control);
        }
    }
}
