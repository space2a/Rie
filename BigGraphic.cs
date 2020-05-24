using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Rie
{
    public partial class BigGraphic : Form
    {
        public Form1 form1;
        public BigGraphic()
        {
            InitializeComponent();
        }

        private void panel1_MouseClick(object sender, MouseEventArgs e)
        {
            form1.Start(panel1, 0.004F, 69000, 2000, 222000);
        }

        private void BigGraphic_FormClosing(object sender, FormClosingEventArgs e)
        {
            form1.bworking = true;
        }
    }
}