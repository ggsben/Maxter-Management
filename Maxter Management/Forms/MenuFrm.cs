using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Maxter_Management.Forms;
using Maxter_Management.Models;
using Maxter_Management.FunctionsAndMethods;
using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using System.Threading;
using Newtonsoft.Json.Linq;

namespace Maxter_Management.Forms
{
    public partial class MenuFrm : Form
    {
        private string access;
        public int id;

        public MenuFrm()
        {
            if (!File.Exists("language.txt"))
            {
                File.WriteAllText("language.txt", "en");
            }
            else
            {
                string lang = File.ReadAllText("language.txt");
                switch (lang)
                {
                    case "en":
                        Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");
                        break;
                    case "hu":
                        Thread.CurrentThread.CurrentUICulture = new CultureInfo("hu");
                        break;
                    default:
                        Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");
                        break;
                }
            }
            InitializeComponent();
            Font = new Font(new FontFamily("Segoe UI"), Font.Size);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Product test = new Product();

            test.SKU = textBox1.Text;
            test.Description = textBox2.Text;
            test.VTSZ = textBox3.Text;
            test.Stock_Quantity = (short)numericUpDown1.Value;
            test.Min_Stock_Quantity = (short)numericUpDown2.Value;

            DBFunctions.CreateNewRecord(test);
        }  //DEBUG

        private void button2_Click(object sender, EventArgs e)
        {
            Product test = new Product();

            test.SKU = textBox4.Text;

            DBFunctions.RemoveRecord(test);
        } //DEBUG

        private void button3_Click(object sender, EventArgs e)
        {
            Product test = new Product();

            test.SKU = textBox7.Text;
            test.Description = textBox6.Text;
            test.VTSZ = textBox5.Text;
            test.Stock_Quantity = (short)numericUpDown4.Value;
            test.Min_Stock_Quantity = (short)numericUpDown3.Value;

            Product old = null;

            using (var context = new MaxterDBEntities())
            {
                old = context.Products.Find(textBox8.Text);
            }

            DBFunctions.UpdateRecord(test, old);
        } //DEBUG

        private void button4_Click(object sender, EventArgs e)
        {
            MenuFunctions.MainMenu(this, textBox9.Text);
        }

        private void MenuFrm_Load(object sender, EventArgs e)
        {
            //Font = new Font(new FontFamily("Segoe UI"), 1.25f * Font.Size);
            Font = new Font(new FontFamily("Segoe UI"), Font.Size);

            
            LoginFrm login = new LoginFrm();
            this.Hide();
            ShowInTaskbar = false;
            
            if (login.ShowDialog() != DialogResult.OK)
            {
                Application.Exit();
            }
            else
            {
                access = login.userAccess[0];
                id = int.Parse(login.userAccess[1]);
                Tag = access;
                ShowInTaskbar = true;
                MenuFunctions.MainMenu(this, access);
                this.Show();
                this.WindowState = FormWindowState.Maximized;
                MenuFunctions.FormCleanUp();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            EAN test = new EAN(EANNUMBER.Value.ToString(), (int)EANID.Value, SKUTEXT.Text);
            //test.EAN_id = (int)EANID.Value;
            test.EAN1 = EANNUMBER.Value.ToString();
            test.SKU = SKUTEXT.Text;
            test.Quantity = 10;

            DBFunctions.CreateNewRecord(test);
        }

        private void PRICEBUTTON_Click(object sender, EventArgs e)
        {
            Price test = new Price(SKUTEXT.Text, EANNUMBER.Value, (byte)PRICECATEGORY.Value);
            //test.Price_id = (int)EANID.Value;

            using(MaxterDBEntities context = new MaxterDBEntities())
            {
                int max = 0;
                foreach (Price item in context.Prices)
                {
                    if (item.Price_id > max)
                    {
                        max = item.Price_id;
                    }
                }

                test.Price_id = max + 1;
            }

            test.Price1 = EANNUMBER.Value;
            test.SKU = SKUTEXT.Text;
            test.Price_Category = (byte)PRICECATEGORY.Value;

            DBFunctions.CreateNewRecord(test);
        }

        private void invoiceButton_Click(object sender, EventArgs e)
        {
            List<InvoiceItem> list = new List<InvoiceItem>();

            FunctionsAndMethods.Address testAddress = new FunctionsAndMethods.Address(true);
            Client testClient = new Client(true, testAddress);

            for (int i = 0; i < 3; i++)
            {
                list.Add(new InvoiceItem(true));
            }

            //Invoice testInvoice = new Invoice(testClient, list, 1941192349);

            Invoice testInvoice = new Invoice(1407549930, list);

            var result = ApiFunctions.CreateInvoiceAsync(testInvoice);

            MessageBox.Show(result.ToString(), "Result", MessageBoxButtons.OK);
        }

        private void button6_Click(object sender, EventArgs e)
        {

            var result = ApiFunctions.GetInvoiceAsync();

            //List<Invoice> test = result.Result;

            var parsedObject = JObject.Parse(result.ToString());

            var idJson = parsedObject["data"]["id"].ToString();

            //MessageBox.Show(result.ToString(), "Result", MessageBoxButtons.OK);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Client newClient = new Client(true, new FunctionsAndMethods.Address(true));

            var outResult = ApiFunctions.CreateClientAsync(newClient);//Task.Run(() => ApiFunctions.CreateClientAsync(result));

            //string output = outResult.Result.ToString();

            var parsedObject = JObject.Parse(outResult.Result);

            var idJson = parsedObject["data"]["id"].ToString();

            File.WriteAllText("resultTest.txt", outResult.Result);
            File.AppendAllText("id.txt", idJson + "\n");

            label13.Text = idJson;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (label13.Text != "label13")
            {
                int id = int.Parse(label13.Text);
                var result = ApiFunctions.DeleteClientAsync(id);//Task.Run(() => ApiFunctions.DeleteClientAsync(id));
            }
            else
            {
                MessageBox.Show("No id");
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Client newClient = new Client(true, new FunctionsAndMethods.Address(true));

            //string result = "";
            var outResult = ApiFunctions.CreateClientAsync(newClient);//Task.Run(() => ApiFunctions.CreateClientAsync(result));

            //string output = outResult.Result.ToString();

            var parsedObject = JObject.Parse(outResult.Result);

            var idJson = parsedObject["data"]["id"].ToString();

            File.WriteAllText("resultTest.txt", outResult.Result);
            File.AppendAllText("id.txt", idJson + "\n");

            int id = int.Parse(idJson);

            List<InvoiceItem> list = new List<InvoiceItem>();

            FunctionsAndMethods.Address testAddress = new FunctionsAndMethods.Address(true);
            Client testClient = new Client(true, testAddress);

            for (int i = 0; i < 3; i++)
            {
                list.Add(new InvoiceItem(true));
            }

            //testClient.Id = id;

            Invoice testInvoice = new Invoice(1407549930, list);

            var result = ApiFunctions.CreateInvoiceAsync(testInvoice);

            MessageBox.Show(result.ToString(), "Result", MessageBoxButtons.OK);

            //if (testClient.Id != null)
            //{
                
            //    var resultDelete = ApiFunctions.DeleteClientAsync(testClient.Id);//Task.Run(() => ApiFunctions.DeleteClientAsync(id));
            //}
            //else
            //{
            //    MessageBox.Show("No id");
            //}
        }
    }
}
