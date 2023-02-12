using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Maxter_Management.Forms
{
    public partial class AddressFrm : Form
    {
        string addressFull;

        public string AddressFull { get => addressFull; set => addressFull = value; }
        public AddressFrm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool filled = true;
            foreach (Control item in this.Controls)
            {

                if(item is TextBox)
                {
                    if (item.Text.Trim() == "")
                    {
                        filled = false;
                    }
                }
            }

            

            if (filled)
            {
                addressFull = textBox1.Text + ";" + textBox2.Text + ';' + textBox3.Text + ";" + textBox4.Text + ';' + textBox5.Text + ';' + textBox6.Text;
            }
            else
            {
                MessageBox.Show("Every field must be filled!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = DialogResult.None;
            }
        }
    }
}
