using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Paradoxlost.UX.WinForms.Api
{
	public delegate bool MessageAction(NativeWindow hwnd, ref Message m);
    public delegate IntPtr HookProc(int code, IntPtr wParam, IntPtr lParam);
}
