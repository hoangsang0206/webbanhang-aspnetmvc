using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace STech_Web.Models
{
    public class CartItem
    {
        public string ProductID { get; set; }
        public int Quantity { get; set; }

        public CartItem() { }
        public CartItem(string productID, int quantity)
        {
            this.ProductID = productID;
            this.Quantity = quantity;
        }
    }
}