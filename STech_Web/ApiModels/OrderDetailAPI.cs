using STech_Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace STech_Web.ApiModels
{
    public class OrderDetailAPI
    {
        public string OrderID { get; set; }
        public int Quantity { get; set; }
        public ProductAPI Product { get; set; }

        public OrderDetailAPI() { }

        public OrderDetailAPI(ProductAPI product, string orderID, int quantity) 
        { 
            this.Product = product;
            this.OrderID = orderID;
            this.Quantity = quantity;
        }
    }
}