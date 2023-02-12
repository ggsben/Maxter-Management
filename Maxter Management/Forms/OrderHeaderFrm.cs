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
    public partial class OrderHeaderFrm : Form
    {
        public string partnerName;
        public byte priceCategory;
        public int orderId;

        public OrderHeaderFrm()
        {
            InitializeComponent();
            numericUpDown1.Maximum = int.MaxValue;
            numericUpDown1.Minimum = 1;
            Font = new Font(new FontFamily("Segoe UI"), Font.Size);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (numericUpDown1.Value > 0 && dateTimePicker1.Value > DateTime.Now)
                {
                    Partner found;

                    using (MaxterDBEntities context = new MaxterDBEntities())
                    {
                        found = context.Partners.Where(x => x.Partner_id == (int) numericUpDown1.Value).FirstOrDefault();

                        if (found != default)
                        {
                            Order current = new Order(found.Partner_id);
                            current.Order_date = DateTime.Now;
                            current.Required_date = dateTimePicker1.Value;
                            priceCategory = (byte)found.Price_Category;
                            Tag = found.Name;

                            DBFunctions.CreateNewRecord(current);

                            orderId = current.Order_id;
                        }
                        else
                        {
                            MessageBox.Show("Partner with such ID does not exist!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            DialogResult = DialogResult.None;
                        }
                    }
                }
                else if (dateTimePicker1.Value <= DateTime.Now)
                {
                    MessageBox.Show("Required date must be in the future!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    DialogResult = DialogResult.None;
                }
                else if (numericUpDown1.Value <= 0)
                {
                    MessageBox.Show("Partner ID must be valid!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    DialogResult = DialogResult.None;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                DialogResult = DialogResult.None;
                throw;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
}
