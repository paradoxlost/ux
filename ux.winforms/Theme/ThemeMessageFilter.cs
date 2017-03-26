using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Paradoxlost.UX.WinForms.Api;

namespace Paradoxlost.UX.WinForms.Theme
{
    class ThemeMessageFilter : IDisposable //: IMessageFilter
    {
        public string Name { get; set; }
        public bool IsResource { get; set; }

        protected ThemeStyle[] Styles { get; set; }

        protected IntPtr Hook { get; set; }

        private HookProc hookProc;

        public ThemeMessageFilter(string name, bool resource)
        {
            Name = name;
            IsResource = resource;

            ParseTheme();

            hookProc = new HookProc(MessageHook);

            Hook = UnsafeNativeMethods.SetWindowsHookEx(
                HookType.WH_CALLWNDPROCRET,
                hookProc,
                IntPtr.Zero,
                UnsafeNativeMethods.GetCurrentThreadId());
        }

        ~ThemeMessageFilter()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (Hook != IntPtr.Zero)
            {
                UnsafeNativeMethods.UnhookWindowsHookEx(Hook);
                Hook = IntPtr.Zero;
            }
        }

        private IntPtr MessageHook(int code, IntPtr wParam, IntPtr lParam)
        {
            CWPRETSTRUCT m = (CWPRETSTRUCT)Marshal.PtrToStructure(lParam, typeof(CWPRETSTRUCT));

            WindowMessage msg = (WindowMessage)m.message;

            switch (msg)
            {
                case WindowMessage.Create:
                    ApplyStyles(m.hWnd);
                    break;

            }

            return UnsafeNativeMethods.CallNextHookEx(Hook, code, wParam, lParam);
        }

        private void ParseTheme()
        {
            string themePath = Name;
            if (!IsResource)
            {
                if (!Path.IsPathRooted(themePath))
                {
                    themePath = Path.Combine(Application.StartupPath, themePath);
                }
                using (FileStream fs = File.OpenRead(themePath))
                {
                    Styles = ThemeParser.Parse(fs);
                }
            }
            else
            {
                Styles = ThemeParser.ParseResource(Name);
            }
        }

        private void ApplyStyles(IntPtr window)
        {
            Control control = Control.FromHandle(window);
            if (control != null)
            {
                foreach (ThemeStyle style in Styles)
                {
                    //System.Diagnostics.Debug.Print(style.TargetClass.Name);
                    if (style.TargetClass.IsAssignableFrom(control.GetType()))
                    {
                        style.Apply(control);
                    }
                }
            }
        }

        //public bool PreFilterMessage(ref Message m)
        //{
        //    WindowMessage msg = (WindowMessage)m.Msg;
        //    System.Diagnostics.Debug.Print("Message {0}, {1:X4}", msg, m.Msg);
        //    switch (msg)
        //    {
        //        case WindowMessage.Create:
        //            Control c = Control.FromHandle(m.HWnd);
        //            Form f = c as Form;
        //            if (f != null)
        //            {
        //                if (IsResource)
        //                {
        //                    f.ApplyThemeResource(Name);
        //                }
        //                else
        //                {
        //                    f.ApplyTheme(Name);
        //                }
        //            }
        //            break;

        //    }

        //    return false;
        //}
    }
}
