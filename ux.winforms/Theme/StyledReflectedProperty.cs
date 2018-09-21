using System;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace Paradoxlost.UX.WinForms.Theme
{
    public class StyledReflectedProperty
    {
        private readonly object instance;
        private readonly PropertyInfo propertyInfo;
        private readonly ISynchronizeInvoke synchronizeInvoke;

        public StyledReflectedProperty(object currentInstance, PropertyInfo propertyInfo, ISynchronizeInvoke synchronizeInvoke)
        {
            this.instance = currentInstance;
            this.propertyInfo = propertyInfo;
            this.synchronizeInvoke = synchronizeInvoke;
        }

        public void SetValue(params string[] args)
        {
            Type propertyType = this.propertyInfo.PropertyType;
            if (propertyType == typeof(Color))
            {
                SetColor(args[0]);
            }
            else if (propertyType.IsPrimitive)
            {
                var primitive = Convert.ChangeType(args[0], propertyType);
                this.SetValueCore(primitive);
            }
            else if (propertyType.IsEnum)
            {
                var enumValue = Enum.Parse(propertyType, args[0], true);
                this.SetValueCore(enumValue);
            }
            else
            {
                SetViaCtor(args);
            }
        }

        private void SetViaCtor(string[] args)
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
                    this.SetValueCore(value);
                    break;
                }
            }
        }

        private void SetColor(string color)
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

            this.SetValueCore(value);
            //pi.SetValue(control, value);
        }

        private void SetValueCore(object value)
        {
            InvokeIfRequired(()=>this.propertyInfo.SetValue(this.instance, value, null));
        }

        public void InvokeIfRequired(MethodInvoker action)
        {
            // See Update 2 for edits Mike de Klerk suggests to insert here.

            if (this.synchronizeInvoke.InvokeRequired)
            {
                this.synchronizeInvoke.Invoke(action,null);
            }
            else
            {
                action();
            }
        }
    }
}