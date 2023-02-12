//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Maxter_Management.Models
{
    using System;
    using System.Collections.Generic;

    public partial class Price
    {
        public string SKU { get; set; }
        public int Price_id { get; set; }
        public Nullable<decimal> Price1 { get; set; }
        public Nullable<byte> Price_Category { get; set; }

        public virtual Product Product { get; set; }

        public Price() { }

        public Price(string sKU, decimal? price1, byte? price_Category)
        {
            SKU = sKU;
            Price1 = price1;
            Price_Category = price_Category;

            using (MaxterDBEntities context = new MaxterDBEntities())
            {
                int max = 0;
                foreach (Price item in context.Prices)
                {
                    if (item.Price_id > max)
                    {
                        max = item.Price_id;
                    }
                }

                Price_id = max + 1;
            }
        }
    }
}