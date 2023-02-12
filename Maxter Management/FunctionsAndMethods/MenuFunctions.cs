using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using Maxter_Management.Classes;
using Maxter_Management.Models;
using Maxter_Management.Forms;
using Maxter_Management.Resources;
using Newtonsoft.Json.Linq;

namespace Maxter_Management.FunctionsAndMethods
{
    public static class MenuFunctions
    {

        #region Methods
        public static string[] Login(string username,
                string password) //Queries given user and checks if encrypted password matches, returns with access level
        {
            try
            {
                UsersDBEntities usersDb = new UsersDBEntities();
                string foundPass = (from users in usersDb.Users where username == users.Username select users.Password)
                    .First();
                string passTest = BitConverter
                    .ToString(new SHA256CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(password + "hahayessecurity")))
                    .Replace("-", "").ToLower();
                if (passTest == foundPass)
                {
                    //System.Windows.Forms.MessageBox.Show("Successful login!", "Success", System.Windows.Forms.MessageBoxButtons.OK);
                    string access = (from users in usersDb.Users where username == users.Username select users.Access)
                        .First();
                    string foundId = (from users in usersDb.Users where username == users.Username select users.Id)
                        .First().ToString();

                    string[] user = new string[2];
                    user[0] = access;
                    user[1] = foundId;
                    return user;
                }
                else
                {
                    MessageBox.Show(DynamicResources.MenuFunctions_Login_Failed_login_, DynamicResources.MenuFunctions_Login_Error, MessageBoxButtons.OK);
                    return null;
                }
            }
            catch (Exception)
            {
                MessageBox.Show(DynamicResources.MenuFunctions_Login_Failed_login_, DynamicResources.MenuFunctions_Login_Error, MessageBoxButtons.OK);
                return null;
            }
        }

        public static void FormCleanUp() //Disposes all controls that are unneeded
        {
            if (Application.OpenForms[0].Controls.Count > 0)
            {
                for (int i = Application.OpenForms[0].Controls.Count - 1; i >= 0; i--)
                {
                    Control item = Application.OpenForms[0].Controls[i];
                    if (!(item is MenuStrip))
                    {
                        if (item is DataGridView)
                        {
                            item.Dispose();
                        }
                        else
                        {
                            //Application.OpenForms[0].Controls.Remove(item);
                            item.Dispose();
                        }
                        
                    }
                }
            }

            Application.OpenForms[0].SizeChanged -= Product_Click;
        }

        public static void
            MainMenu(Form menu, string access) //Creates the menu strip for the main menu based on access level
        {
            MenuStrip ms = new MenuStrip() {AutoSize = true};
            menu.Controls.Add(ms);
            ToolStripMenuItem file = new ToolStripMenuItem(DynamicResources.MenuFunctions_MainMenu_File);
            ms.Items.Add(file);
            ToolStripMenuItem exit = new ToolStripMenuItem(DynamicResources.MenuFunctions_MainMenu_Exit);


            //if (access[0] != 'N' || access[1] != 'N')
            //{
            //    ToolStripMenuItem data = new ToolStripMenuItem("Data");
            //    ms.Items.Add(data);
            //    if (access[0] != 'N')
            //    {
            //        ToolStripMenuItem products = new ToolStripMenuItem("Products");
            //        products.Click += Product_Click;
            //        data.DropDownItems.Add(products);
            //    }

            //    if (access[1] != 'N')
            //    {
            //        ToolStripMenuItem partners = new ToolStripMenuItem("Partners");
            //        partners.Click += Partners_Click;
            //        data.DropDownItems.Add(partners);
            //    }
            //}

            if (access[0] != 'N')
            {
                ToolStripMenuItem adminSettings = new ToolStripMenuItem(DynamicResources.MenuFunctions_MainMenu_Admin);
                file.DropDownItems.Add(adminSettings);
                ToolStripMenuItem invoiceSettings = new ToolStripMenuItem(DynamicResources.MenuFunctions_MainMenu_InvoiceSettings);
                file.DropDownItems.Add(invoiceSettings);
                adminSettings.Click += AdminSettings_Click;
                invoiceSettings.Click += InvoiceSettings_Click;
            }

            if (access[1] != 'N')
            {
                ToolStripMenuItem settings = new ToolStripMenuItem(DynamicResources.MenuFunctions_MainMenu_Pass);
                file.DropDownItems.Add(settings);
                settings.Click += Settings_Click;
                ToolStripMenuItem language = new ToolStripMenuItem(DynamicResources.MenuFunctions_MainMenu_Lang);
                file.DropDownItems.Add(language);
                language.Click += Language_Click;
                ToolStripSeparator separator = new ToolStripSeparator();
                file.DropDownItems.Add(separator);
            }

            ToolStripMenuItem data = new ToolStripMenuItem(DynamicResources.MenuFunctions_MainMenu_Data);
            ms.Items.Add(data);

            ToolStripMenuItem products = new ToolStripMenuItem(DynamicResources.MenuFunctions_MainMenu_Products);
            products.Click += Product_Click;
            data.DropDownItems.Add(products);

            ToolStripMenuItem partners = new ToolStripMenuItem(DynamicResources.MenuFunctions_MainMenu_Partners);
            partners.Click += Partners_Click;
            data.DropDownItems.Add(partners);

            if (access[2] != 'N')
            {
                ToolStripMenuItem orders = new ToolStripMenuItem(DynamicResources.MenuFunctions_MainMenu_Orders);
                ms.Items.Add(orders);
                orders.Click += Orders_Click;
            }

            

            //if (access[3] != 'N')
            //{
            //    ToolStripMenuItem stock = new ToolStripMenuItem("Stock movement");
            //    ms.Items.Add(stock);
            //}

            if (access[3] != 'N')
            {
                ToolStripMenuItem billing = new ToolStripMenuItem(DynamicResources.MenuFunctions_MainMenu_Billing);
                ms.Items.Add(billing);
                billing.Click += Billing_Click;
            }

            file.DropDownItems.Add(exit);
            exit.Click += Exit_Click;
        }

        private static void InvoiceSettings_Click(object sender, EventArgs e)
        {
            InvoiceSettingsFrm dialog = new InvoiceSettingsFrm();
            dialog.ShowDialog();
        }

        private static bool UpdateStock(string sku, int amount, int previous)
        {
            using (MaxterDBEntities context = new MaxterDBEntities())
            {
                var found = context.Products.FirstOrDefault(x => x.SKU == sku);

                if (found != default)
                {
                    if (found.Stock_Quantity + (short)(previous - amount) >= 0)
                    {
                        found.Stock_Quantity += (short)(previous - amount);
                        context.SaveChanges();
                        return true;
                    }
                    else
                    {
                        MessageBox.Show(string.Format(DynamicResources.MenuFunctions_UpdateStock_Not_enough_stock_for_requested_amount__Current_stock___0_, found.Stock_Quantity));
                        return false;
                    }
                    //found.Stock_Quantity -= (short)(amount-(previous*-1));
                }
                return false;
            }
        }


        private static void OrderDgv_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                using (MaxterDBEntities context = new MaxterDBEntities())
                {
                    if ((sender as DataGridView).CurrentRow.Cells[0].Value != null && (sender as DataGridView).CurrentRow.Cells[0].Tag == null)
                    {

                        if ((sender as DataGridView).CurrentRow.Cells[0].Value.ToString().Trim() != "")
                        {
                            string searchSKU = (sender as DataGridView).CurrentRow.Cells[0].Value.ToString().Trim();
                            var search = context.Products
                                .Where(x => x.SKU == searchSKU)
                                .FirstOrDefault();
                            if (search != default)
                            {
                                DataGridViewRow current = new DataGridViewRow();
                                DataGridViewCell first = new DataGridViewButtonCell()
                                {
                                    Value = search.SKU,
                                    Tag = null,


                                };
                                DataGridViewCell second = new DataGridViewButtonCell()
                                {
                                    Value = search.Description,
                                    Tag = "Fixed",

                                };

                                current.Cells.Add(first);
                                current.Cells.Add(second);
                                first.ReadOnly = true;
                                second.ReadOnly = true;
                                //current.Cells[0] = first;
                                //current.Cells[1] = second;



                                (sender as DataGridView).Rows.Remove((sender as DataGridView).CurrentRow);
                                (sender as DataGridView).Rows.Add(current);
                                //(sender as DataGridView).SelectedRows[0].Cells = current.Cells;
                            }
                        }
                    }
                }
            }
        }

        private static void OrderDgv_KeyDown(object sender, KeyEventArgs e, int id)
        {
            DataGridView view = sender as DataGridView;
            if (e.KeyCode == Keys.Enter && view != null)
            {
                using (MaxterDBEntities context = new MaxterDBEntities())
                {
                    if (view.CurrentRow != null && (view.CurrentRow.Cells[0].Value != null && view.CurrentRow.Cells[0].Tag == null))
                    {

                        if (view.CurrentRow.Cells[0].Value.ToString().Trim() != "")
                        {
                            string searchSKU = view.CurrentRow.Cells[0].Value.ToString().Trim();

                            var search = context.Products
                                .Where(x => x.SKU == searchSKU)
                                .FirstOrDefault();

                            var searchPrice = context.Prices.Where(x =>
                                    x.SKU == searchSKU && x.Price_Category.ToString() == view.Tag.ToString())
                                .FirstOrDefault();

                            if (search != default)
                            {
                                

                                DataGridViewRow current = new DataGridViewRow();
                                DataGridViewCell first = new DataGridViewTextBoxCell()
                                {
                                    Value = search.SKU,
                                    Tag = "Set",


                                };
                                DataGridViewCell second = new DataGridViewTextBoxCell()
                                {
                                    Value = search.Description,
                                    Tag = "Fixed",
                                    
                                };
                                DataGridViewCell third = new DataGridViewTextBoxCell()
                                {
                                    Value = "0"
                                };
                                DataGridViewCell fourth = new DataGridViewTextBoxCell()
                                {
                                    Value = search.PU,
                                    Tag = "Fixed"
                                };

                                DataGridViewCell fifth = new DataGridViewTextBoxCell();

                                if (searchPrice != default)
                                {
                                    fifth.Value = searchPrice.Price1;
                                }
                                else
                                {
                                    fifth.Value = 0;
                                }
                                

                                Order_detail currentDetail = new Order_detail
                                    (id, search.SKU, decimal.Parse(fifth.Value.ToString()), int.Parse(third.Value.ToString()));

                                first.Tag = currentDetail.Order_Detail_id;

                                var exists = context.Order_detail.FirstOrDefault(x =>
                                    x.Order_Detail_id == currentDetail.Order_Detail_id);

                                if (exists == default)
                                {
                                    DBFunctions.CreateNewRecord(currentDetail);
                                }

                                current.Cells.Add(first);
                                current.Cells.Add(second);
                                current.Cells.Add(third);
                                current.Cells.Add(fourth);
                                current.Cells.Add(fifth);
                                first.ReadOnly = true;
                                second.ReadOnly = false;
                                fourth.ReadOnly = true;
                                //current.Cells[0] = first;
                                //current.Cells[1] = second;



                                view.Rows.Remove(view.CurrentRow);
                                view.Rows.Add(current);
                                e.Handled = true;
                                //(sender as DataGridView).SelectedRows[0].Cells = current.Cells;

                                UpdateStock(search.SKU, 1, 0);
                            }
                            else
                            {
                                view.Rows.Remove(view.CurrentRow);
                                e.Handled = true;
                            }
                        }
                    }
                }
            }

            if (e.KeyCode == Keys.F3 && view != null)
            {
                e.Handled = true;
                if (view.SelectedCells.Count > 0)
                {
                    if (MessageBox.Show(DynamicResources.MenuFunctions_OrderDgv_KeyDown_Delete_selected_item_, DynamicResources.MenuFunctions_RemoveOrder_Click_Delete, MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        try
                        {
                            using (MaxterDBEntities context = new MaxterDBEntities())
                            {
                                if (int.TryParse(view.CurrentRow.Cells[0].Tag.ToString(), out int iD))
                                {
                                    Order_detail found =
                                        context.Order_detail.FirstOrDefault(x => x.Order_Detail_id == iD);

                                    if (found != default)
                                    {
                                        DBFunctions.RemoveRecord(found);
                                    }
                                }

                                if (int.TryParse(view.CurrentRow.Cells[2].Value.ToString(), out int qty))
                                {
                                    UpdateStock(view.CurrentRow.Cells[0].Value.ToString(), 0, qty);
                                }
                                else
                                {
                                    UpdateStock(view.CurrentRow.Cells[0].Value.ToString(), 0, 0);
                                }

                                
                                view.Rows.Remove(view.CurrentRow);
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

        private static void ProductMenu(object sender, EventArgs e)
        {
            //Application.OpenForms[0].SizeChanged += Product_Click;
            ListView lv = new ListView()
            {
                Top = Application.OpenForms[0].Height / 15,
                Left = Application.OpenForms[0].Width / 25,
                Width = Application.OpenForms[0].Width - (Application.OpenForms[0].Width / 25) * 2,
                Height = Application.OpenForms[0].Height / 3,
                FullRowSelect = true,
                View = View.Details,
                HideSelection = false,
                //Anchor = (AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Left)
            };
            Application.OpenForms[0].Controls.Add(lv);
            
                lv.Columns.Add(DynamicResources.MenuFunctions_ProductMenu_SKU);
                lv.Columns.Add(DynamicResources.MenuFunctions_ProductMenu_Desc);
                lv.Columns.Add(DynamicResources.MenuFunctions_ProductMenu_CTN);
                lv.Columns.Add(DynamicResources.MenuFunctions_ProductMenu_Stock);
                lv.Columns.Add(DynamicResources.MenuFunctions_ProductMenu_Min);
                lv.Columns.Add(DynamicResources.MenuFunctions_ProductMenu_EanCount);
                lv.Columns.Add(DynamicResources.MenuFunctions_ProductMenu_OrdersCount);
                lv.Columns.Add(DynamicResources.MenuFunctions_ProductMenu_PricesCount);
                lv.Columns.Add(DynamicResources.MenuFunctions_ProductMenu_Unit);

                for (int i = 0; i < lv.Columns.Count; i++)
            {
                lv.Columns[i].Width = lv.Width / lv.Columns.Count;
            }

            GroupBox propertiesGb = new GroupBox()
            {
                Text = DynamicResources.MenuFunctions_ProductMenu_Properties,
                Top = lv.Bottom + Application.OpenForms[0].Height / 15,
                Left = Application.OpenForms[0].Width / 25,
                Height = (int) (Application.OpenForms[0].Height / 2.5),
                //Width = Application.OpenForms[0].Width - (Application.OpenForms[0].Width / 25) * 2
                Width = (lv.Width / 6) * 4,
                AutoSize = true,
                //Anchor = (AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Left)
            };
            Application.OpenForms[0].Controls.Add(propertiesGb);
            Label eansLabel = new Label()
            {
                Parent = propertiesGb,
                Text = DynamicResources.MenuFunctions_ProductMenu_EANs,
                Left = propertiesGb.Width / 30,
                Top = propertiesGb.Height / 10,
                AutoSize = true,
                //Anchor = (AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Left)
            };
            ListView eansLv = new ListView()
            {
                Parent = propertiesGb,
                Top = propertiesGb.Height / 6,
                Left = propertiesGb.Width / 30,
                Height = (int) (propertiesGb.Height * 0.65),
                Width = propertiesGb.Width / 4,
                View = View.Details,
                Enabled = false,
                FullRowSelect = true
                //AutoSize = true,
            };
            eansLv.Columns.Add(DynamicResources.MenuFunctions_ProductMenu_ID);
            eansLv.Columns.Add(DynamicResources.MenuFunctions_ProductMenu_EAN);
            eansLv.Columns.Add(DynamicResources.MenuFunctions_ProductMenu_Qty);
            for (int i = 0; i < eansLv.Columns.Count; i++)
            {
                eansLv.Columns[i].Width = eansLv.Width / eansLv.Columns.Count;
            }

            CheckBox showEans = new CheckBox()
            {
                Parent = propertiesGb,
                Top = eansLv.Bottom + propertiesGb.Height / 25,
                Left = eansLv.Left,
                Text = DynamicResources.MenuFunctions_ProductMenu_Show_EANs,
                AutoSize = true,
                //Anchor = (AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Left)
            };
            Label pricesLabel = new Label()
            {
                Parent = propertiesGb,
                Text = DynamicResources.MenuFunctions_ProductMenu_Prices,
                Left = (propertiesGb.Width / 30) * 2 + eansLv.Width,
                Top = propertiesGb.Height / 10,
                AutoSize = true,
                //Anchor = (AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Left)
            };
            ListView pricesLv = new ListView()
            {
                Parent = propertiesGb,
                Top = propertiesGb.Height / 6,
                Left = (propertiesGb.Width / 30) * 2 + eansLv.Width,
                Height = (int) (propertiesGb.Height * 0.65),
                Width = propertiesGb.Width / 4,
                View = View.Details,
                Enabled = false,
                FullRowSelect = true
                //AutoSize = true,
            };
            pricesLv.Columns.Add(DynamicResources.MenuFunctions_ProductMenu_ID);
            pricesLv.Columns.Add("HUF");
            pricesLv.Columns.Add(DynamicResources.MenuFunctions_ProductMenu_Category);
            for (int i = 0; i < pricesLv.Columns.Count; i++)
            {
                pricesLv.Columns[i].Width = pricesLv.Width / pricesLv.Columns.Count;
            }

            CheckBox showPrices = new CheckBox()
            {
                Parent = propertiesGb,
                Top = eansLv.Bottom + propertiesGb.Height / 25,
                Left = eansLv.Left + showEans.Width,
                Text = DynamicResources.MenuFunctions_ProductMenu_Show_Prices,
                AutoSize = true,
                //Anchor = (AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Left)
            };
            Label orderDetailLabel = new Label()
            {
                Parent = propertiesGb,
                Text = DynamicResources.MenuFunctions_ProductMenu_Orders_including_item,
                Left = (propertiesGb.Width / 30) * 3 + (eansLv.Width * 2),
                Top = propertiesGb.Height / 10,
                AutoSize = true,
                //Anchor = (AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Left)
            };
            ListView orderDetailLv = new ListView()
            {
                Parent = propertiesGb,
                Top = propertiesGb.Height / 6,
                Left = (propertiesGb.Width / 30) * 3 + (eansLv.Width * 2),
                Height = (int) (propertiesGb.Height * 0.65),
                Width = propertiesGb.Width / 4,
                View = View.Details,
                Enabled = false,
                FullRowSelect = true
                //AutoSize = true,
                //Anchor = (AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Left)
            };
            orderDetailLv.Columns.Add(DynamicResources.MenuFunctions_ProductMenu_Number);
            orderDetailLv.Columns.Add(DynamicResources.MenuFunctions_ProductMenu_Qty);
            for (int i = 0; i < orderDetailLv.Columns.Count; i++)
            {
                orderDetailLv.Columns[i].Width = orderDetailLv.Width / orderDetailLv.Columns.Count;
            }

            CheckBox showOrderDetail = new CheckBox()
            {
                Parent = propertiesGb,
                Top = eansLv.Bottom + propertiesGb.Height / 25,
                Left = eansLv.Left + showEans.Width + showPrices.Width,
                Text = DynamicResources.MenuFunctions_ProductMenu_Show_orders_including_item,
                AutoSize = true,
                //Anchor = (AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Left)
            };
            if (Application.OpenForms[0].Tag.ToString()[0] == 'F')
            {
                Button createRecord = new Button()
                {
                    Top = propertiesGb.Top + eansLv.Top,
                    //Left = propertiesGb.Width / 30 + orderDetailLv.Right,
                    Left = propertiesGb.Right + propertiesGb.Width / 30,
                    Text = DynamicResources.MenuFunctions_ProductMenu_Create_new_record,

                    //AutoSize = true
                };
                createRecord.Width *= 2;
                createRecord.Height *= 2;
                //propertiesGb.Controls.Add(createRecord);
                Application.OpenForms[0].Controls.Add(createRecord);
                createRecord.Click += CreateRecord_Click;
                Button createEan = new Button()
                {
                    Top = createRecord.Bottom + (int) (createRecord.Height * 0.25),
                    //Left = propertiesGb.Width / 30 + orderDetailLv.Right,
                    Left = createRecord.Left,
                    Text = DynamicResources.MenuFunctions_ProductMenu_Add_EAN,
                    Width = createRecord.Width,
                    Height = createRecord.Height,
                };
                //propertiesGb.Controls.Add(createEan);
                Application.OpenForms[0].Controls.Add(createEan);
                //createEAN.Click += CreateEAN_Click;
                createEan.Click += delegate(object sender2, EventArgs e2)
                {
                    if (lv.SelectedIndices.Count > 0)
                    {
                        CreateEAN_Click(sender2, e2, lv.Tag.ToString());
                    }
                };
                Button createPrice = new Button()
                {
                    Top = createEan.Bottom + (int) (createRecord.Height * 0.25),
                    //Left = propertiesGb.Width / 30 + orderDetailLv.Right,
                    Left = createRecord.Left,
                    Text = DynamicResources.MenuFunctions_ProductMenu_Add_Price,
                    Width = createRecord.Width,
                    Height = createRecord.Height,
                };
                //propertiesGb.Controls.Add(createPrice);
                Application.OpenForms[0].Controls.Add(createPrice);
                //createPrice.Click += delegate { CreatePrice_Click(sender, e, lv.Tag.ToString()); };

                createPrice.Click += delegate(object sender3, EventArgs e3)
                {
                    if (lv.SelectedIndices.Count > 0)
                    {
                        CreatePrice_Click(sender, e, lv.Tag.ToString());
                    }
                };

                Button modifyRecord = new Button()
                {
                    Top = propertiesGb.Top + eansLv.Top,
                    //Left = propertiesGb.Width / 30 + orderDetailLv.Right,
                    Left = (int) (createRecord.Left + createRecord.Width * 1.05),
                    Text = DynamicResources.MenuFunctions_ProductMenu_Modify_record,
                    Width = createRecord.Width,
                    Height = createRecord.Height,
                };
                Application.OpenForms[0].Controls.Add(modifyRecord);
                modifyRecord.Click += delegate { ModifyRecord_Click(sender, e, lv); };
                Button modifyEan = new Button()
                {
                    Top = createRecord.Bottom + (int) (createRecord.Height * 0.25),
                    //Left = propertiesGb.Width / 30 + orderDetailLv.Right,
                    Left = modifyRecord.Left,
                    Text = DynamicResources.MenuFunctions_ProductMenu_Modify_EAN,
                    Width = createRecord.Width,
                    Height = createRecord.Height,
                };
                Application.OpenForms[0].Controls.Add(modifyEan);
                modifyEan.Click += delegate { ModifyEan_Click(sender, e, eansLv); };
                Button modifyPrice = new Button()
                {
                    Top = createEan.Bottom + (int) (createRecord.Height * 0.25),
                    //Left = propertiesGb.Width / 30 + orderDetailLv.Right,
                    Left = modifyRecord.Left,
                    Text = DynamicResources.MenuFunctions_ProductMenu_Modify_Price,
                    Width = createRecord.Width,
                    Height = createRecord.Height,
                };
                Application.OpenForms[0].Controls.Add(modifyPrice);
                modifyPrice.Click += delegate { ModifyPrice_Click(sender, e, pricesLv); };
                Button removeRecord = new Button()
                {
                    Top = createRecord.Top,
                    Left = (int) (modifyRecord.Left + createRecord.Width * 1.05),
                    Text = DynamicResources.MenuFunctions_ProductMenu_Remove_record,
                    Width = modifyRecord.Width,
                    Height = modifyRecord.Height
                };
                Application.OpenForms[0].Controls.Add(removeRecord);
                removeRecord.Click += delegate { RemoveRecord_Click(sender, e, lv); };
                Button removeEAN = new Button()
                {
                    Top = removeRecord.Bottom + (int) (createRecord.Height * 0.25),
                    //Left = propertiesGb.Width / 30 + orderDetailLv.Right,
                    Left = removeRecord.Left,
                    Text = DynamicResources.MenuFunctions_ProductMenu_Remove_EAN,
                    Width = createRecord.Width,
                    Height = createRecord.Height,
                };
                Application.OpenForms[0].Controls.Add(removeEAN);
                removeEAN.Click += delegate { RemoveEAN_Click(sender, e, eansLv); };
                Button removePrice = new Button()
                {
                    Top = removeEAN.Bottom + (int) (createRecord.Height * 0.25),
                    //Left = propertiesGb.Width / 30 + orderDetailLv.Right,
                    Left = removeRecord.Left,
                    Text = DynamicResources.MenuFunctions_ProductMenu_Remove_Price,
                    Width = createRecord.Width,
                    Height = createRecord.Height,
                };
                Application.OpenForms[0].Controls.Add(removePrice);
                removePrice.Click += delegate { RemovePrice_Click(sender, e, pricesLv); };
            }

            Button close = new Button()
            {
                //Top = propertiesGB.Bottom + Application.OpenForms[0].Height / 25,
                //Left = Application.OpenForms[0].Width - Application.OpenForms[0].Width / 10,
                //Top = propertiesGb.Bottom,
                Top = (Application.OpenForms[0].Height / 16) * 14,
                Left = lv.Right,
                Text = DynamicResources.MenuFunctions_Billing_Click_Close,
                AutoSize = true
            };
            close.Top -= close.Height;
            close.Left -= close.Width;
            Application.OpenForms[0].Controls.Add(close);
            close.Click += Close_Click;
            propertiesGb.Controls.Add(eansLabel);
            propertiesGb.Controls.Add(eansLv);
            propertiesGb.Controls.Add(showEans);
            propertiesGb.Controls.Add(pricesLabel);
            propertiesGb.Controls.Add(pricesLv);
            propertiesGb.Controls.Add(showPrices);
            propertiesGb.Controls.Add(orderDetailLabel);
            propertiesGb.Controls.Add(orderDetailLv);
            propertiesGb.Controls.Add(showOrderDetail);
            using (MaxterDBEntities context = new MaxterDBEntities())
            {
                foreach (var item in context.Products)
                {
                    ListViewItem current = new ListViewItem(item.SKU);
                    current.SubItems.Add(item.Description);
                    current.SubItems.Add(item.VTSZ);
                    current.SubItems.Add(item.Stock_Quantity.ToString());
                    current.SubItems.Add(item.Min_Stock_Quantity.ToString());
                    current.SubItems.Add(item.EANs.Count().ToString());
                    current.SubItems.Add(item.Order_detail.Count().ToString());
                    current.SubItems.Add(item.Prices.Count().ToString());
                    current.SubItems.Add(item.PU);
                    lv.Items.Add(current);
                }
            }

            showEans.CheckedChanged += delegate(object sender2, EventArgs e2)
            {
                Checked_Changed(sender, e, showEans, eansLv, showPrices, pricesLv, showOrderDetail, orderDetailLv);
            };
            showPrices.CheckedChanged += delegate(object sender2, EventArgs e2)
            {
                Checked_Changed(sender, e, showEans, eansLv, showPrices, pricesLv, showOrderDetail, orderDetailLv);
            };
            showOrderDetail.CheckedChanged += delegate(object sender2, EventArgs e2)
            {
                Checked_Changed(sender, e, showEans, eansLv, showPrices, pricesLv, showOrderDetail, orderDetailLv);
            };
            lv.SelectedIndexChanged += delegate(object sender2, EventArgs e2)
            {
                Selected_Index_Changed(sender, e, lv, eansLv, showEans, pricesLv, showPrices, orderDetailLv,
                    showOrderDetail);
            };
        }

        private static void PartnerMenu(object sender, EventArgs e)
        {
            ListView lv = new ListView()
            {
                Top = Application.OpenForms[0].Height / 15,
                Left = Application.OpenForms[0].Width / 25,
                Width = Application.OpenForms[0].Width - (Application.OpenForms[0].Width / 25) * 2,
                Height = Application.OpenForms[0].Height / 3,
                FullRowSelect = true,
                View = View.Details,
                HideSelection = false,
            };

            lv.SelectedIndexChanged += delegate (object sender2, EventArgs e2)
            {
                Selected_Index_Changed_Partner(sender, e, lv);
            };

            Application.OpenForms[0].Controls.Add(lv);
            /*foreach (var item in typeof(Partner).GetProperties())
            {
                lv.Columns.Add(item.Name);
            }*/

            lv.Columns.Add(DynamicResources.MenuFunctions_PartnerMenu_TIN);
            lv.Columns.Add(DynamicResources.MenuFunctions_PartnerMenu_PartnerId);
            lv.Columns.Add(DynamicResources.MenuFunctions_PartnerMenu_Name);
            lv.Columns.Add(DynamicResources.MenuFunctions_PartnerMenu_InvoiceAddress);
            lv.Columns.Add(DynamicResources.MenuFunctions_PartnerMenu_ShippingAddress);
            lv.Columns.Add(DynamicResources.MenuFunctions_PartnerMenu_PriceCategory);
            lv.Columns.Add(DynamicResources.MenuFunctions_PartnerMenu_Email);
            lv.Columns.Add(DynamicResources.MenuFunctions_PartnerMenu_Telephone);
            lv.Columns.Add(DynamicResources.MenuFunctions_PartnerMenu_Orders);

            for (int i = 0; i < lv.Columns.Count; i++)
            {
                lv.Columns[i].Width = lv.Width / lv.Columns.Count;
            }


            if (Application.OpenForms[0].Tag.ToString()[0] == 'F')
            {
                Button removePartner = new Button()
                {
                    Text = DynamicResources.MenuFunctions_PartnerMenu_Remove_partner,
                    Top = lv.Bottom + Application.OpenForms[0].Height / 15,
                    AutoSize = true
                };
                removePartner.Width *= 2;
                removePartner.Height *= 2;
                removePartner.Left = lv.Right - removePartner.Width;
                Application.OpenForms[0].Controls.Add(removePartner);
                removePartner.Click += delegate { RemovePartner_Click(sender, e, lv); };

                Button modifyPartner = new Button()
                {
                    Text = DynamicResources.MenuFunctions_PartnerMenu_Modify_partner,
                    Top = removePartner.Top,
                    Width = removePartner.Width,
                    Height = removePartner.Height
                };
                modifyPartner.Left = (int)(removePartner.Left - modifyPartner.Width * 1.05);
                Application.OpenForms[0].Controls.Add(modifyPartner);
                modifyPartner.Click += delegate { ModifyPartner_Click(sender, e, lv); };

                Button createPartner = new Button()
                {
                    Text = DynamicResources.MenuFunctions_PartnerMenu_Create_partner,
                    Top = removePartner.Top,
                    Width = removePartner.Width,
                    Height = removePartner.Height
                };
                createPartner.Left = (int)(modifyPartner.Left - createPartner.Width * 1.05);
                Application.OpenForms[0].Controls.Add(createPartner);
                createPartner.Click += CreatePartner_Click;
            }

            Button close = new Button()
            {

                Top = (Application.OpenForms[0].Height / 16) * 14,
                Left = lv.Right,
                Text = DynamicResources.MenuFunctions_Billing_Click_Close,
                AutoSize = true
            };
            close.Top -= close.Height;
            close.Left -= close.Width;
            Application.OpenForms[0].Controls.Add(close);
            close.Click += Close_Click;

            using (MaxterDBEntities context = new MaxterDBEntities())
            {
                foreach (var item in context.Partners)
                {
                    //var invoice = context.Addresses.FirstOrDefault(x => x.Address_ID == item.Invoice_Address);
                    //var shipping = context.Addresses.FirstOrDefault(x => x.Address_ID == item.Shipping_Address);

                    ListViewItem current = new ListViewItem();
                    current.Text = item.TIN;
                    //current.SubItems.Add(item.TIN);
                    current.SubItems.Add(item.Partner_id.ToString());
                    current.SubItems.Add(item.Name);
                    current.SubItems.Add(item.Invoice_Address.Replace(';', ' '));
                    current.SubItems.Add(item.Shipping_Address.Replace(';', ' '));
                    current.SubItems.Add(item.Price_Category.ToString());
                    current.SubItems.Add(item.Email);
                    current.SubItems.Add(item.Telephone);
                    current.SubItems.Add(item.Orders.Count.ToString());
                    lv.Items.Add(current);
                }
            }
        }

        private static void RemovePrice(object sender, EventArgs e, ListView pricesLv)
        {
            if (pricesLv.SelectedItems.Count > 0)
            {
                int id = int.Parse(pricesLv.SelectedItems[0].SubItems[0].Text);
                using (MaxterDBEntities context = new MaxterDBEntities())
                {
                    Price found = context.Prices.Where(x => x.Price_id == id).FirstOrDefault();
                    if (found != null)
                    {
                        if (MessageBox.Show(DynamicResources.MenuFunctions_RemovePrice_Are_you_sure_you_want_to_delete_the_selected_Price_, DynamicResources.MenuFunctions_RemoveOrder_Click_Delete,
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            DBFunctions.RemoveRecord(found);
                            ProductMenu(sender, e);
                        }
                    }
                }
            }
        }

        private static void RemoveEAN(object sender, EventArgs e, ListView eansLv)
        {
            if (eansLv.SelectedItems.Count > 0)
            {
                int id = int.Parse(eansLv.SelectedItems[0].SubItems[0].Text);
                using (MaxterDBEntities context = new MaxterDBEntities())
                {
                    EAN found = context.EANs.FirstOrDefault(x => x.EAN_id == id);
                    if (found != default)
                    {
                        if (MessageBox.Show(DynamicResources.MenuFunctions_RemoveEAN_Are_you_sure_you_want_to_delete_the_selected_EAN_, DynamicResources.MenuFunctions_RemoveOrder_Click_Delete,
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            DBFunctions.RemoveRecord(found);
                            ProductMenu(sender, e);
                        }
                    }
                }
            }
        }

        private static void RemoveRecord(object sender, EventArgs e, ListView lv)
        {
            if (lv.SelectedItems.Count > 0)
            {
                string sku = lv.SelectedItems[0].SubItems[0].Text;
                using (MaxterDBEntities context = new MaxterDBEntities())
                {
                    Product found = context.Products.Where(x => x.SKU == sku).FirstOrDefault();
                    if (found != null)
                    {
                        if (MessageBox.Show(DynamicResources.MenuFunctions_RemoveRecord_Are_you_sure_you_want_to_delete_the_selected_Record_, DynamicResources.MenuFunctions_RemoveOrder_Click_Delete,
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            DBFunctions.RemoveRecord(found);
                            ProductMenu(sender, e);
                        }
                    }
                }
            }
        }

        private static void ModifyPrice(object sender, EventArgs e, ListView pricesLv)
        {
            if (pricesLv.SelectedItems.Count > 0)
            {
                int id = int.Parse(pricesLv.SelectedItems[0].SubItems[0].Text);
                PriceFrm dialog = new PriceFrm(id);
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    ProductMenu(sender, e);
                }
            }
        }

        private static void ModifyRecord(object sender, EventArgs e, ListView lv)
        {
            if (lv.SelectedItems.Count > 0)
            {
                string sku = lv.SelectedItems[0].SubItems[0].Text;
                NewProductFrm dialog = new NewProductFrm(sku);
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    ProductMenu(sender, e);
                }
            }
        }

        private static void ModifyEan(object sender, EventArgs e, ListView eansLv)
        {
            if (eansLv.SelectedItems.Count > 0)
            {
                int id = int.Parse(eansLv.SelectedItems[0].SubItems[0].Text);
                EanFrm dialog = new EanFrm(id);
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    ProductMenu(sender, e);
                }
            }
        }

        private static void CreatePrice(object sender, EventArgs e, string sku)
        {
            PriceFrm dialog = new PriceFrm(sku);
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                ProductMenu(sender, e);
            }
        }

        private static void CreateEan(object sender, EventArgs e, string sku)
        {
            EanFrm dialog = new EanFrm(sku);
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                ProductMenu(sender, e);
            }
        }

        private static void CreateRecord(object sender, EventArgs e)
        {
            NewProductFrm dialog = new NewProductFrm();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                ProductMenu(sender, e);
            }
        }

        private static void FillListViews(ListView lvProducts, ListView eansLv, CheckBox showEans, ListView pricesLv,
            CheckBox showPrices, ListView orderDetailLv, CheckBox showOrderDetail)
        {
            if (lvProducts.SelectedItems.Count > 0)
            {
                lvProducts.Tag = lvProducts.SelectedItems[0].SubItems[0].Text;
            }

            if (showEans.Checked && lvProducts.SelectedItems.Count > 0)
            {
                eansLv.Items.Clear();
                eansLv.Enabled = true;
                DBFunctions.FillEansListView(eansLv, lvProducts.SelectedItems[0].Text);
            }

            if (!showEans.Checked)
            {
                eansLv.Items.Clear();
                eansLv.Enabled = false;
            }

            if (showPrices.Checked && lvProducts.SelectedItems.Count > 0)
            {
                pricesLv.Items.Clear();
                pricesLv.Enabled = true;
                DBFunctions.FillPricesListView(pricesLv, lvProducts.SelectedItems[0].Text);
            }

            if (!showPrices.Checked)
            {
                pricesLv.Items.Clear();
                pricesLv.Enabled = false;
            }

            if (showOrderDetail.Checked && lvProducts.SelectedItems.Count > 0)
            {
                orderDetailLv.Items.Clear();
                orderDetailLv.Enabled = true;
                DBFunctions.FillOrder_detailListView(orderDetailLv, lvProducts.SelectedItems[0].Text);
            }

            if (!showOrderDetail.Checked)
            {
                orderDetailLv.Items.Clear();
                orderDetailLv.Enabled = false;
            }
        }

        #endregion


        #region Events

        private static void Partners_Click(object sender, EventArgs e)
        {
            FormCleanUp();
            PartnerMenu(sender, e);
        }

        private static void RemovePartner_Click(object sender, EventArgs e, ListView lv)
        {
            if (lv.SelectedItems.Count > 0)
            {
                string id = lv.SelectedItems[0].SubItems[1].Text;
                using (MaxterDBEntities context = new MaxterDBEntities())
                {
                    Partner found = context.Partners.Where(x => x.Partner_id.ToString() == id).FirstOrDefault();
                    if (found != null)
                    {
                        if (MessageBox.Show(DynamicResources.MenuFunctions_RemovePartner_Click_Are_you_sure_you_want_to_delete_the_selected_Partner_, DynamicResources.MenuFunctions_RemoveOrder_Click_Delete,
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            DBFunctions.RemoveRecord(found);
                            PartnerMenu(sender, e);
                        }
                    }
                }
            }
        }

        private static void ModifyPartner_Click(object sender, EventArgs e, ListView lv)
        {
            if (lv.SelectedItems.Count > 0)
            {
                string id = lv.SelectedItems[0].SubItems[1].Text;
                PartnerFrm dialog = new PartnerFrm(id);
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    PartnerMenu(sender, e);
                }
            }
        }

        private static void Selected_Index_Changed_Partner(object sender, EventArgs eventArgs, ListView lv)
        {
            if (lv.SelectedItems.Count > 0)
            {
                lv.Tag = lv.SelectedItems[0].SubItems[1].Text;
            }
        }

        private static void CreatePartner_Click(object sender, EventArgs e)
        {
            PartnerFrm dialog = new PartnerFrm();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                
            }
        }

        private static void
            Product_Click(object sender,
                EventArgs e) //Creates specific controls for working with Products in the database
        {
            FormCleanUp();
            ProductMenu(sender, e);
        }

        private static void RemovePrice_Click(object sender, EventArgs e, ListView pricesLv)
        {
            RemovePrice(sender, e, pricesLv);
        }

        private static void RemoveEAN_Click(object sender, EventArgs e, ListView eansLv)
        {
            RemoveEAN(sender, e, eansLv);
        }

        private static void RemoveRecord_Click(object sender, EventArgs e, ListView lv)
        {
            RemoveRecord(sender, e, lv);
        }

        private static void ModifyPrice_Click(object sender, EventArgs e, ListView pricesLv)
        {
            ModifyPrice(sender, e, pricesLv);
        }

        private static void ModifyRecord_Click(object sender, EventArgs e, ListView lv)
        {
            ModifyRecord(sender, e, lv);
        }

        private static void ModifyEan_Click(object sender, EventArgs e, ListView eansLv)
        {
            ModifyEan(sender, e, eansLv);
        }

        private static void CreatePrice_Click(object sender, EventArgs e, string sku)
        {
            CreatePrice(sender, e, sku);
        }

        private static void CreateEAN_Click(object sender, EventArgs e, string sku)
        {
            CreateEan(sender, e, sku);
        }

        private static void CreateRecord_Click(object sender, EventArgs e)
        {
            CreateRecord(sender, e);
        }

        private static void Close_Click(object sender, EventArgs e)
        {
            FormCleanUp();
        }

        private static void Checked_Changed(object sender, EventArgs e, CheckBox showEans, ListView eansLb,
            CheckBox showPrices, ListView pricesLv, CheckBox showOrderDetail, ListView orderDetailLb)
        {
            if (!showEans.Checked)
            {
                eansLb.Items.Clear();
                eansLb.Enabled = false;
            }

            if (!showPrices.Checked)
            {
                pricesLv.Items.Clear();
                pricesLv.Enabled = false;
            }

            if (!showOrderDetail.Checked)
            {
                orderDetailLb.Items.Clear();
                orderDetailLb.Enabled = false;
            }
        } //Empties listBox if unchecked

        private static void Selected_Index_Changed(object sender, EventArgs e, ListView lvProducts, ListView eansLv,
            CheckBox showEans, ListView pricesLv, CheckBox showPrices, ListView orderDetailLv,
            CheckBox showOrderDetail) //Fills up the listBoxes based on checkBoxes states
        {
            FillListViews(lvProducts, eansLv, showEans, pricesLv, showPrices, orderDetailLv, showOrderDetail);
        }

        private static void Language_Click(object sender, EventArgs e) //Open language change form
        {
            LanguageFrm dialog = new LanguageFrm();
            dialog.ShowDialog();
        }

        private static void AdminSettings_Click(object sender, EventArgs e)
        {
            AdminSettingsFrm dialog = new AdminSettingsFrm();
            dialog.ShowDialog();
        }

        private static void Settings_Click(object sender, EventArgs e)
        {
            ChangePasswordFrm dialog = new ChangePasswordFrm((Application.OpenForms[0] as MenuFrm).id);

            dialog.ShowDialog();
        }

        private static void Exit_Click(object sender, EventArgs e)
        {
            Application.OpenForms[0].Close();
        } //Exits program

        private static void Billing_Click(object sender, EventArgs e)
        {
            if (ApiFunctions.LoadKey())
            {
                FormCleanUp();

                //string orderId = Interaction.InputBox("Please enter order ID", "Billing");

                ListView lv = new ListView()
                {
                    Top = Application.OpenForms[0].Height / 15,
                    Left = Application.OpenForms[0].Width / 25,
                    Width = Application.OpenForms[0].Width - (Application.OpenForms[0].Width / 25) * 2,
                    Height = Application.OpenForms[0].Height / 3,
                    FullRowSelect = true,
                    View = View.Details,
                    HideSelection = false
                };
                Application.OpenForms[0].Controls.Add(lv);

                lv.Columns.Add(DynamicResources.MenuFunctions_Billing_Click_Invoice_Id);
                lv.Columns.Add(DynamicResources.MenuFunctions_Billing_Click_Date);
                lv.Columns.Add(DynamicResources.MenuFunctions_Billing_Click_Client);

                for (int i = 0; i < lv.Columns.Count; i++)
                {
                    lv.Columns[i].Width = lv.Width / lv.Columns.Count;
                }

                Button downloadInvoices = new Button()
                {
                    Text = DynamicResources.MenuFunctions_Billing_Click_Download_invoices,
                    Top = lv.Bottom + Application.OpenForms[0].Height / 15,
                    AutoSize = true
                };
                downloadInvoices.Width *= 2;
                downloadInvoices.Height *= 2;
                downloadInvoices.Left = lv.Right - downloadInvoices.Width;
                Application.OpenForms[0].Controls.Add(downloadInvoices);
                downloadInvoices.Click += DownloadInvoices_Click;

                Button loadInvoices = new Button()
                {
                    Text = DynamicResources.MenuFunctions_Billing_Click_Load_invoices,
                    Top = downloadInvoices.Top,
                    Width = downloadInvoices.Width,
                    Height = downloadInvoices.Height
                };
                loadInvoices.Left = (int)(downloadInvoices.Left - loadInvoices.Width * 1.05);
                Application.OpenForms[0].Controls.Add(loadInvoices);
                loadInvoices.Click += delegate { LoadInvoices_Click(sender, e, lv); };

                Button createInvoice = new Button()
                {
                    Text = DynamicResources.MenuFunctions_Billing_Click_Create_invoice,
                    Top = loadInvoices.Top,
                    Width = loadInvoices.Width,
                    Height = loadInvoices.Height
                };
                createInvoice.Left = (int)(loadInvoices.Left - createInvoice.Width * 1.05);
                Application.OpenForms[0].Controls.Add(createInvoice);
                createInvoice.Click += CreateInvoice_Click;


                Button close = new Button()
                {
                    //Top = propertiesGB.Bottom + Application.OpenForms[0].Height / 25,
                    //Left = Application.OpenForms[0].Width - Application.OpenForms[0].Width / 10,
                    //Top = propertiesGb.Bottom,
                    Top = (Application.OpenForms[0].Height / 16) * 14,
                    Left = lv.Right,
                    Text = DynamicResources.MenuFunctions_Billing_Click_Close,
                    AutoSize = true
                };
                close.Top -= close.Height;
                close.Left -= close.Width;
                Application.OpenForms[0].Controls.Add(close);
                close.Click += Close_Click;
                
            }
            else
            {
                MessageBox.Show("API connection not set up. Billing is disabled. Please contact an admin.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        private static void CreateInvoice_Click(object sender, EventArgs e)
        {
            OrderSelectFrm dialog = new OrderSelectFrm();
            //InvoiceFrm dialog = new InvoiceFrm();

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                int foundOrder = dialog.OrderId;

                InvoiceFrm invoiceDialog = new InvoiceFrm(foundOrder);

                invoiceDialog.ShowDialog();
            }


        }

        private static void LoadInvoices_Click(object sender, EventArgs e, ListView lv)
        {
            try
            {
                if (!File.Exists("invoicesJson.txt"))
                {
                    MessageBox.Show(DynamicResources.MenuFunctions_LoadInvoices_Click_Invoices_are_not_downloaded__Please_download_invoices_first_, DynamicResources.MenuFunctions_LoadInvoices_Click_Warning,
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    string invoicesLoaded = File.ReadAllText("invoicesJson.txt");

                    //var parsedObject = JObject.Parse(invoicesLoaded);

                    //var parsedObject = JObject.Parse(invoicesLoaded)["Data"].Cast<JProperty>();
                    //var result = parsedObject.SelectMany(ts => (ts.Value as JObject).Properties(), (obj, props) => new { Date = obj.Name, Position = props.Name, props.Value });

                    //List<string> list = new List<string>();

                    //IList<int> list = parsedObject["data"].SelectToken("id").Select(s => (int)s).ToList();

                    var idValues = JObject.Parse(invoicesLoaded)["data"].Select(p => p["id"].Value<string>()).ToList();
                    var dateValues = JObject.Parse(invoicesLoaded)["data"].Children()["attributes"].Select(p => p["date"].Value<string>()).ToList();
                    var clientValues = JObject.Parse(invoicesLoaded)["data"].Children()["attributes"]["client"].Select(p => p["name"].Value<string>()).ToList();

                    List<TempInvoice> loaded = new List<TempInvoice>();


                    for (int i = 0; i < idValues.Count; i++)
                    {
                        loaded.Add(new TempInvoice(idValues[i], dateValues[i], clientValues[i]));
                        //TempInvoice debug = new TempInvoice(idValues[i], dateValues[i], clientValues[i]);
                        //result += debug.ToString() + "\n";
                    }

                    foreach (TempInvoice item in loaded)
                    {
                        ListViewItem current = new ListViewItem(item.Id);
                        current.SubItems.Add(item.Date);
                        current.SubItems.Add(item.Client);

                        lv.Items.Add(current);
                    }

                    //MessageBox.Show(result, "result", MessageBoxButtons.OK);

                    //var idJson = parsedObject["data"]["id"].ToString();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }

        private static void DownloadInvoices_Click(object sender, EventArgs e)
        {
            var result = ApiFunctions.GetInvoiceAsync();
        }

        private static void Orders_Click(object sender, EventArgs e)
        {
            FormCleanUp();

            ListView lv = new ListView()
            {
                Top = Application.OpenForms[0].Height / 15,
                Left = Application.OpenForms[0].Width / 25,
                Width = Application.OpenForms[0].Width - (Application.OpenForms[0].Width / 25) * 2,
                Height = Application.OpenForms[0].Height / 3,
                FullRowSelect = true,
                View = View.Details,
                HideSelection = false
            };

            /*foreach (var item in typeof(Order).GetProperties())
            {
                lv.Columns.Add(item.Name);
            }*/

            lv.Columns.Add(DynamicResources.MenuFunctions_OrderMenu_OrderID);
            lv.Columns.Add(DynamicResources.MenuFunctions_OrderMenu_PartnerID);
            lv.Columns.Add(DynamicResources.MenuFunctions_OrderMenu_OderDate);
            lv.Columns.Add(DynamicResources.MenuFunctions_OrderMenu_ReqDate);
            lv.Columns.Add(DynamicResources.MenuFunctions_OrderMenu_ShipDate);
            lv.Columns.Add(DynamicResources.MenuFunctions_OrderMenu_OrderDetail);
            lv.Columns.Add(DynamicResources.MenuFunctions_OrderMenu_Partner);

            for (int i = 0; i < lv.Columns.Count; i++)
            {
                lv.Columns[i].Width = lv.Width / lv.Columns.Count;
            }

            using (MaxterDBEntities context = new MaxterDBEntities())
            {
                foreach (var item in context.Orders)
                {
                    ListViewItem current = new ListViewItem(item.Order_id.ToString());
                    current.SubItems.Add(item.Partner_id.ToString());
                    current.SubItems.Add(item.Order_date.ToString());
                    current.SubItems.Add(item.Required_date.ToString());
                    if (item.Shipped_date != default)
                    {
                        current.SubItems.Add(item.Shipped_date.ToString());
                    }
                    else
                    {
                        current.SubItems.Add(DynamicResources.MenuFunctions_OrderMenu_NotShipped);
                    }

                    current.SubItems.Add(item.Order_detail.Count.ToString());
                    current.SubItems.Add(item.Partner.Name);

                    lv.Items.Add(current);
                }
            }

            Application.OpenForms[0].Controls.Add(lv);

            if (Application.OpenForms[0].Tag.ToString()[0] == 'F')
            {
                Button removeOrder = new Button()
                {
                    Text = DynamicResources.MenuFunctions_Orders_Click_Remove_order,
                    Top = lv.Bottom + Application.OpenForms[0].Height / 15,
                    AutoSize = true
                };
                removeOrder.Width *= 2;
                removeOrder.Height *= 2;
                removeOrder.Left = lv.Right - removeOrder.Width;
                Application.OpenForms[0].Controls.Add(removeOrder);
                removeOrder.Click += delegate { RemoveOrder_Click(sender, e, lv); };

                Button shippedOrder = new Button()
                {
                    Text = DynamicResources.MenuFunctions_Orders_Click_Mark_as_shipped,
                    Top = removeOrder.Top,
                    Width = removeOrder.Width,
                    Height = removeOrder.Height
                };
                shippedOrder.Left = (int)(removeOrder.Left - shippedOrder.Width * 1.05);
                //viewOrder.Left = lv.Right - viewOrder.Width;
                Application.OpenForms[0].Controls.Add(shippedOrder);
                shippedOrder.Click += delegate { ShippedOrder_Click(sender, e, lv); };

                Button modifyOrder = new Button()
                {
                    Text = DynamicResources.MenuFunctions_Orders_Click_Modify_order,
                    Top = removeOrder.Top,
                    Width = removeOrder.Width,
                    Height = removeOrder.Height
                };
                modifyOrder.Left = (int)(shippedOrder.Left - modifyOrder.Width * 1.05);
                Application.OpenForms[0].Controls.Add(modifyOrder);
                modifyOrder.Click += delegate { ModifyOrder_Click(sender, e, lv); };

                Button createOrder = new Button()
                {
                    Text = DynamicResources.MenuFunctions_Orders_Click_Create_order,
                    Top = removeOrder.Top,
                    Width = removeOrder.Width,
                    Height = removeOrder.Height
                };
                createOrder.Left = (int)(modifyOrder.Left - createOrder.Width * 1.05);
                Application.OpenForms[0].Controls.Add(createOrder);
                createOrder.Click += CreateOrder_Click;

                Button fillStock = new Button()
                {
                    Text = DynamicResources.MenuFunctions_Orders_Click_Fill_stock,
                    Top = removeOrder.Top,
                    Width = removeOrder.Width,
                    Height = removeOrder.Height
                };
                fillStock.Left = lv.Left;
                Application.OpenForms[0].Controls.Add(fillStock);
                fillStock.Click += FillStock_Click;
            }



            Button close = new Button()
            {
                //Top = propertiesGB.Bottom + Application.OpenForms[0].Height / 25,
                //Left = Application.OpenForms[0].Width - Application.OpenForms[0].Width / 10,
                //Top = propertiesGb.Bottom,
                Top = (Application.OpenForms[0].Height / 16) * 14,
                Left = lv.Right,
                Text = DynamicResources.MenuFunctions_Billing_Click_Close,
                AutoSize = true
            };
            close.Top -= close.Height;
            close.Left -= close.Width;
            Application.OpenForms[0].Controls.Add(close);
            close.Click += Close_Click;


        }

        private static void FillStock_Click(object sender, EventArgs e)
        {
            FillStockFrm dialog = new FillStockFrm();
            dialog.ShowDialog();
        }

        private static void ShippedOrder_Click(object sender, EventArgs e, ListView lv)
        {
            try
            {
                if (MessageBox.Show(DynamicResources.MenuFunctions_ShippedOrder_Click_Are_you_sure_, DynamicResources.MenuFunctions_ShippedOrder_Click_Shipped, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    using (MaxterDBEntities context = new MaxterDBEntities())
                    {
                        if (int.TryParse(lv.SelectedItems[0].SubItems[0].Text, out int iD))
                        {
                            Order found = context.Orders.FirstOrDefault(x => x.Order_id == iD);
                            if (found != default)
                            {
                                if (found.Shipped_date == default)
                                {
                                    found.Shipped_date = DateTime.Now;
                                    context.SaveChanges();
                                }
                                else
                                {
                                    MessageBox.Show(DynamicResources.MenuFunctions_ShippedOrder_Click_Order_already_shipped_, DynamicResources.MenuFunctions_Login_Error, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }

        private static void ModifyOrder_Click(object sender, EventArgs eventArgs, ListView lv)
        {
            if (lv.SelectedItems.Count > 0)
            {

                int findOrderId = int.Parse(lv.SelectedItems[0].SubItems[0].Text);
                Order foundOrder;

                FormCleanUp();

                using (MaxterDBEntities context = new MaxterDBEntities())
                {
                    foundOrder = context.Orders.FirstOrDefault(x => x.Order_id == findOrderId);


                    DataGridView orderDgv = new DataGridView()
                    {
                        Top = Application.OpenForms[0].Height / 15,
                        Left = Application.OpenForms[0].Width / 25,
                        Width = Application.OpenForms[0].Width - (Application.OpenForms[0].Width / 25) * 2,
                        Height = (int)(Application.OpenForms[0].Height / 5 * 3.5),
                        Tag = foundOrder.Partner.Price_Category,
                        MultiSelect = false
                    };

                    Button finish = new Button()
                    {
                        //Top = propertiesGB.Bottom + Application.OpenForms[0].Height / 25,
                        //Left = Application.OpenForms[0].Width - Application.OpenForms[0].Width / 10,
                        //Top = propertiesGb.Bottom,
                        Top = (Application.OpenForms[0].Height / 16) * 14,
                        Left = orderDgv.Right,
                        Text = DynamicResources.MenuFunctions_Billing_Click_Finish,
                        AutoSize = true
                    };
                    finish.Top -= finish.Height;
                    finish.Left -= finish.Width;
                    Application.OpenForms[0].Controls.Add(finish);
                    finish.Click += Finish_Click;


                    //orderDgv.KeyPress += OrderDgv_KeyPress;

                    orderDgv.ColumnCount = 5;
                    orderDgv.Columns[0].Name = DynamicResources.MenuFunctions_OrderDgv_Sku_;
                    orderDgv.Columns[1].Name = DynamicResources.MenuFunctions_OrderDgv_Desc_;
                    orderDgv.Columns[2].Name = DynamicResources.MenuFunctions_OrderDgv_Qty_;
                    orderDgv.Columns[3].Name = DynamicResources.MenuFunctions_OrderDgv_Pu_;
                    orderDgv.Columns[4].Name = DynamicResources.MenuFunctions_OrderDgv_Price_;
                    foreach (DataGridViewColumn item in orderDgv.Columns)
                    {
                        item.Width = orderDgv.Width / orderDgv.Columns.Count;
                    }

                    Label partnerName = new Label()
                    {
                        Left = orderDgv.Left,
                        AutoSize = true,
                        Text = DynamicResources.MenuFunctions_ModifyOrder_Click_Partner__ + foundOrder.Partner.Name,
                        Top = orderDgv.Bottom + Application.OpenForms[0].Height / 15
                    };
                    Label orderId = new Label()
                    {
                        Left = partnerName.Left,
                        AutoSize = true,
                        Text = DynamicResources.MenuFunctions_ModifyOrder_Click_Order_ID__ + findOrderId,
                        Tag = findOrderId,
                        Top = partnerName.Bottom + partnerName.Height
                    };

                    Application.OpenForms[0].Controls.Add(orderDgv);
                    Application.OpenForms[0].Controls.Add(partnerName);
                    Application.OpenForms[0].Controls.Add(orderId);

                    orderDgv.CellBeginEdit += OrderDgv_CellBeginEdit;
                    orderDgv.CellEndEdit += (sender1, e1) => OrderDgv_CellEndEdit(sender1, e1, findOrderId);
                    orderDgv.KeyDown += (sender1, e1) => OrderDgv_KeyDown(sender1, e1, findOrderId);

                    /*for (int i = context.Order_detail.Count() - 1; i >= 0; i--)
                    {

                    }*/

                    int i = 0;
                    //int j = 0;
                    foreach (Order_detail item in context.Order_detail)
                    {
                        if (item.Order_id == findOrderId)
                        {
                            orderDgv.Rows.Add(new DataGridViewRow());
                            orderDgv.Rows[i].Cells[0].Value = item.SKU;
                            orderDgv.Rows[i].Cells[0].Tag = item.Order_Detail_id.ToString();
                            orderDgv.Rows[i].Cells[1].Value = item.Product.Description;
                            orderDgv.Rows[i].Cells[2].Value = item.Ordered_Quantity;
                            orderDgv.Rows[i].Cells[2].Tag = item.Ordered_Quantity;
                            orderDgv.Rows[i].Cells[3].Value = item.Product.PU;
                            orderDgv.Rows[i].Cells[4].Value = item.Item_Price;
                            i++;

                        }
                    }
                }
            }
        }

        private static void RemoveOrder_Click(object sender, EventArgs eventArgs, ListView lv)
        {
            if (lv.SelectedItems.Count > 0)
            {
                string id = lv.SelectedItems[0].SubItems[0].Text;
                using (MaxterDBEntities context = new MaxterDBEntities())
                {
                    Order found = context.Orders.FirstOrDefault(x => x.Order_id.ToString() == id);
                    if (found != default && found.Shipped_date == null)
                    {
                        if (MessageBox.Show(DynamicResources.MenuFunctions_RemoveOrder_Click_Are_you_sure_you_want_to_delete_the_selected_Order_, DynamicResources.MenuFunctions_RemoveOrder_Click_Delete,
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            List<Order_detail> delete = new List<Order_detail>();
                            foreach (Order_detail item in context.Order_detail)
                            {
                                if (item.Order_id == found.Order_id)
                                {
                                    delete.Add(item);
                                }
                            }

                            for (int i = delete.Count - 1; i >= 0; i--)
                            {
                                UpdateStock(delete[i].SKU, 0, delete[i].Ordered_Quantity);
                                DBFunctions.RemoveRecord(delete[i]);
                            }

                            DBFunctions.RemoveRecord(found);
                            //PartnerMenu(sender, e);
                        }
                    }
                    else
                    {
                        MessageBox.Show(DynamicResources.MenuFunctions_RemoveOrder_Click_Order_has_already_been_shipped_, DynamicResources.MenuFunctions_LoadInvoices_Click_Warning, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
            }
        }

        private static void CreateOrder_Click(object sender, EventArgs e)
        {
            OrderHeaderFrm dialog = new OrderHeaderFrm();

            if (dialog.ShowDialog() == DialogResult.OK) //If header is filled
            {
                FormCleanUp();
                DataGridView orderDgv = new DataGridView()
                {
                    Top = Application.OpenForms[0].Height / 15,
                    Left = Application.OpenForms[0].Width / 25,
                    Width = Application.OpenForms[0].Width - (Application.OpenForms[0].Width / 25) * 2,
                    Height = (int)(Application.OpenForms[0].Height / 5 * 3.5),
                    Tag = dialog.priceCategory,
                    MultiSelect = false
                };


                //orderDgv.KeyPress += OrderDgv_KeyPress;
                orderDgv.ColumnCount = 5;
                orderDgv.Columns[0].Name = DynamicResources.MenuFunctions_CreateOrder_SKU;
                orderDgv.Columns[1].Name = DynamicResources.MenuFunctions_CreateOrder_Description;
                orderDgv.Columns[2].Name = DynamicResources.MenuFunctions_CreateOrder_Qty;
                orderDgv.Columns[3].Name = DynamicResources.MenuFunctions_CreateOrder_PU;
                orderDgv.Columns[4].Name = DynamicResources.MenuFunctions_CreateOrder_Price;
                foreach (DataGridViewColumn item in orderDgv.Columns)
                {
                    item.Width = orderDgv.Width / orderDgv.Columns.Count;
                }

                Label partnerName = new Label()
                {
                    Left = orderDgv.Left,
                    AutoSize = true,
                    Text = DynamicResources.MenuFunctions_ModifyOrder_Click_Partner__ + dialog.Tag,
                    Top = orderDgv.Bottom + Application.OpenForms[0].Height / 15
                };
                Label orderId = new Label()
                {
                    Left = partnerName.Left,
                    AutoSize = true,
                    Text = DynamicResources.MenuFunctions_ModifyOrder_Click_Order_ID__ + dialog.orderId,
                    Tag = dialog.orderId,
                    Top = partnerName.Bottom + partnerName.Height
                };

                Application.OpenForms[0].Controls.Add(orderDgv);
                Application.OpenForms[0].Controls.Add(partnerName);
                Application.OpenForms[0].Controls.Add(orderId);

                orderDgv.CellBeginEdit += OrderDgv_CellBeginEdit;
                orderDgv.CellEndEdit += (sender1, e1) => OrderDgv_CellEndEdit(sender1, e1, dialog.orderId);
                orderDgv.KeyDown += (sender1, e1) => OrderDgv_KeyDown(sender1, e1, dialog.orderId);

                Button finish = new Button()
                {
                    //Top = propertiesGB.Bottom + Application.OpenForms[0].Height / 25,
                    //Left = Application.OpenForms[0].Width - Application.OpenForms[0].Width / 10,
                    //Top = propertiesGb.Bottom,
                    Top = (Application.OpenForms[0].Height / 16) * 14,
                    Left = orderDgv.Right,
                    Text = DynamicResources.MenuFunctions_Billing_Click_Finish,
                    AutoSize = true
                };
                finish.Top -= finish.Height;
                finish.Left -= finish.Width;
                Application.OpenForms[0].Controls.Add(finish);
                finish.Click += Finish_Click;
            }
        }

        private static void Finish_Click(object sender, EventArgs e)
        {
            Orders_Click(sender, e);
        }

        private static void OrderDgv_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if ((sender as DataGridView)?.SelectedCells[0].OwningColumn.Name == (sender as DataGridView)?.Columns[0].Name && (sender as DataGridView)?.SelectedCells[0].Value != null)
            {
                e.Cancel = true;
            }
        }

        private static void OrderDgv_CellEndEdit(object sender, DataGridViewCellEventArgs e, int id)
        {
            if ((sender as DataGridView)?.SelectedCells[0].OwningColumn.Name == (sender as DataGridView)?.Columns[0].Name)
            {
                OrderDgv_KeyDown(sender, new KeyEventArgs(Keys.Enter), id);

            }
            else if ((sender as DataGridView).SelectedCells[0].OwningColumn.Name == (sender as DataGridView).Columns[1].Name)
            {
                //(sender as DataGridView).SelectedCells[0].Value = null;
                //DataGridViewRow lastRow = (sender as DataGridView).Rows[(sender as DataGridView).RowCount - 1];
                //(sender as DataGridView).Rows.Remove(lastRow);
            }
            else if ((sender as DataGridView).SelectedCells[0].OwningColumn.Name == (sender as DataGridView).Columns[2].Name)
            {
                if ((sender as DataGridView).SelectedCells[0].OwningRow.Cells[2].Tag == null)
                {
                    (sender as DataGridView).SelectedCells[0].OwningRow.Cells[2].Tag = 0;
                }
                else if (int.TryParse((sender as DataGridView).SelectedCells[0].OwningRow.Cells[2].Value.ToString(), out int save))
                {

                }

                if ((sender as DataGridView).SelectedCells[0].OwningRow.Cells[2].Value != null)
                {
                    if (int.TryParse((sender as DataGridView).SelectedCells[0].OwningRow.Cells[2].Value.ToString(), out int result))
                    {
                        int stock = 0;

                        using (MaxterDBEntities context = new MaxterDBEntities())
                        {
                            string sku = (sender as DataGridView).SelectedCells[0].OwningRow.Cells[0].Value.ToString();
                            var found = context.Products.FirstOrDefault(x => x.SKU == sku);
                            if (found != default)
                            {
                                stock = Convert.ToInt32(found.Stock_Quantity);
                                if (result < stock)
                                {
                                    UpdateStock((sender as DataGridView).SelectedCells[0].OwningRow.Cells[0].Value.ToString(), result, (int)(sender as DataGridView).SelectedCells[0].OwningRow.Cells[2].Tag);
                                    (sender as DataGridView).SelectedCells[0].OwningRow.Cells[2].Tag =
                                        int.Parse((sender as DataGridView).SelectedCells[0].OwningRow.Cells[2].Value.ToString());
                                }
                                else
                                {
                                    MessageBox.Show(string.Format(DynamicResources.MenuFunctions_OrderDgv_CellEndEdit_Requested_amount_exceeds_current_stock__Avaiable_stock___0_, stock), DynamicResources.MenuFunctions_LoadInvoices_Click_Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    (sender as DataGridView).SelectedCells[0].OwningRow.Cells[2].Value = (sender as DataGridView).SelectedCells[0].OwningRow.Cells[2].Tag.ToString();
                                    (sender as DataGridView).SelectedCells[0].OwningRow.Cells[2].Tag =
                                        int.Parse((sender as DataGridView).SelectedCells[0].OwningRow.Cells[2].Value.ToString());
                                }
                            }
                        }



                        using (MaxterDBEntities context = new MaxterDBEntities())
                        {
                            int iD = int.Parse((sender as DataGridView).CurrentRow.Cells[0].Tag.ToString());
                            Order_detail found = context.Order_detail.FirstOrDefault(x => x.Order_Detail_id == iD);
                            found.Ordered_Quantity = result;
                            context.SaveChanges();
                        }
                    }
                }
            }
            else if ((sender as DataGridView).SelectedCells[0].OwningColumn.Name == (sender as DataGridView).Columns[3].Name)
            {

            }

            if (sender != null)
            {
                int i = ((DataGridView)sender).Rows.Count - 1;
                DataGridViewRow row = (sender as DataGridView)?.Rows[i];
                for (; i > -1; i--)
                {

                    if (!row.IsNewRow && (row.Cells[0].Value == null || ((string)row.Cells[2].Value == "0" ||
                                          !int.TryParse(row.Cells[2].Value.ToString(), out int res))))
                    {
                        (sender as DataGridView)?.Rows.RemoveAt(i);
                    }

                    /*var value = row.Cells[0].Value;
                    if (value == null  && (string)row.Cells[0].Tag != null)
                    {
                        if ((string)row.Cells[0].Tag != "Set")
                        {
                            using (MaxterDBEntities context = new MaxterDBEntities())
                            {
                                int.TryParse(row.Cells[0].Tag.ToString(), out int orderDetailId);
                                var toDelete = context.Order_detail.FirstOrDefault(x =>
                                    x.Order_Detail_id == orderDetailId);

                                DBFunctions.RemoveRecord(toDelete);
                            }
                        }
                    }*/
                }

            }
        }
    }

    #endregion
}
