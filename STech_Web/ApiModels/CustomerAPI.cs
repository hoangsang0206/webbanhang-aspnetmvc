using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace STech_Web.ApiModels
{
    public class CustomerAPI
    {
        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Gender { get; set; }
        public DateTime? DoB { get; set; }
        public string Email { get; set; }

        public CustomerAPI() { }
        public CustomerAPI(string CustomerID, string CustomerName, string Phone, string Address, string Gender, DateTime? DoB, string Email)
        {
            this.CustomerID = CustomerID;
            this.CustomerName = CustomerName;
            this.Phone = Phone;
            this.Address = Address;
            this.Gender = Gender;
            this.DoB = DoB;
            this.Email = Email;
        }
    }
}