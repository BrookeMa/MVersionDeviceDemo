using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace MVersionDeviceDemo
{
    public partial class ManagerForm : Form
    {
        private CamereaViewModel viewModel;
        public ManagerForm()
        {
            InitializeComponent();
            viewModel = new CamereaViewModel();
            viewModel.Init(null);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            viewModel.Open();
        }

        private void button2_Click(object sender, EventArgs e)
        { 
            viewModel.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            viewModel.Save("U3v", "D:\\1.bmp");
        }

        private void Dectect_Click(object sender, EventArgs e)
        {
            this.viewModel.detectDevice();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            object img = null;
            this.viewModel.Grab(out img, "U3v", 1);


        }

        private void button5_Click(object sender, EventArgs e)
        {
            viewModel.ShowVideo(0);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            viewModel.CloseVideo(0);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            viewModel.ShowVideo(1);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            viewModel.CloseVideo(1);
        }
    }
}
