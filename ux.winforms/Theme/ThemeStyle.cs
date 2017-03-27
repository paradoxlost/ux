using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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
        public ThemeParserContext Context { get; protected set; }
        public Type Target { get; protected set; }
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

        public void AddProperty(string name, string value)
        {
            Properties.Add(name, value);
        }

        public void Apply(Control control)
        {
            foreach (StringKeyValue pair in Properties)
            {
                PropertyInfo pi = Target.GetProperty(pair.Key, BindingFlags.Instance | BindingFlags.Public);
                if (pi == null)
                    continue;

                // check for variables
                string pairValue = pair.Value;
                if (pairValue[0] == '$')
                    pairValue = Variables[pairValue.Substring(1)];

                Type propertyType = pi.PropertyType;
                string[] args = pairValue.Split(new string[] { ", " }, StringSplitOptions.None);

                // color is stupid
                // it doesn't use ctors. it has a bunch of static methods
                if (propertyType == typeof(Color))
                {
                    object value;
                    // we'll only handle names & html
                    if (args[0][0] == '#')
                    {
                        value = ColorTranslator.FromHtml(args[0]);
                    }
                    else
                    {
                        value = Color.FromKnownColor(
                            (KnownColor)Enum.Parse(typeof(KnownColor), args[0], true));
                    }
                    pi.SetValue(control, value, null);
                    //pi.SetValue(control, value);

                    continue;
                }

                // find a property ctor with the same number of args
                foreach (ConstructorInfo ci in propertyType.GetConstructors())
                {
                    ParameterInfo[] ctorArgs = ci.GetParameters();
                    if (ctorArgs.Length != args.Length)
                        continue;

                    // see if we can use this one
                    object[] values = new object[args.Length];
                    bool good = true;
                    for (int i = 0; i < ctorArgs.Length; i++)
                    {
                        try
                        {
                            Type argType = ctorArgs[i].ParameterType;
                            if (argType.IsEnum)
                            {
                                values[i] = Enum.Parse(argType, args[i], true);
                            }
                            else
                            {
                                values[i] = Convert.ChangeType(args[i], ctorArgs[i].ParameterType);
                            }
                        }
                        catch (Exception ex)
                        {
                            if (ex is ArgumentException || ex is ArgumentNullException || ex is InvalidCastException)
                            {
                                good = false;
                                break;
                            }
                            throw;
                        }
                    }

                    if (good)
                    {
                        object value = ci.Invoke(values);
                        pi.SetValue(control, value, null);
                        break;
                    }
                }
            }
        }
    }
}
