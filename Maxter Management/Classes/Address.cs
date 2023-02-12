using System.Collections.Generic;
using Maxter_Management.Models;

namespace Maxter_Management.FunctionsAndMethods
{
    public class Address
    {
        private string Street_name;
        private string Street_type;
        private string House_nr;
        private string Block;
        private string Entrance;
        private string Floor;
        private string Door;
        private string City;
        private string Postcode;
        private string Country;
        private string District;

        public Address(Partner paramPartner)
        {
            string[] split = paramPartner.Invoice_Address.Split(';');

            Street_name = split[3];
            Street_type = split[4];
            House_nr = split[5];
            Block = "";
            Entrance = "";
            Floor = "";
            Door = "";
            City = split[2];
            Postcode = split[1];
            Country = split[0];
            District = "";
        }

        public Address(bool test)
        {
            Street_name = "Utcanev";
            Street_type = "utca";
            House_nr = "2.";
            Block = "";
            Entrance = "";
            Floor = "";
            Door = "";
            City = "Budapest";
            Postcode = "2000";
            Country = "Magyarorszag";
            District = "";
        }

        public string street_name { get => Street_name; set => Street_name = value; }
        public string street_type { get => Street_type; set => Street_type = value; }
        public string house_nr { get => House_nr; set => House_nr = value; }
        public string block { get => Block; set => Block = value; }
        public string entrance { get => Entrance; set => Entrance = value; }
        public string floor { get => Floor; set => Floor = value; }
        public string door { get => Door; set => Door = value; }
        public string city { get => City; set => City = value; }
        public string postcode { get => Postcode; set => Postcode = value; }
        public string country { get => Country; set => Country = value; }
        public string district { get => District; set => District = value; }
    }
}