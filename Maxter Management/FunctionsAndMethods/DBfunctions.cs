using Maxter_Management.Models;
using System;
using System.Linq;
//using System.Windows.Controls;
using System.Windows.Forms;


namespace Maxter_Management
{
    static class DBFunctions
    {
        //static SqlCommand command;
        //static SqlConnection connection;

        static DBFunctions()
        {
            /*try
            {
                command = new SqlCommand();
                connection = new SqlConnection();
                connection.ConnectionString = ConfigurationManager.ConnectionStrings["MaxterDBEntities"].ConnectionString;
                connection.Open();
                command.Connection = connection;
                MessageBox.Show("Connection established!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception)
            {
                //throw new ArgumentException("Csatlakozás nem sikerült!", e);
                MessageBox.Show("Connection failed!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }*/
        }

        public static void CreateNewRecord(object item)
        {
            try
            {
                using (var context = new MaxterDBEntities())
                {
                    switch (item)
                    {
                        case EAN a:
                            context.EANs.Add(a);
                            break;
                        case Order a:
                            context.Orders.Add(a);
                            break;
                        case Order_detail a:
                            context.Order_detail.Add(a);
                            break;
                        case Partner a:
                            context.Partners.Add(a);
                            break;
                        case Price a:
                            context.Prices.Add(a);
                            break;
                        case Product a:
                            context.Products.Add(a);
                            break;
                        default:
                            throw new ArgumentException("Item cannot be added to database!");
                            
                    }
                    context.SaveChanges();
                }
            }
            catch (ArgumentException e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Database error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                
            }
        }

        public static void UpdateRecord(object item, object old)
        {
            try
            {
                using (MaxterDBEntities context = new MaxterDBEntities())
                {
                    switch (item)
                    {
                        case EAN a:
                            //context.EANs.AddOrUpdate();
                            //var eANUpdate = context.EANs.Find((old as EAN ).EAN_id);
                            //eANUpdate.EAN1 = (item as EAN).EAN1;
                            //eANUpdate.Quantity = (item as EAN).Quantity;

                            //context.EANs.AddOrUpdate(x => x.EAN_id, (EAN)item);
                            int id = (old as EAN).EAN_id;
                            string ean = (item as EAN).EAN1;
                            int qty = (int)(item as EAN).Quantity;

                            EAN eanUpdate = context.EANs.Where(x => x.EAN_id == id).FirstOrDefault();
                            //eanUpdate.EAN1 = (item as EAN).EAN1;
                            //eanUpdate.Quantity = (item as EAN).Quantity;
                            eanUpdate.EAN1 = ean;
                            eanUpdate.Quantity = qty;
                            break;
                        case Order a:
                            context.Orders.Add(a);
                            break;
                        case Order_detail a:
                            context.Order_detail.Add(a);
                            break;
                        case Partner a:
                            var partnerUpdate = context.Partners.Find((old as Partner).Partner_id);
                            partnerUpdate.Name = (item as Partner).Name;
                            partnerUpdate.Invoice_Address = (item as Partner).Invoice_Address;
                            partnerUpdate.Shipping_Address = (item as Partner).Shipping_Address;
                            partnerUpdate.Email = (item as Partner).Email;
                            partnerUpdate.Telephone = (item as Partner).Telephone;
                            partnerUpdate.Price_Category = (item as Partner).Price_Category;
                            break;
                        case Price a:
                            int idPrice = (old as Price).Price_id;
                            decimal price = (decimal) (item as Price).Price1;
                            byte priceCategory = (byte) (old as Price).Price_Category;

                            Price priceUpdate = context.Prices.Where(x => x.Price_id == idPrice).FirstOrDefault();
                            priceUpdate.Price1 = price;
                            break;
                        case Product a:
                            var productUpdate = context.Products.Find((old as Product).SKU);
                            productUpdate.Stock_Quantity = (item as Product).Stock_Quantity;
                            productUpdate.Min_Stock_Quantity = (item as Product).Min_Stock_Quantity;
                            productUpdate.VTSZ = (item as Product).VTSZ;
                            productUpdate.Description = (item as Product).Description;
                            productUpdate.PU = (item as Product).PU;

                            //context.Entry(productUpdate).CurrentValues.SetValues(item);

                            break;
                        default:
                            throw new ArgumentException("Item cannot be updated in the database!");

                    }
                    context.SaveChanges();
                }
            }
            catch (ArgumentException e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Database error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        public static void RemoveRecord(object item)
        {
            try
            {
                using (var context = new MaxterDBEntities())
                {

                    switch (item)
                    {
                        case EAN a:
                            var eANRemove = context.EANs.Find((item as EAN).EAN_id);
                            if (eANRemove != null)
                            {
                                context.EANs.Remove(eANRemove);
                            }
                            else
                            {
                                MessageBox.Show("Record does not exist!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            break;
                        case Order a:
                            var orderRemove = context.Orders.Find((item as Order).Order_id);
                            if (orderRemove != null)
                            {
                                context.Orders.Remove(orderRemove);
                            }
                            else
                            {
                                MessageBox.Show("Record does not exist!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            break;
                        case Order_detail a:
                            var order_DetailRemove = context.Order_detail.Find((item as Order_detail).Order_Detail_id);
                            if (order_DetailRemove != null)
                            {
                                context.Order_detail.Remove(order_DetailRemove);
                            }
                            else
                            {
                                MessageBox.Show("Record does not exist!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            break;
                        case Partner a:
                            var partnerRemove = context.Partners.Find((item as Partner).Partner_id);
                            if (partnerRemove != null)
                            {
                                context.Partners.Remove(partnerRemove);
                            }
                            else
                            {
                                MessageBox.Show("Record does not exist!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            break;
                        case Price a:
                            var priceRemove = context.Prices.Find((item as Price).Price_id);
                            if (priceRemove != null)
                            {
                                context.Prices.Remove(priceRemove);
                            }
                            else
                            {
                                MessageBox.Show("Record does not exist!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            break;
                        case Product a:
                            var productRemove = context.Products.Find((item as Product).SKU);
                            if (productRemove != null)
                            {
                                context.Products.Remove(productRemove);
                            }
                            else
                            {
                                MessageBox.Show("Record does not exist!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            break;
                        default:
                            throw new ArgumentException("Item cannot be added to database!");

                    }
                    context.SaveChanges();
                }
            }
            catch (ArgumentException e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Database error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static bool ConnectionTest()
        {
            

            return true;
        }

        public static ListView FillEansListView(ListView fill, string SKU)
        {
            using (MaxterDBEntities context = new MaxterDBEntities())
            {
                    var foundEan = context.EANs.Where(found => SKU == found.SKU).ToList();
                    

                    foreach (var itemEan in foundEan)
                    {
                    ListViewItem current = new ListViewItem();
                    current.SubItems[0].Text = (itemEan as EAN).EAN_id.ToString();
                        current.SubItems.Add((itemEan as EAN).EAN1);
                        current.SubItems.Add((itemEan as EAN).Quantity.ToString());
                    fill.Items.Add(current);

                }
                
            }

            return fill;
        }

        public static ListView FillPricesListView(ListView fill, string SKU)
        {
            using (MaxterDBEntities context = new MaxterDBEntities())
            {
                var foundPrice = context.Prices.Where(found => SKU == found.SKU).ToList();


                foreach (var itemPrice in foundPrice)
                {
                    ListViewItem current = new ListViewItem();
                    current.SubItems[0].Text = (itemPrice as Price).Price_id.ToString();
                    current.SubItems.Add((itemPrice as Price).Price1.ToString());
                    current.SubItems.Add((itemPrice as Price).Price_Category.ToString());
                    fill.Items.Add(current);
                }

            }

            return fill;
        }

        public static ListView FillOrder_detailListView(ListView fill, string SKU)
        {
            using (MaxterDBEntities context = new MaxterDBEntities())
            {
                var foundOrder_detail = context.Order_detail.Where(found => SKU == found.SKU).ToList();


                foreach (var itemOrder_detail in foundOrder_detail)
                {
                    ListViewItem current = new ListViewItem();
                    current.SubItems[0].Text = (itemOrder_detail as Order_detail).Order_id.ToString();
                    current.SubItems.Add((itemOrder_detail as Order_detail).Ordered_Quantity.ToString());
                    fill.Items.Add(current);

                }

            }

            return fill;
        }
    }
}
