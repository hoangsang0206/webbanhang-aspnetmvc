using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.DynamicData;
using System.Web.Http;
using STech_Web.Filters;
using STech_Web.Models;
using STech_Web.ApiModels;

namespace STech_Web.ApiControllers
{
    [Authorize(Roles = "Admin, Manager")]
    public class OrdersController : ApiController
    {
        //Lấy tất cả đơn hàng
        public List<OrderAPI> GetAll()
        {
            List<OrderAPI> ordersApi = new List<OrderAPI>();
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            List<Order> orders = db.Orders.ToList();

            if (orders.Count > 0)
            {
                foreach (Order order in orders)
                {
                    ordersApi.Add(new OrderAPI(order.Customer.CustomerID, order.Customer.CustomerName, order.OrderID, (DateTime)order.OrderDate, order.TotalPrice, order.PaymentStatus, order.Status, order.Note, (decimal)order.DeliveryFee, order.TotalPaymentAmout, order.ShipMethod, order.PaymentMethod, order.ShipAddress));
                }
            }

            return ordersApi;
        }

        //Lấy 1 đơn hàng
        public OrderAPI GetOne(string id)
        {
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            Order order = db.Orders.FirstOrDefault(t => t.OrderID ==  id);
            OrderAPI orderApi = new OrderAPI();
            if(order != null)
            {
                orderApi = new OrderAPI(order.Customer.CustomerID, order.Customer.CustomerName, order.OrderID, (DateTime)order.OrderDate, order.TotalPrice, order.PaymentStatus, order.Status, order.Note, (decimal)order.DeliveryFee, order.TotalPaymentAmout, order.ShipMethod, order.PaymentMethod, order.ShipAddress);

            }

            return orderApi;
        }

        //Lấy chi tiết 1 đơn hàng
        public List<OrderDetailAPI> GetOrderDetail(string orderID)
        {
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            List<OrderDetail> orderDetails = db.OrderDetails.Where(t => t.OrderID == orderID).ToList();
            List<OrderDetailAPI> orderDetailAPIs = new List<OrderDetailAPI>();

            if(orderDetails.Count > 0)
            {
                foreach(OrderDetail orderDetail in orderDetails)
                {
                    Product p = db.Products.FirstOrDefault(t => t.ProductID == orderDetail.ProductID);
                    ProductAPI productAPI = new ProductAPI(p.ProductID, p.ProductName, p.ImgSrc, (decimal)p.Cost, (decimal)p.Price, (int)p.Warranty, p.BrandID, p.CateID, (int)p.WareHouse.Quantity);
                    orderDetailAPIs.Add(new OrderDetailAPI(productAPI, orderDetail.OrderID, orderDetail.Quantity));
                }
            }

            return orderDetailAPIs;
        }

        //Tìm kiếm đơn hàng
        public List<OrderAPI> GetOrders(string searchType, string searchValue)
        {
            List<OrderAPI> ordersApi = new List<OrderAPI>();
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            List<Order> orders = new List<Order>();

            try
            {
                switch (searchType)
                {
                    case "order-id":
                        orders = db.Orders.Where(t => t.OrderID == searchValue).ToList();
                        break;
                    case "customer-name":
                        orders = db.Orders.Where(t => t.Customer.CustomerName.ToLower().Contains(searchValue.ToLower())).ToList();
                        break;
                    case "customer-phone":
                        orders = db.Orders.Where(t => t.Customer.Phone == searchValue).ToList();
                        break;
                    case "order-date":
                        DateTime date = DateTime.Parse(searchValue);
                        orders = db.Orders.Where(t => t.OrderDate.Value.Day == date.Day && t.OrderDate.Value.Month == date.Month && t.OrderDate.Value.Year == date.Year).ToList();
                        break;
                    case "order-month":
                        DateTime date1 = DateTime.Parse(searchValue);
                        orders = db.Orders.Where(t => t.OrderDate.Value.Month == date1.Month && t.OrderDate.Value.Year == date1.Year).ToList();
                        break;
                }

                if (orders.Count > 0)
                {
                    foreach (Order order in orders)
                    {
                        ordersApi.Add(new OrderAPI(order.Customer.CustomerID, order.Customer.CustomerName, order.OrderID, (DateTime)order.OrderDate, order.TotalPrice, order.PaymentStatus, order.Status, order.Note, (decimal)order.DeliveryFee, order.TotalPaymentAmout, order.ShipMethod, order.PaymentMethod, order.ShipAddress));
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return ordersApi;
        }

        //Tìm sản phẩm theo khoảng thòi gian
        public List<OrderAPI> GetOrdersByDateRange(string dateFrom, string dateTo)
        {
            List<OrderAPI> ordersApi = new List<OrderAPI>();
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            List<Order> orders = new List<Order>();

            try
            {
                
                DateTime dateF = DateTime.Parse(dateFrom);
                DateTime dateT = DateTime.Parse(dateTo);

                orders = db.Orders.Where(t => t.OrderDate >= dateF && t.OrderDate <= dateT).ToList();
                
                if (orders.Count > 0)
                {
                    foreach (Order order in orders)
                    {
                        ordersApi.Add(new OrderAPI(order.Customer.CustomerID, order.Customer.CustomerName, order.OrderID, (DateTime)order.OrderDate, order.TotalPrice, order.PaymentStatus, order.Status, order.Note, (decimal)order.DeliveryFee, order.TotalPaymentAmout, order.ShipMethod, order.PaymentMethod, order.ShipAddress));
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return ordersApi;
        }

        //Lấy các đơn hàng chờ xác nhận
        public List<OrderAPI> GetOrderWaiting(string type, string status)
        {
            List<OrderAPI> ordersApi = new List<OrderAPI>();
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            List<Order> orders = new List<Order>();

            if(type == "status")
            {
                orders = db.Orders.Where(t => t.Status == status).ToList();
            }
            else if(type == "payment-stt")
            {
                orders = db.Orders.Where(t => t.PaymentStatus == status).ToList();
            }

            if (orders.Count > 0)
            {
                foreach (Order order in orders)
                {
                    ordersApi.Add(new OrderAPI(order.Customer.CustomerID, order.Customer.CustomerName, order.OrderID, (DateTime)order.OrderDate, order.TotalPrice, order.PaymentStatus, order.Status, order.Note, (decimal)order.DeliveryFee, order.TotalPaymentAmout, order.ShipMethod, order.PaymentMethod, order.ShipAddress));
                }
            }
            return ordersApi;
        }

    }
}
