using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maxter_Management.Classes
{
    class TempInvoice
    {
        private string id;
        private string date;
        private string client;

        public TempInvoice(string id, string date, string client)
        {
            Id = id;
            Date = date;
            Client = client;
        }

        public override string ToString()
        {
            return $"{id} - {date} - {client}";
        }

        public string Id { get => id; set => id = value; }
        public string Date { get => date; set => date = value; }
        public string Client { get => client; set => client = value; }
    }
}
