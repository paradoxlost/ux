using System;
using System.Drawing;
using System.Reflection;

namespace Paradoxlost.UX.WinForms.Theme
{
    public class StyledReflectedProperty
    {
        private readonly object instance;
        private readonly PropertyInfo propertyInfo;

        public StyledReflectedProperty(object currentInstance, PropertyInfo propertyInfo)
        {
            this.instance = currentInstance;
            this.propertyInfo = propertyInfo;
        }

        public void SetValue(params string[] args)
        {
            Type propertyType = this.propertyInfo.PropertyType;
            if (propertyType == typeof(Color))
            {
                SetColor(this.instance, args[0]);
            }
            else if (propertyType.IsPrimitive)
            {
                var value = Convert.ChangeType(args[0], propertyType);
                this.propertyInfo.SetValue(this.instance, value, null);
            }
            else if (propertyType.IsEnum)
            {
                this.propertyInfo.SetValue(this.instance, Enum.Parse(propertyType, args[0], true), null);
            }
            else
            {
                SetViaCtor(this.instance, args);
            }
        }

        private void SetViaCtor(object control, string[] args)
        {
            // find a property ctor with the same number of args
            foreach (ConstructorInfo ci in this.propertyInfo.PropertyType.GetConstructors())
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
                    this.propertyInfo.SetValue(control, value, null);
                    break;
                }
            }
        }

        private void SetColor(object control, string color)
        {
            // color is stupid
            // it doesn't use ctors. it has a bunch of static methods
            object value;
            // we'll only handle names & html
            if (color[0] == '#')
            {
                value = ColorTranslator.FromHtml(color);
            }
            else
            {
                value = Color.FromKnownColor(
                    (KnownColor)Enum.Parse(typeof(KnownColor), color, true));
            }
            this.propertyInfo.SetValue(control, value, null);
            //pi.SetValue(control, value);
        }
    }
}