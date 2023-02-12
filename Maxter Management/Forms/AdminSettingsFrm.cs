using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Maxter_Management.FunctionsAndMethods;
using Maxter_Management.Models;
using Maxter_Management.Resources;

namespace Maxter_Management.Forms
{
    public partial class AdminSettingsFrm : Form
    {
        private List<User> users;

        public AdminSettingsFrm()
        {
            InitializeComponent();
            users = new List<User>();

            using (UsersDBEntities context = new UsersDBEntities())
            {
                foreach (User item in context.Users)
                {
                    users.Add(item);
                }

                listBox1.DataSource = users;
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItems.Count > 0)
            {
                textBox1.Text = ((User) listBox1.SelectedItem).First_name;
                textBox2.Text = ((User) listBox1.SelectedItem).Last_name;
                textBox3.Text = ((User) listBox1.SelectedItem).Username;
                string temp = ((User) listBox1.SelectedItem).Access;

                if (temp[0] != 'N')
                {
                    checkBox1.Checked = true;
                }
                else
                {
                    checkBox1.Checked = false;
                }
                if (temp[1] != 'N')
                {
                    checkBox2.Checked = true;
                }
                else
                {
                    checkBox2.Checked = false;
                }
                if (temp[2] != 'N')
                {
                    checkBox3.Checked = true;
                }
                else
                {
                    checkBox3.Checked = false;
                }
                if (temp[3] != 'N')
                {
                    checkBox4.Checked = true;
                }
                else
                {
                    checkBox4.Checked = false;
                }
            }
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox6.Checked)
            {
                textBox1.ReadOnly = false;
                textBox2.ReadOnly = false;
                textBox3.ReadOnly = true;

                checkBox1.Enabled = true;
                checkBox2.Enabled = true;
                checkBox3.Enabled = true;
                checkBox4.Enabled = true;
                checkBox5.Enabled = false;

                listBox1.Enabled = false;
            }
            else
            {
                if (MessageBox.Show(DynamicResources.AdminSettingsFrm_checkBox5_CheckedChanged_Cancel_editing_, DynamicResources.AdminSettingsFrm_checkBox5_CheckedChanged_Cancel, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    textBox1.ReadOnly = true;
                    textBox2.ReadOnly = true;
                    textBox3.ReadOnly = true;

                    checkBox1.Enabled = false;
                    checkBox2.Enabled = false;
                    checkBox3.Enabled = false;
                    checkBox4.Enabled = false;
                    checkBox5.Enabled = true;

                    listBox1.Enabled = true;
                    listBox1_SelectedIndexChanged(sender, e);
                }
                else
                {
                    checkBox6.Checked = true;
                }
            }
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox5.Checked)
            {
                textBox1.ReadOnly = false;
                textBox1.Text = "";
                textBox2.ReadOnly = false;
                textBox2.Text = "";
                textBox3.ReadOnly = false;
                textBox3.Text = "";
                textBox4.ReadOnly = false;

                checkBox1.Enabled = true;
                checkBox1.Checked = false;
                checkBox2.Enabled = true;
                checkBox2.Checked = false;
                checkBox3.Enabled = true;
                checkBox3.Checked = false;
                checkBox4.Enabled = true;
                checkBox4.Checked = false;
                checkBox6.Enabled = false;

                listBox1.Enabled = false;
            }
            else
            {
                if (MessageBox.Show(DynamicResources.AdminSettingsFrm_checkBox5_CheckedChanged_Cancel_editing_, DynamicResources.AdminSettingsFrm_checkBox5_CheckedChanged_Cancel, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    textBox1.ReadOnly = true;
                    textBox1.Text = "";
                    textBox2.ReadOnly = true;
                    textBox2.Text = "";
                    textBox3.ReadOnly = true;
                    textBox3.Text = "";
                    textBox4.ReadOnly = true;
                    textBox4.Text = "";

                    checkBox1.Enabled = false;
                    checkBox1.Checked = false;
                    checkBox2.Enabled = false;
                    checkBox2.Checked = false;
                    checkBox3.Enabled = false;
                    checkBox3.Checked = false;
                    checkBox4.Enabled = false;
                    checkBox4.Checked = false;
                    checkBox6.Enabled = true;

                    listBox1.Enabled = true;
                    listBox1_SelectedIndexChanged(sender, e);
                }
                else
                {
                    checkBox5.Checked = true;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool filled = true;
            foreach (Control item in Controls)
            {
                if (item is TextBox && item.Text.Trim().Length == 0 && item.Name != "textBox4")
                {
                    filled = false;
                }
            }

            if ((checkBox5.Checked || checkBox6.Checked) && filled && MessageBox.Show(DynamicResources.AdminSettingsFrm_button1_Click_Are_you_sure_, DynamicResources.AdminSettingsFrm_button1_Click_Confirmation, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (checkBox5.Checked)
                {
                    if (textBox4.Text.Trim().Length >= 6 && textBox4.Text.Trim().Length <= 24)
                    {
                        string access = AccessCheck();

                        string newPassEncrypted = BitConverter
                            .ToString(new SHA256CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(textBox4.Text.Trim() + "hahayessecurity")))
                            .Replace("-", "").ToLower();

                        User newUser = new User(textBox1.Text.Trim(), textBox2.Text.Trim(), textBox3.Text.Trim(), newPassEncrypted, access);

                        UserDBFunctions.CreateNewUser(newUser);
                    }
                    else
                    {
                        MessageBox.Show(DynamicResources.AdminSettingsFrm_button1_Click_Password_Length, DynamicResources.AdminSettingsFrm_button1_Click_Figyelmeztetés, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    string access = AccessCheck();

                    User updateUser = new User();

                    updateUser.Id = ((User) listBox1.SelectedItem).Id;
                    updateUser.First_name = textBox1.Text.Trim();
                    updateUser.Last_name = textBox2.Text.Trim();
                    updateUser.Username = ((User) listBox1.SelectedItem).Username;
                    updateUser.Password = ((User) listBox1.SelectedItem).Password;
                    updateUser.Access = access;

                    UserDBFunctions.UpdateUser(updateUser);
                }
            }
            else if ((checkBox5.Checked || checkBox6.Checked) && !filled)
            {
                MessageBox.Show(DynamicResources.AdminSettingsFrm_button1_Click_Please_fill_every_field_, DynamicResources.AdminSettingsFrm_button1_Click_Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = DialogResult.None;
            }
        }

        private string AccessCheck()
        {
            string access = "";

            if (checkBox1.Checked)
            {
                access += 'Y';
            }
            else
            {
                access += 'N';
            }
            if (checkBox2.Checked)
            {
                access += 'Y';
            }
            else
            {
                access += 'N';
            }
            if (checkBox3.Checked)
            {
                access += 'Y';
            }
            else
            {
                access += 'N';
            }
            if (checkBox4.Checked)
            {
                access += 'Y';
            }
            else
            {
                access += 'N';
            }

            return access;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(DynamicResources.AdminSettingsFrm_button3_Click_Delete_selected_user_, DynamicResources.AdminSettingsFrm_button1_Click_Confirmation, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                UserDBFunctions.RemoveUser((listBox1.SelectedItem as User).Id);
                users.Remove((User)listBox1.SelectedItem);
                listBox1.DataSource = null;
                listBox1.DataSource = users;
            }
        }
    }
}
