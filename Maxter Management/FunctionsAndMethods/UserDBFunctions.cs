using System;
using System.Linq;
using System.Windows.Forms;
using Maxter_Management.Models;

namespace Maxter_Management.FunctionsAndMethods
{
    static class UserDBFunctions
    {
        public static void CreateNewUser(User newUser)
        {
            try
            {
                using (UsersDBEntities context = new UsersDBEntities())
                {
                    var exists = context.Users.FirstOrDefault(x => x.Username == newUser.Username);

                    if (exists == default)
                    {
                        int idMax = context.Users.Max(x => x.Id);
                        newUser.Id = idMax + 1;
                        context.Users.Add(newUser);

                        context.SaveChanges();
                    }
                    else
                    {
                        MessageBox.Show("Username taken!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static void UpdateUser(User updatedUser)
        {
            try
            {
                using (UsersDBEntities context = new UsersDBEntities())
                {
                    User foundUser = context.Users.FirstOrDefault(x => x.Id == updatedUser.Id);

                    if (foundUser != default)
                    {
                        foundUser.First_name = updatedUser.First_name;
                        foundUser.Last_name = updatedUser.Last_name;
                        foundUser.Password = updatedUser.Password;
                        foundUser.Access = updatedUser.Access;

                        context.SaveChanges();
                    }
                    else
                    {
                        MessageBox.Show("User does not exist!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static void RemoveUser(int id)
        {
            try
            {
                using (UsersDBEntities context = new UsersDBEntities())
                {
                    User foundUser = context.Users.FirstOrDefault(x => x.Id == id);
                    
                    if (foundUser != default)
                    {
                        context.Users.Remove(foundUser);

                        context.SaveChanges();
                    }
                    else
                    {
                        MessageBox.Show("User does not exist!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
