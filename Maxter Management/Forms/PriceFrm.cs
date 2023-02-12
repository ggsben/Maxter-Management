using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Core.Objects;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Maxter_Management.Models;

namespace Maxter_Management.Forms
{
    public partial class PriceFrm : Form
    {
        string internalSKU;
        private Price old;

        public PriceFrm()
        {
            InitializeComponent();
            Font = new Font(new FontFamily("Segoe UI"), Font.Size);
            numericUpDown1.Maximum = Decimal.MaxValue;
            numericUpDown2.Maximum = byte.MaxValue;
            numericUpDown2.Minimum = 0;
        }

        public PriceFrm(string sku) : this()
        {
            internalSKU = sku;
        }

        public PriceFrm(int id) : this()
        {
            Text = "Modify Price";
            using (MaxterDBEntities context = new MaxterDBEntities())
            {
                old = (context.Prices.Where(x => x.Price_id.Equals(id)).FirstOrDefault());
                if (old != null)
                {
                    numericUpDown1.Value = (int)old.Price1;
                    numericUpDown2.Value = (int)old.Price_Category;
                    numericUpDown2.Enabled = false;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (old == null)
                {
                    Price current = new Price(internalSKU, numericUpDown1.Value, (byte)numericUpDown2.Value);
                    DBFunctions.CreateNewRecord(current);
                    
                }
                else
                {
                    Price current = new Price();
                    current.SKU = old.SKU;
                    current.Price_Category = old.Price_Category;
                    current.Price1 = numericUpDown1.Value;
                    current.Price_id = old.Price_id;

                    DBFunctions.UpdateRecord(current, old);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }
    }
}
