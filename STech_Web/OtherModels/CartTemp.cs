using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace STech_Web.Models
{
    public class CartTemp
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public CartTemp() { }
        public CartTemp(Product product, int quantity) 
        { 
            this.Product = product;
            this.Quantity = quantity;
        }
    }
}