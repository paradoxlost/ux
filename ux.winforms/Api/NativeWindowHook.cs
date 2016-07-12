﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Paradoxlost.UX.WinForms.Api
{
	using ActionDictionary = MultiValueDictionary<WindowMessage, MessageAction>;

	internal class NativeWindowHook : NativeWindow
	{
		protected Control Hooked { get; private set; }
		protected ActionDictionary MessageActions { get; private set; }

		public NativeWindowHook(Control control)
			: base()
		{
			MessageActions = new ActionDictionary();
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

		public void HandleMessage(WindowMessage msg, MessageAction action)
		{
			this.MessageActions.Add(msg, action);
		}

		private void OnHookedHandleCreated(object sender, EventArgs e)
		{
			AssignHandle(Hooked.Handle);
			Hooked.HandleCreated -= OnHookedHandleCreated;
		}

		protected override void WndProc(ref Message m)
		{
			WindowMessage msg = (WindowMessage)m.Msg;

			bool messageHandled = false;
			IReadOnlyCollection<MessageAction> actions;
			if (this.MessageActions.TryGetValue(msg, out actions))
			{
				foreach (MessageAction handler in actions)
				{
					try
					{
						messageHandled |= handler(this, ref m);
					}
					catch
					{
					}
				}
			}

			switch (msg)
			{
				case WindowMessage.Close:
				case WindowMessage.Destroy:
					this.MessageActions.Clear();
					this.ReleaseHandle();
					Hooked = null;
					break;

				default:
					if (!messageHandled)
					{
						base.WndProc(ref m);
					}
					break;
			}
		}
	}
}
