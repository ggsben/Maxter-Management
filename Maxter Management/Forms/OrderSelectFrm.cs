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

namespace Maxter_Management.Forms
{
    public partial class OrderSelectFrm : Form
    {

        private Order found = null;
        private int orderId;

        public OrderSelectFrm()
        {
            InitializeComponent();
        }

        public Order Found { get => found; set => found = value; }
        public int OrderId { get => orderId; set => orderId = value; }

        private void button1_Click(object sender, EventArgs e)
        {
            using (MaxterDBEntities context = new MaxterDBEntities())
            {
                string id = textBox1.Text.Trim();
               

                if (!int.TryParse(id, out int test))
                {
                    MessageBox.Show("Invalid ID. ID can only contain numbers.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    DialogResult = DialogResult.None;
                }
                else
                {
                    try
                    {
                        Found = context.Orders.FirstOrDefault(x => x.Order_id == test);
                        OrderId = test;

                        if (Found == default)
                        {
                            MessageBox.Show("No order found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            DialogResult = DialogResult.None;
                        }
                        else if (Found.Shipped_date != null)
                        {
                            MessageBox.Show("Order has been already shipped.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            DialogResult = DialogResult.None;
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
    }
}
