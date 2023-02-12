using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Maxter_Management.Models;

namespace Maxter_Management.Forms
{
    public partial class PartnerFrm : Form
    {
        private Partner old;

        public PartnerFrm()
        {
            InitializeComponent();
            Font = new Font(new FontFamily("Segoe UI"), Font.Size);
        }

        public PartnerFrm(string oldId) : this()
        {
            using (MaxterDBEntities context = new MaxterDBEntities())
            {
                old = context.Partners.Where(x => x.Partner_id.ToString() == oldId).FirstOrDefault();
            }

            Text = "Update Partner";

            textBox1.Text = old.Name;
            textBox2.Text = old.TIN;
            textBox2.ReadOnly = true;
            textBox3.Text = old.Invoice_Address;
            textBox4.Text = old.Shipping_Address;
            textBox5.Text = old.Email;
            textBox6.Text = old.Telephone;
            numericUpDown1.Value = (decimal)old.Price_Category;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                bool filled = true;
                foreach (Control item in this.Controls)
                {
                    if (item is TextBox)
                    {
                        if (item.Text.Trim().Length == 0)
                        {
                            filled = false;
                        }
                    }
                }

                if (old == null && filled)
                {
                    Partner current = new Partner(textBox2.Text, textBox1.Text, textBox3.Text, textBox4.Text, (byte)numericUpDown1.Value, textBox5.Text, textBox6.Text);

                    DBFunctions.CreateNewRecord(current);
                }
                else
                {
                    Partner current = new Partner(textBox2.Text, textBox1.Text, textBox3.Text, textBox4.Text, (byte)numericUpDown1.Value, textBox5.Text, textBox6.Text);
                    current.Partner_id = old.Partner_id;

                    DBFunctions.UpdateRecord(current, old);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            AddressFrm dialog = new AddressFrm();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                textBox3.Text = dialog.AddressFull;
                if (textBox4.Text == "")
                {
                    textBox4.Text = dialog.AddressFull;
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            AddressFrm dialog = new AddressFrm();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                textBox4.Text = dialog.AddressFull;
            }
        }
    }
}
