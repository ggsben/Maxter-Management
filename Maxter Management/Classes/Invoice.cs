using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Threading;
using Maxter_Management.Models;

namespace Maxter_Management.FunctionsAndMethods
{
    public class Invoice
    {

        private string fulfillment_date;
        private string due_date;
        private int payment_method = 1;
        private string comment = "";
        private string template_lang_code = "hu";
        private int electronic_invoice = 0;
        private string currency = "HUF";
        private int client_uid;
        private int block_uid = 0;
        private int type = 3;
        private int round_to = 5;
        private int bank_account_uid = 0;
        private List<InvoiceItem> items;


        public Invoice(Client paramClient, List<InvoiceItem> paramItems, Order order)
        {
            Items = paramItems;
            //Client = paramClient;
            //Id = order.Order_id;

            CultureInfo originalCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("nl-NL");
            //Date = order.Order_date.ToShortDateString();
            Fulfillment_date = (order.Shipped_date is DateTime ? (DateTime) order.Shipped_date : default).ToShortDateString();
            Due_date = order.Required_date.ToShortDateString();
            Thread.CurrentThread.CurrentCulture = originalCulture;

            //Invoice_no = DateTime.Now.Year.ToString() + "-" + Id;
            Currency = "HUF";
        }

        /*public Invoice(Client paramClient, List<InvoiceItem> paramItems, int paramId)
        {
            Items = paramItems;
            //Client = paramClient;
            //Id = paramId;

            CultureInfo originalCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("nl-NL");
            //Date = DateTime.Now.ToShortDateString();
            Fulfillment_date = DateTime.Now.ToShortDateString();
            Due_date = DateTime.Now.ToShortDateString();
            Thread.CurrentThread.CurrentCulture = originalCulture;

            //Invoice_no = DateTime.Now.Year.ToString() + "-" + Id;
            Currency = "HUF";
        }*/

        public Invoice(int clientId, List<InvoiceItem> paramItems)
        {
            Client_uid = clientId;
            Items = paramItems;
            //Client = paramClient;
            //Id = paramId;

            CultureInfo originalCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("nl-NL");
            //Date = DateTime.Now.ToShortDateString();
            Fulfillment_date = DateTime.Now.ToString("yyyy-MM-dd");
            Due_date = DateTime.Now.ToString("yyyy-MM-dd");
            Thread.CurrentThread.CurrentCulture = originalCulture;

            //Invoice_no = DateTime.Now.Year.ToString() + "-" + Id;
            Currency = "HUF";
        }


        public string Fulfillment_date { get => fulfillment_date; set => fulfillment_date = value; }
        public string Due_date { get => due_date; set => due_date = value; }
        public int Payment_method { get => payment_method; set => payment_method = value; }
        public string Comment { get => comment; set => comment = value; }
        public string Template_lang_code { get => template_lang_code; set => template_lang_code = value; }
        public int Electronic_invoice { get => electronic_invoice; set => electronic_invoice = value; }
        public string Currency { get => currency; set => currency = value; }
        public int Client_uid { get => client_uid; set => client_uid = value; }
        public int Block_uid { get => block_uid; set => block_uid = value; }
        public int Type { get => type; set => type = value; }
        public int Round_to { get => round_to; set => round_to = value; }
        public int Bank_account_uid { get => bank_account_uid; set => bank_account_uid = value; }
        public List<InvoiceItem> Items { get => items; set => items = value; }
    }
}