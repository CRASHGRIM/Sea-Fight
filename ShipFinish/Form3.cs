using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShipsWar
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            this.FormClosing += (sender, args) => Application.Exit();
            this.Size = new Size(1300, 600);
            this.BackgroundImage = Image.FromFile(@"..\..\fail.png");
            this.BackgroundImageLayout = ImageLayout.None;
            InitializeComponent();
        }
    }
}
