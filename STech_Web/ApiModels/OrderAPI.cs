using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace STech_Web.Models
{
    public class OrderAPI
    {
        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string OrderID { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string PaymentStatus { get; set; }
        public string Status { get; set; }
        public string Note { get; set; }
        public decimal DeliveryFee { get; set; }
        public decimal TotalPaymentAmout { get; set; }
        public string ShipMethod { get; set; }
        public string PaymentMethod { get; set; }
        public string ShipAddress { get; set; }

        public OrderAPI() { }

        public OrderAPI(string CustomerID, string CustomerName, string OrderID, DateTime OrderDate, decimal TotalPrice, string PaymentStatus, string Status, string Note, decimal DeliveryFee, decimal TotalPaymentAmout, string ShipMethod, string PaymentMethod, string shipAddress)
        {
            this.CustomerID = CustomerID;
            this.CustomerName = CustomerName;
            this.OrderID = OrderID;
            this.OrderDate = OrderDate;
            this.TotalPrice = TotalPrice;
            this.PaymentStatus = PaymentStatus;
            this.Status = Status;
            this.Note = Note;
            this.DeliveryFee = DeliveryFee;
            this.TotalPaymentAmout = TotalPaymentAmout;
            this.ShipMethod = ShipMethod;
            this.PaymentMethod = PaymentMethod;
            this.ShipAddress = shipAddress;
        }
    }
}