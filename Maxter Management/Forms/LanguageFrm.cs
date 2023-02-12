using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Maxter_Management.Forms
{
    public partial class LanguageFrm : Form
    {
        private string current;

        public LanguageFrm()
        {
            InitializeComponent();
            comboBox1.Items.Add("English");
            comboBox1.Items.Add("Magyar");

            if (File.Exists("language.txt"))
            {
                current = File.ReadAllText("language.txt");

                switch (current)
                {
                    case "hu":
                        comboBox1.SelectedIndex = 1;
                        break;
                    default:
                        comboBox1.SelectedIndex = 0;
                        break;
                }
            }
            else
            {
                File.WriteAllText("language.txt", "en");
                comboBox1.SelectedIndex = 0;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0)
            {
                switch (current)
                {
                    case "en":
                        break;
                    case "hu":
                        File.WriteAllText("language.txt", "en");
                        break;
                }
            }
            else if (comboBox1.SelectedIndex == 1)
            {
                switch (current)
                {
                    case "en":
                        File.WriteAllText("language.txt", "hu");
                        break;
                    case "hu":
                        break;
                }
            }

            MessageBox.Show("The program needs to be restarted for changes to take effect!", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
