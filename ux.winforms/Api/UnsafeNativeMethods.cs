using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Paradoxlost.UX.WinForms.Api
{
	internal static class UnsafeNativeMethods
	{
		[DllImport("user32.dll")]
		public static extern bool PostMessage(IntPtr hwnd, WindowMessage msg, uint wparam, uint lparam);

		[DllImport("user32.dll")]
		public static extern int SendMessage(IntPtr hwnd, WindowMessage msg, uint wparam, uint lparam);
		[DllImport("user32.dll")]
		public static extern int SendMessage(IntPtr hwnd, WindowMessage msg, int wparam, int lparam);
		[DllImport("user32.dll", CharSet = CharSet.Ansi)]
		public static extern int SendMessage(IntPtr hwnd, WindowMessage msg, int wparam, StringBuilder lparam);

		[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		public static extern bool SetWindowText(IntPtr hwnd, String lpString);

		/// <summary>
		/// Retrieves the show state and the restored, minimized, and maximized positions of the specified window.
		/// </summary>
		/// <param name="hWnd">
		/// A handle to the window.
		/// </param>
		/// <param name="lpwndpl">
		/// A pointer to the WINDOWPLACEMENT structure that receives the show state and position information.
		/// <para>
		/// Before calling GetWindowPlacement, set the length member to sizeof(WINDOWPLACEMENT). GetWindowPlacement fails if lpwndpl-> length is not set correctly.
		/// </para>
		/// </param>
		/// <returns>
		/// If the function succeeds, the return value is nonzero.
		/// <para>
		/// If the function fails, the return value is zero. To get extended error information, call GetLastError.
		/// </para>
		/// </returns>
		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

		/// <summary>
		/// Sets the show state and the restored, minimized, and maximized positions of the specified window.
		/// </summary>
		/// <param name="hWnd">
		/// A handle to the window.
		/// </param>
		/// <param name="lpwndpl">
		/// A pointer to a WINDOWPLACEMENT structure that specifies the new show state and window positions.
		/// <para>
		/// Before calling SetWindowPlacement, set the length member of the WINDOWPLACEMENT structure to sizeof(WINDOWPLACEMENT). SetWindowPlacement fails if the length member is not set correctly.
		/// </para>
		/// </param>
		/// <returns>
		/// If the function succeeds, the return value is nonzero.
		/// <para>
		/// If the function fails, the return value is zero. To get extended error information, call GetLastError.
		/// </para>
		/// </returns>
		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetWindowPlacement(IntPtr hWnd, [In] ref WINDOWPLACEMENT lpwndpl);

		/// <summary>
		/// Changes an attribute of the specified window. The function also sets the 32-bit (long) value at the specified offset into the extra window memory.
		/// </summary>
		/// <param name="hWnd">A handle to the window and, indirectly, the class to which the window belongs..</param>
		/// <param name="nIndex">The zero-based offset to the value to be set. Valid values are in the range zero through the number of bytes of extra window memory, minus the size of an integer. To set any other value, specify one of the following values: GWL_EXSTYLE, GWL_HINSTANCE, GWL_ID, GWL_STYLE, GWL_USERDATA, GWL_WNDPROC </param>
		/// <param name="dwNewLong">The replacement value.</param>
		/// <returns>If the function succeeds, the return value is the previous value of the specified 32-bit integer.
		/// If the function fails, the return value is zero. To get extended error information, call GetLastError. </returns>
		[DllImport("user32.dll")]
		public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

		[DllImport("user32.dll")]
		public static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

		// For Windows Mobile, replace user32.dll with coredll.dll
		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr FindWindowEx(IntPtr hWndParent, IntPtr hWndChildAfter, string lpszClass, string lpszWindow);

		[DllImport("user32.dll")]
		public static extern IntPtr GetMenu(IntPtr hwnd);

		[DllImport("user32.dll")]
		public static extern int GetMenuString(IntPtr hMenu, UInt32 id, string lpString, int ccMax, UInt32 flags);

		[DllImport("user32.dll")]
		public static extern int GetMenuItemCount(IntPtr hMenu);
	}
}
