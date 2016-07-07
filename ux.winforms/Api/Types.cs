using System;
using System.Runtime.InteropServices;

namespace Paradoxlost.UX.WinForms.Api
{
	[StructLayout(LayoutKind.Sequential)]
	public struct POINT
	{
		public int X;
		public int Y;

		public POINT(int x, int y)
		{
			this.X = x;
			this.Y = y;
		}

		public POINT(System.Drawing.Point pt) : this(pt.X, pt.Y) { }

		public static implicit operator System.Drawing.Point(POINT p)
		{
			return new System.Drawing.Point(p.X, p.Y);
		}

		public static implicit operator POINT(System.Drawing.Point p)
		{
			return new POINT(p.X, p.Y);
		}

		public bool Equals(POINT pt)
		{
			return this.X == pt.X && this.Y == pt.Y;
		}

		public override bool Equals(object obj)
		{
			if (obj is POINT)
				return Equals((POINT)obj);
			else if (obj is System.Drawing.Point)
				return Equals(new POINT((System.Drawing.Point)obj));
			return false;
		}

		public static bool operator ==(POINT p1, POINT p2)
		{
			return p1.Equals(p2);
		}

		public static bool operator !=(POINT p1, POINT p2)
		{
			return !p1.Equals(p2);
		}

		public override int GetHashCode()
		{
			return this.X.GetHashCode() + this.Y.GetHashCode();
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct RECT
	{
		public int Left;
		public int Top;
		public int Right;
		public int Bottom;

		public RECT(int left, int top, int right, int bottom)
		{
			Left = left;
			Top = top;
			Right = right;
			Bottom = bottom;
		}

		public RECT(System.Drawing.Rectangle r) : this(r.Left, r.Top, r.Right, r.Bottom) { }

		public int X
		{
			get { return Left; }
			set { Right -= (Left - value); Left = value; }
		}

		public int Y
		{
			get { return Top; }
			set { Bottom -= (Top - value); Top = value; }
		}

		public int Height
		{
			get { return Bottom - Top; }
			set { Bottom = value + Top; }
		}

		public int Width
		{
			get { return Right - Left; }
			set { Right = value + Left; }
		}

		public System.Drawing.Point Location
		{
			get { return new System.Drawing.Point(Left, Top); }
			set { X = value.X; Y = value.Y; }
		}

		public System.Drawing.Size Size
		{
			get { return new System.Drawing.Size(Width, Height); }
			set { Width = value.Width; Height = value.Height; }
		}

		public static implicit operator System.Drawing.Rectangle(RECT r)
		{
			return new System.Drawing.Rectangle(r.Left, r.Top, r.Width, r.Height);
		}

		public static implicit operator RECT(System.Drawing.Rectangle r)
		{
			return new RECT(r);
		}

		public static bool operator ==(RECT r1, RECT r2)
		{
			return r1.Equals(r2);
		}

		public static bool operator !=(RECT r1, RECT r2)
		{
			return !r1.Equals(r2);
		}

		public bool Equals(RECT r)
		{
			return r.Left == Left && r.Top == Top && r.Right == Right && r.Bottom == Bottom;
		}

		public override bool Equals(object obj)
		{
			if (obj is RECT)
				return Equals((RECT)obj);
			else if (obj is System.Drawing.Rectangle)
				return Equals(new RECT((System.Drawing.Rectangle)obj));
			return false;
		}

		public override int GetHashCode()
		{
			return ((System.Drawing.Rectangle)this).GetHashCode();
		}

		public override string ToString()
		{
			return string.Format(System.Globalization.CultureInfo.CurrentCulture, "{{Left={0},Top={1},Right={2},Bottom={3}}}", Left, Top, Right, Bottom);
		}
	}

	/// <summary>
	/// Contains information about the placement of a window on the screen.
	/// </summary>
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct WINDOWPLACEMENT
	{
		/// <summary>
		/// The length of the structure, in bytes. Before calling the GetWindowPlacement or SetWindowPlacement functions, set this member to sizeof(WINDOWPLACEMENT).
		/// <para>
		/// GetWindowPlacement and SetWindowPlacement fail if this member is not set correctly.
		/// </para>
		/// </summary>
		public int Length;

		/// <summary>
		/// Specifies flags that control the position of the minimized window and the method by which the window is restored.
		/// </summary>
		public int Flags;

		/// <summary>
		/// The current show state of the window.
		/// </summary>
		public ShowWindowCommands ShowCmd;

		/// <summary>
		/// The coordinates of the window's upper-left corner when the window is minimized.
		/// </summary>
		public POINT MinPosition;

		/// <summary>
		/// The coordinates of the window's upper-left corner when the window is maximized.
		/// </summary>
		public POINT MaxPosition;

		/// <summary>
		/// The window's coordinates when the window is in the restored position.
		/// </summary>
		public RECT NormalPosition;

		/// <summary>
		/// Gets the default (empty) value.
		/// </summary>
		public static WINDOWPLACEMENT Default
		{
			get
			{
				WINDOWPLACEMENT result = new WINDOWPLACEMENT();
				result.Length = Marshal.SizeOf(result);
				return result;
			}
		}

		public bool Equals(WINDOWPLACEMENT wp)
		{
			return this.Length == wp.Length && this.Flags == wp.Flags &&
				this.ShowCmd == wp.ShowCmd && this.MinPosition == wp.MinPosition &&
				this.MaxPosition == wp.MaxPosition && this.NormalPosition == wp.NormalPosition;
		}

		public override bool Equals(object obj)
		{
			if (obj is WINDOWPLACEMENT)
				return Equals((WINDOWPLACEMENT)obj);
			return false;
		}

		public static bool operator ==(WINDOWPLACEMENT r1, WINDOWPLACEMENT r2)
		{
			return r1.Equals(r2);
		}

		public static bool operator !=(WINDOWPLACEMENT r1, WINDOWPLACEMENT r2)
		{
			return !r1.Equals(r2);
		}

		public override int GetHashCode()
		{
			return this.Flags.GetHashCode() + this.Length.GetHashCode() + this.MaxPosition.GetHashCode() +
				this.MinPosition.GetHashCode() + this.NormalPosition.GetHashCode() + this.ShowCmd.GetHashCode();
		}
	}
}