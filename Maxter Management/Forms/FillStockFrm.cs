using Maxter_Management.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Maxter_Management.Forms
{
    public partial class FillStockFrm : Form
    {
        private List<string> skuList;
        private List<int> fillList;

        public FillStockFrm()
        {
            InitializeComponent();

            skuList = new List<string>();
            fillList = new List<int>();

            listView1.FullRowSelect = true;
            listView1.View = View.Details;
            listView1.HideSelection = false;

            listView1.Columns.Add("SKU");
            listView1.Columns.Add("Description");
            listView1.Columns.Add("Stock");
            
            listView2.FullRowSelect = true;
            listView2.View = View.Details;
            listView2.HideSelection = false;

            listView2.Columns.Add("SKU");
            listView2.Columns.Add("Description");
            listView2.Columns.Add("Quantity");

            for (int i = 0; i < listView1.Columns.Count; i++)
            {
                listView1.Columns[i].Width = listView1.Width / listView1.Columns.Count;
            }
            LVRefresh();
        }

        private void LVRefresh()
        {
            listView1.Items.Clear();

            using (MaxterDBEntities context = new MaxterDBEntities())
            {
                foreach (var item in context.Products)
                {
                    ListViewItem current = new ListViewItem(item.SKU);
                    current.SubItems.Add(item.Description);
                    current.SubItems.Add(item.Stock_Quantity.ToString());
                    current.Tag = $"{item.SKU}";

                    bool contains = false;

                    foreach (ListViewItem lvItem in listView2.Items)
                    {
                        if (current.ToString() == lvItem.ToString())
                        {
                            contains = true;
                        }
                    }

                    if (!contains)
                    {
                        listView1.Items.Add(current);
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count >= 1)
            {
                ListViewItem add = new ListViewItem(listView1.SelectedItems[0].Text);
                add.SubItems.Add(listView1.SelectedItems[0].SubItems[1].Text);
                add.SubItems.Add(numericUpDown1.Value.ToString());
                add.Tag = listView1.SelectedItems[0].SubItems[1];

                listView2.Items.Add(add);
                skuList.Add(listView1.SelectedItems[0].Text);
                fillList.Add((int)numericUpDown1.Value);

                listView1.SelectedItems[0].Remove();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (listView2.SelectedItems.Count >= 1)
            {
                skuList.RemoveAt(listView2.SelectedIndices[0]);
                fillList.RemoveAt(listView2.SelectedIndices[0]);
                listView2.SelectedItems[0].Remove();
                LVRefresh();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                using (MaxterDBEntities context = new MaxterDBEntities())
                {
                    for (int i = 0; i < skuList.Count; i++)
                    {
                        string currentSku = skuList[i];
                        short fill = Convert.ToInt16(fillList[i]);

                        Product old = context.Products.FirstOrDefault(x => x.SKU.ToString() == currentSku);

                        if (old.Stock_Quantity != null)
                        {
                            fill += (short)old.Stock_Quantity;
                        }

                        Product current = new Product();
                        current.Description = old.Description;
                        current.VTSZ = old.VTSZ;
                        current.SKU = old.SKU;
                        current.Stock_Quantity = fill;
                        current.EANs = old.EANs;
                        current.Min_Stock_Quantity = old.Min_Stock_Quantity;
                        current.Order_detail = old.Order_detail;
                        current.PU = old.PU;
                        current.Prices = old.Prices;

                        DBFunctions.UpdateRecord(current, old);
                    }
                }
            }
        }
    }
}
