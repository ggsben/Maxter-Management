using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Maxter_Management.Forms
{
    public partial class InvoiceSettingsFrm : Form
    {
        public InvoiceSettingsFrm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            /*if (File.Exists("key.zip"))
            {
                string zipPath = "key.zip";

                using (ZipArchive archive = ZipFile.Open(zipPath, ZipArchiveMode.Update))
                {

                    var entry = archive.GetEntry("key.txt");

                    entry.Delete();

                    entry = archive.CreateEntry("key.txt");

                    using (StreamWriter writer = new StreamWriter(entry.Open()))
                    {
                        writer.WriteLine(textBox1.Text);
                        writer.WriteLine(textBox2.Text);
                    }
                }
            }*/

            using (StreamWriter writer = new StreamWriter(File.Open("key.txt", FileMode.Create)))
            {
                writer.WriteLine(textBox1.Text.Trim());
                writer.WriteLine(textBox2.Text.Trim());
            }
        }
    }
}
