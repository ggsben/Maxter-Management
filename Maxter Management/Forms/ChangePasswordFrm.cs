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

namespace Maxter_Management.Forms
{
    public partial class ChangePasswordFrm : Form
    {
        private int idSaved;

        public ChangePasswordFrm(int id)
        {
            InitializeComponent();
            idSaved = id;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Trim() != "" && textBox2.Text.Trim() != "" && textBox3.Text.Trim() != "")
            {
                try
                {
                    using (UsersDBEntities context = new UsersDBEntities())
                    {
                        User foundUser = context.Users.FirstOrDefault(x => x.Id == idSaved);

                        if (foundUser.Password != default)
                        {
                            string encryptedPass = BitConverter
                                .ToString(new SHA256CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(textBox1.Text.Trim() + "hahayessecurity")))
                                .Replace("-", "").ToLower();
                            if (foundUser.Password == encryptedPass && textBox2.Text == textBox3.Text)
                            {
                                string newPassEncrypted = BitConverter
                                    .ToString(new SHA256CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(textBox2.Text.Trim() + "hahayessecurity")))
                                    .Replace("-", "").ToLower();

                                User updated = new User();
                                updated.Id = foundUser.Id;
                                updated.Access = foundUser.Access;
                                updated.Username = foundUser.Username;
                                updated.Password = newPassEncrypted;
                                updated.First_name = foundUser.First_name;
                                updated.Last_name = foundUser.Last_name;

                                UserDBFunctions.UpdateUser(updated);
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
        }
    }
}
