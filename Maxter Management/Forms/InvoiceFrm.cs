using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Maxter_Management.FunctionsAndMethods;
using Maxter_Management.Models;

namespace Maxter_Management.Forms
{

    public partial class InvoiceFrm : Form
    {
        private int orderId;
        private Order foundOrder = null;
        private List<InvoiceItem> items = null;
        private Client client = null;

        public Order FoundOrder { get => foundOrder; set => foundOrder = value; }

        public InvoiceFrm(int getOrder)
        {
            InitializeComponent();
            orderId = getOrder;
        }

        private void InvoiceFrm_Load(object sender, EventArgs e)
        {
            using (MaxterDBEntities context = new MaxterDBEntities())
            {
                foundOrder = context.Orders.FirstOrDefault(x => x.Order_id == orderId);

                items = InvoiceItem.GetList(foundOrder);

                var foundClient = context.Partners.FirstOrDefault(x => x.Partner_id == foundOrder.Partner_id);

                client = new Client(foundClient);

                textBox1.Text = foundOrder.Partner.Name;
                textBox1.Enabled = false;
                textBox2.Text = foundOrder.Partner.Invoice_Address.Replace(';', ' ');
                textBox2.Enabled = false;
                textBox3.Text = foundOrder.Partner.Shipping_Address.Replace(';', ' ');
                textBox3.Enabled = false;
                dateTimePicker1.Value = DateTime.Now;
                dateTimePicker2.Value = foundOrder.Required_date;
                textBox5.Text = foundClient.TIN;
                textBox5.Enabled = false;

                float totalNet = 0;
                float total = 0;

                foreach (Order_detail item in foundOrder.Order_detail)
                {
                    totalNet += (float)item.Item_Price * item.Ordered_Quantity;
                    total += ((float) item.Item_Price * (float) 1.27) * item.Ordered_Quantity;
                }

                label10.Text = totalNet.ToString() + " Ft";
                label12.Text = total.ToString() + " Ft";


                comboBox1.Items.Add("Készpénz");
                comboBox1.Items.Add("Átutalás");
                comboBox1.SelectedIndex = 0;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            switch (MessageBox.Show("Create invoice?", "Confirm", MessageBoxButtons.YesNo))
                {
                case DialogResult.Yes:
                    try
                    {
                        Invoice current = new Invoice(foundOrder.Order_id, items);
                        current.Comment = textBox4.Text;
                        if (comboBox1.SelectedIndex == 0)
                        {
                            current.Payment_method = 1;
                        }
                        else
                        {
                            current.Payment_method = 2;
                            current.Due_date = dateTimePicker2.Value.ToString("yyyy-MM-dd");
                        }
                        var result = ApiFunctions.CreateInvoiceComplexAsync(current, client);
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    break;
                case DialogResult.No:
                    break;
                }
        }
    }
}
