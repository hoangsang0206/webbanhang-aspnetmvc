using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.DynamicData;

namespace STech_Web.Models
{
    public class OrderTemp
    {
       
        public string ShipMethod { get; set; }
        public string Note { get; set; }

        public OrderTemp() { }

        public OrderTemp(string shipmethod, string note)
        {
            this.ShipMethod = shipmethod;
            this.Note = note;
        }
    }
}