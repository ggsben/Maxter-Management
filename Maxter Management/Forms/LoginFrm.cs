using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Maxter_Management.Models;
using Maxter_Management.FunctionsAndMethods;
using Maxter_Management.Forms;

namespace Maxter_Management
{
    public partial class LoginFrm : Form
    {
        public string[] userAccess;

        public LoginFrm()
        {
            InitializeComponent();
            //DBFunctions.ConnectionTest();
            this.AcceptButton = button1;
            Font = new Font(new FontFamily("Segoe UI"), Font.Size);

            //textBox1.Text = ApiFunctions.RunAsync();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.ToString().Trim() != "" && textBox2.Text.ToString().Trim() != "")
            {
                userAccess = MenuFunctions.Login(textBox1.Text.ToString().Trim(), textBox2.Text.ToString().Trim());

                if (userAccess != null)
                {
                    DialogResult = DialogResult.OK;
                    /*MenuFrm menu = new MenuFrm();
                    this.Hide();
                    menu.ShowDialog();
                    this.Close();*/
                } 
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void LoginFrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            if (DialogResult != DialogResult.OK)
            {
                DialogResult = DialogResult.Abort;
            }
            e.Cancel = false;
        }
    }
}
