using Maxter_Management.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Maxter_Management.Forms
{
    public partial class EanFrm : Form
    {
        string internalSKU;
        private EAN old;

        public EanFrm(string sku)
        {
            InitializeComponent();
            Font = new Font(new FontFamily("Segoe UI"), Font.Size);
            internalSKU = sku;
            old = null;
        }

        public EanFrm(int id)
        {
            InitializeComponent();
            Font = new Font(new FontFamily("Segoe UI"), Font.Size);
            Text = "Modify EAN";
            using (MaxterDBEntities context = new MaxterDBEntities())
            {
                old = (context.EANs.Where(x => x.EAN_id.Equals(id)).FirstOrDefault());
                if (old != null)
                {
                    textBox1.Text = old.EAN1;
                    numericUpDown1.Value = (int) old.Quantity;
                }
            }
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox1.Text.Trim().Length > 0)
                {
                    if (old == null)
                    {
                        EAN current = new EAN(textBox1.Text.Trim(), (int)numericUpDown1.Value, internalSKU);
                        //current.EAN1 = textBox1.Text.Trim();
                        //current.SKU = internalSKU;
                        //current.Quantity = (int)numericUpDown1.Value;
                        /*using (MaxterDBEntities context = new MaxterDBEntities())
                    {
                        int max = 0;
                        foreach (EAN item in context.EANs)
                        {
                            if (item.EAN_id > max)
                            {
                                max = item.EAN_id;
                            }
                        }

                        current.EAN_id = max + 1;
                    }*/

                        DBFunctions.CreateNewRecord(current);
                    }
                    else
                    {
                        EAN current = new EAN(textBox1.Text.Trim(), (int)numericUpDown1.Value, old.SKU);
                        DBFunctions.UpdateRecord(current, old);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating EAN record: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.None;
            }
            
        }
    }
}
