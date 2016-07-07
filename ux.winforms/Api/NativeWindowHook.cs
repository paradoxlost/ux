using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Paradoxlost.UX.WinForms.Api
{
	internal class NativeWindowHook : NativeWindow
	{
		protected Control Hooked { get; private set; }

		public NativeWindowHook(Control control)
			: base()
		{
			Hooked = control;
			if (!control.IsHandleCreated)
			{
				control.HandleCreated += OnHookedHandleCreated;
			}
			else
			{
				AssignHandle(control.Handle);
			}
		}

		private void OnHookedHandleCreated(object sender, EventArgs e)
		{
			AssignHandle(Hooked.Handle);
			Hooked.HandleCreated -= OnHookedHandleCreated;
		}

		protected override void WndProc(ref Message m)
		{
			switch ((WindowMessage)m.Msg)
			{
				case WindowMessage.Close:
				case WindowMessage.Destroy:
					this.ReleaseHandle();
					Hooked = null;
					break;

				default:
					base.WndProc(ref m);
					break;
			}
		}
	}
}
