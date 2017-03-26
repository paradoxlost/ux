using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Paradoxlost.UX.WinForms.Theme;
using Paradoxlost.UX.WinForms.Api;

namespace Paradoxlost.UX.test
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
            //this.Show();
            //ThemeManager.ApplyTheme(this);

            for (int i = 0; i < 1; i++)
                (new Form2()).Show();
		}
	}
}
