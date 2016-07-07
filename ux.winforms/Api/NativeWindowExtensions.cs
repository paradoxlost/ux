using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Paradoxlost.UX.WinForms.Api
{
	public class NativeWindowMenu
	{
		public IntPtr Handle { get; set; }

		internal NativeWindowMenu(IntPtr handle)
		{
			Handle = handle;
		}

		public int GetCount()
		{
			return UnsafeNativeMethods.GetMenuItemCount(Handle);
		}
	}

	public static class NativeWindowExtensions
	{
		public static void Close(this NativeWindow window)
		{
			window.SendMessage(WindowMessage.Close, 0, 0);
		}

		public static bool FindWindow(this NativeWindow window, string className, string caption)
		{
			bool result = false;
			IntPtr handle = UnsafeNativeMethods.FindWindow(className, caption);

			if (handle != IntPtr.Zero)
			{
				result = true;
				window.AssignHandle(handle);
			}

			return result;
		}

		public static NativeWindow FindWindowEx(this NativeWindow window, NativeWindow childAfter, string className, string caption)
		{
			IntPtr handle = UnsafeNativeMethods.FindWindowEx(window.Handle, childAfter == null ? IntPtr.Zero : childAfter.Handle, className, caption);

			if (handle != IntPtr.Zero)
			{
				NativeWindow result = new NativeWindow();
				result.AssignHandle(handle);
				return result;
			}
			return null;
		}

		public static NativeWindowMenu GetMenu(this NativeWindow window)
		{
			IntPtr menu = UnsafeNativeMethods.GetMenu(window.Handle);

			if (menu != IntPtr.Zero)
				return new NativeWindowMenu(menu);
			return null;
		}

		public static RECT GetClientRect(this NativeWindow window)
		{
			RECT rect;
			UnsafeNativeMethods.GetClientRect(window.Handle, out rect);
			return rect;
		}

		public static WindowStyles GetStyles(this NativeWindow window)
		{
			return (WindowStyles)UnsafeNativeMethods.GetWindowLong(window.Handle, (int)WindowLongFlags.Style);
		}

		public static void SetStyles(this NativeWindow window, WindowStyles styles)
		{
			WINDOWPLACEMENT placement = window.GetPlacement();
			UnsafeNativeMethods.SetWindowLong(window.Handle, (int)WindowLongFlags.Style, (int)styles);
			window.SetPlacement(placement);
		}

		public static WINDOWPLACEMENT GetPlacement(this NativeWindow window)
		{
			WINDOWPLACEMENT result = WINDOWPLACEMENT.Default;
			UnsafeNativeMethods.GetWindowPlacement(window.Handle, ref result);
			return result;
		}

		public static void SetPlacement(this NativeWindow window, WINDOWPLACEMENT placement)
		{
			UnsafeNativeMethods.SetWindowPlacement(window.Handle, ref placement);
		}

		public static int SendMessage(this NativeWindow window, WindowMessage msg, uint wparam, uint lparam)
		{
			return UnsafeNativeMethods.SendMessage(window.Handle, msg, wparam, lparam);
		}

		public static bool PostMessage(this NativeWindow window, WindowMessage msg, uint wparam, uint lparam)
		{
			return UnsafeNativeMethods.PostMessage(window.Handle, msg, wparam, lparam);
		}

		public static bool SetWindowText(this NativeWindow window, string text)
		{
			return UnsafeNativeMethods.SetWindowText(window.Handle, text);
		}

		public static bool SendKey(this NativeWindow window, VirtualKey key)
		{
			bool result = SendKeyDown(window, key);
			result = result & SendKeyUp(window, key);
			return result;
		}

		public static bool SendKeyDown(this NativeWindow window, VirtualKey key)
		{
			uint lparam = 1;		// repeat count
			lparam |= GetScanCode(key) << 16;

			return UnsafeNativeMethods.PostMessage(window.Handle, WindowMessage.KeyDown, (uint)key, lparam);
		}

		public static bool SendKeyUp(this NativeWindow window, VirtualKey key)
		{
			uint lparam = 1;		// repeat count
			lparam |= 0x80000000;	// transition state
			lparam |= 0x40000000;	// previous key state
			lparam |= GetScanCode(key) << 16;

			return UnsafeNativeMethods.PostMessage(window.Handle, WindowMessage.KeyUp, (uint)key, lparam);
		}

		//http://www.microsoft.com/whdc/device/input/Scancode.mspx
		//http://web.archive.org/web/20080409063910/http://www.microsoft.com/whdc/device/input/Scancode.mspx
		private static uint GetScanCode(VirtualKey key)
		{
			switch (key)
			{
				case VirtualKey.SPACE: return 0x39;
				case VirtualKey.PAUSE: return 0x45;
				case VirtualKey.SCROLL: return 0x46;
				case VirtualKey.LEFT: return 0x14B;
				case VirtualKey.RIGHT: return 0x14D;
				case VirtualKey.UP: return 0x148;
				case VirtualKey.DOWN: return 0x150;
				case VirtualKey.END: return 0x14F;
				case VirtualKey.LSHIFT: return 0x2A;
				case VirtualKey.DELETE: return 0x153;
				case VirtualKey.PGDN: return 0x151;
				case VirtualKey.CONTROL: return 0x1D;
				//case VirtualKey.Z: return 0x2C;
				//case VirtualKey.C: return 0x2E;

				case VirtualKey.A: return 0x1E;
				case VirtualKey.B: return 0x30;
				case VirtualKey.C: return 0x2E;
				case VirtualKey.D: return 0x20;
				case VirtualKey.E: return 0x12;
				case VirtualKey.F: return 0x21;
				case VirtualKey.G: return 0x22;
				case VirtualKey.H: return 0x23;
				case VirtualKey.I: return 0x17;
				case VirtualKey.J: return 0x24;
				case VirtualKey.K: return 0x25;
				case VirtualKey.L: return 0x26;
				case VirtualKey.M: return 0x32;
				case VirtualKey.N: return 0x31;
				case VirtualKey.O: return 0x18;
				case VirtualKey.P: return 0x19;
				case VirtualKey.Q: return 0x10;
				case VirtualKey.R: return 0x13;
				case VirtualKey.S: return 0x1F;
				case VirtualKey.T: return 0x14;
				case VirtualKey.U: return 0x16;
				case VirtualKey.V: return 0x2F;
				case VirtualKey.W: return 0x11;
				case VirtualKey.X: return 0x2D;
				case VirtualKey.Y: return 0x15;
				case VirtualKey.Z: return 0x2C;
				case VirtualKey.FSLASH: return 0x35;
			}

			return 0;
		}

		public static string[] GetListBoxItems(this NativeWindow window)
		{
			int itemCount = window.SendMessage(WindowMessage.ListBoxGetCount, 0, 0);
			List<string> items = new List<string>(itemCount);

			for (int i = 0; i < itemCount; i++)
			{
				StringBuilder buffer;
				int length = UnsafeNativeMethods.SendMessage(window.Handle, WindowMessage.ListBoxGetTextLength, i, 0);

				buffer = new StringBuilder(length + 1);
				UnsafeNativeMethods.SendMessage(window.Handle, WindowMessage.ListBoxGetText, i, buffer);

				items.Add(buffer.ToString());
			}

			return items.ToArray();
		}

	}
}
