using Maxter_Management.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Entity;

namespace Maxter_Management.Forms
{
    public partial class NewProductFrm : Form
    {
        private Product old;

        public NewProductFrm()
        {
            InitializeComponent();
            Font = new Font(new FontFamily("Segoe UI"), Font.Size);
        }

        public NewProductFrm(string sku) : this()
        {
            Text = "Update Product";
            using (MaxterDBEntities context = new MaxterDBEntities())
            {
                old = context.Products.Where(x => x.SKU == sku).FirstOrDefault();
                if (old != null)
                {
                    textBox1.Text = old.SKU;
                    textBox2.Text = old.Description;
                    textBox3.Text = old.VTSZ;
                    numericUpDown2.Value = (int) old.Min_Stock_Quantity;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (old == null)
                {
                    Product create = new Product();
                    create.SKU = textBox1.Text;
                    create.Description = textBox2.Text;
                    create.VTSZ = textBox3.Text;
                    create.Stock_Quantity = 0;
                    create.Min_Stock_Quantity = (short)numericUpDown2.Value;
                    create.PU = textBox4.Text;

                    using (MaxterDBEntities context = new MaxterDBEntities())
                    {

                
                        if (context.Products.Count(x => x.SKU == create.SKU) == 0)
                        {
                            DBFunctions.CreateNewRecord(create);
                        }
                        else
                        {
                            MessageBox.Show("A product with the same SKU already exists!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            DialogResult = DialogResult.None;
                            //foundSKU = null;
                        }
                    }
                }
                else
                {
                    Product current = new Product();
                    current.SKU = textBox1.Text;
                    current.Description = textBox2.Text;
                    current.VTSZ = textBox3.Text;
                    current.Stock_Quantity = 0;
                    current.Min_Stock_Quantity = (short)numericUpDown2.Value;
                    current.Stock_Quantity = old.Stock_Quantity;
                    current.PU = textBox4.Text;

                    DBFunctions.UpdateRecord(current, old);
                }
            }
            catch (Exception exception)
            {
                DialogResult = DialogResult.None;
                MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
    }
}
