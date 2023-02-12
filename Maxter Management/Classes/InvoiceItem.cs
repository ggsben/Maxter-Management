using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using Maxter_Management.Models;

namespace Maxter_Management.FunctionsAndMethods
{
    public class InvoiceItem
    {
        private string description;
        private decimal net_unit_price;
        private decimal qty;
        private string unit;
        private int vat_id;
        private string item_comment = "";

        public string Description { get => description; set => description = value; }
        public decimal Net_unit_price { get => net_unit_price; set => net_unit_price = value; }
        public decimal Qty { get => qty; set => qty = value; }
        public string Unit { get => unit; set => unit = value; }
        public int Vat_id { get => vat_id; set => vat_id = value; }
        public string Item_comment { get => item_comment; set => item_comment = value; }

        /*public InvoiceItem(Product product, Order order)
        {
            Description = product.Description;
            using (MaxterDBEntities context = new MaxterDBEntities())
            {
                Order_detail price =
                    context.Order_detail.FirstOrDefault(x => x.Order_id == order.Order_id && x.SKU == product.SKU);

                Net_unit_price = (decimal)price.Item_Price;
                Qty = price.Ordered_Quantity;
            }

            Unit = product.PU;
            Vat_id = int.Parse(product.VTSZ);
            Item_comment = "";
        }*/

        public InvoiceItem(Order order)
        {
            
            using (MaxterDBEntities context = new MaxterDBEntities())
            {
                Order_detail found =
                    context.Order_detail.FirstOrDefault(x => x.Order_id == order.Order_id/* && x.SKU == product.SKU*/);

                Net_unit_price = (decimal)found.Item_Price;
                Qty = found.Ordered_Quantity;
                Description = found.Product.Description;
                Unit = found.Product.PU;
                Vat_id = int.Parse(found.Product.VTSZ);
            }

            
            Item_comment = "";
        }

        public static List<InvoiceItem> GetList(Order order)
        {
            List<InvoiceItem> items = new List<InvoiceItem>();

            using (MaxterDBEntities context = new MaxterDBEntities())
            {
                List<Order_detail> found =
                    context.Order_detail.Where(x => x.Order_id == order.Order_id/* && x.SKU == product.SKU*/).ToList();

                

                for (int i = 0; i < found.Count; i++)
                {
                    InvoiceItem current = new InvoiceItem(found[i]);
                    items.Add(current);
                }
                //Net_unit_price = (decimal)found.Item_Price;
                //Qty = found.Ordered_Quantity;
                //Description = found.Product.Description;
                //Unit = found.Product.PU;
                //Vat_id = int.Parse(found.Product.VTSZ);
            }

            return items;
        }

        public InvoiceItem(Order_detail input)
        {
            Description = input.Product.Description;
            Net_unit_price = (decimal)input.Item_Price;
            Qty = input.Ordered_Quantity;
            Unit = input.Product.PU;
            //Vat_id = int.Parse(input.Product.VTSZ);
            Vat_id = 1010101010;
        }

        public InvoiceItem(bool test)
        {
            Description = "Teszt termek";
            Net_unit_price = 10.0M;
            Qty = 15;
            Unit = "db";
            Vat_id = 1010101010;
        }
    }
}