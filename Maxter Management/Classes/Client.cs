using System.Runtime.CompilerServices;
using Maxter_Management.Models;

namespace Maxter_Management.FunctionsAndMethods
{
    public class Client
    {
        private string Name;
        private string Email;
        private string Taxcode;
        private Address Billing_address;
        //private int id;

        public Client(Address paramAddress, Partner paramPartner)
        {
            Name = paramPartner.Name;
            Email = paramPartner.Email;
            Taxcode = paramPartner.TIN;
            //Billing_address = paramAddress;
        }

        public Client(Partner paramPartner)
        {
            Name = paramPartner.Name;
            Email = paramPartner.Email;
            Taxcode = paramPartner.TIN;
            Billing_address = new Address(paramPartner);
        }

        public Client(bool test, Address paramAddress)
        {
            Name = "Teszt Elek Kft.";
            Email = "teszt@mail.com";
            Taxcode = "1122132432";
            //Billing_address = paramAddress;
            //Id = id;
        }

        public string name { get => Name; set => Name = value; }
        public string email { get => Email; set => Email = value; }
        public string taxcode { get => Taxcode; set => Taxcode = value; }
        public Address billing_address { get => Billing_address; set => Billing_address = value; }
        //public int Id { get => id; set => id = value; }
    }
}