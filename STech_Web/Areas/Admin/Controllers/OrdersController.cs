using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using STech_Web.Filters;
using STech_Web.Models;
using System.Globalization;
using Microsoft.Owin.Security;
using System.Security;
using STech_Web.Identity;
using Microsoft.AspNet.Identity;

namespace STech_Web.Areas.Admin.Controllers
{
    [ManagerAuthorization]
    public class OrdersController : Controller
    {
        // GET: Admin/Order
        public ActionResult Index()
        {
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            List<Order> orders = db.Orders.Where(t => t.Status == "Chờ xác nhận").ToList();

            CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
            ViewBag.cul = cul;
            ViewBag.ActiveNav = "orders";
            return View(orders);
        }

        //Đếm số hóa đơn mới chờ xác nhận---
        [HttpGet]
        public JsonResult CountNewOrder()
        {
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            List<Order> order = db.Orders.Where(t => t.Status == "Chờ xác nhận").ToList();
            return Json(new { count = order.Count }, JsonRequestBehavior.AllowGet);
        }

        //In hóa đơn ----------------
        public ActionResult PrintOrder(string orderID)
        {
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            Order order = db.Orders.FirstOrDefault(t => t.OrderID == orderID);
            if (order == null)
            {
                return Redirect("#");
            }

            PrintInvoice printInvoice = new PrintInvoice(order);
            byte[] file = printInvoice.Print();

            return File(file, printInvoice.ContentType, printInvoice.FileName);
        }

        //Xác nhận đã thanh toán -----------
        [HttpPost]
        public JsonResult AcceptPaid(string orderID = "")
        {
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            Order order = db.Orders.FirstOrDefault(t => t.OrderID == orderID);
            if (order != null)
            {
                order.PaymentStatus = "Đã thanh toán";
                db.SaveChanges();
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }

        //Xác nhận/hủy đơn hàng --------------
        [HttpPost]
        public JsonResult UpdateStatus(string orderID = "", string type = "")
        {
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            Order order = db.Orders.FirstOrDefault(t => t.OrderID == orderID);

            if (order == null)
            {
                return Json(new { success = false });
            }

            if (type == "accept")
            {
                order.Status = "Đã xác nhận";
            }
            else
            {
                order.Status = "Đã hủy";
            }

            db.SaveChanges();
            return Json(new { success = true });
        }

        //Tạo đơn hàng ---------------
        public ActionResult Create()
        {
            ViewBag.ActiveNav = "orders";
            return View();
        }

        [HttpPost]
        public JsonResult Create(Customer cus, string productStr, string paymentMethod, string note)
        {
            try
            {
                DatabaseSTechEntities db = new DatabaseSTechEntities();
                List<Product> products = new List<Product>();
                Customer customer = db.Customers.FirstOrDefault(c => c.CustomerID == cus.CustomerID);

                List<Order> orders = db.Orders.OrderByDescending(t => t.OrderID).ToList();
                int orderNumber = 1;
                if (orders.Count > 0)
                {
                    orderNumber = int.Parse(orders[0].OrderID.Substring(2)) + 1;
                }

                string orderID = "DH" + orderNumber.ToString().PadLeft(5, '0');
                decimal totalPrice = 0;

                //Cập nhật thông tin khách hàng
                customer.CustomerName = cus.CustomerName;
                customer.Phone = cus.Phone;
                customer.Address = cus.Address;
                customer.Gender = cus.Gender;
                customer.Email = cus.Email;

                Order order = new Order();
                order.OrderID = orderID;
                order.CustomerID = customer.CustomerID;
                order.OrderDate = DateTime.Now;
                order.Note = note;
                order.ShipMethod = "COD";
                order.DeliveryFee = 0;
                order.PaymentStatus = "Chờ thanh toán";
                order.Status = "Chờ xác nhận";
                order.PaymentMethod = paymentMethod;
                order.ShipAddress = customer.Address;

                //Tạo chi tiết đơn hàng
                List<OrderDetail> orderDetails = new List<OrderDetail>();

                List<string> productString = productStr.Split(new string[] { ";;;;;;;;" }, StringSplitOptions.None).ToList();
                foreach (string str in productString)
                {
                    if(!String.IsNullOrEmpty(str))
                    {
                        string[] parts = str.Split(new string[] { "++++++++" }, StringSplitOptions.None);
                        string productID = parts[0];
                        int qty = Convert.ToInt32(parts[1]);

                        Product product = db.Products.FirstOrDefault(t => t.ProductID == productID);
                        totalPrice += qty * (decimal)product.Price;
                        OrderDetail orderDetail = new OrderDetail();
                        orderDetail.OrderID = orderID;
                        orderDetail.ProductID = product.ProductID;
                        orderDetail.Quantity = qty;
                        orderDetail.Price = product.Price;
                        orderDetails.Add(orderDetail);

                        //Trừ số lượng khỏi kho
                        WareHouse wh = product.WareHouse;
                        wh.Quantity -= qty;
                    }
                }

                order.TotalPrice = totalPrice;
                order.TotalPaymentAmout = totalPrice + (decimal)order.DeliveryFee;
                db.Orders.Add(order);
                db.OrderDetails.AddRange(orderDetails);
                db.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false });
            }         
        }

        //Tăng/giảm số lượng sản phẩm trong đơn hàng
        [HttpPost]
        public JsonResult UpdateProductQty(string productID, int qty)
        {
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            Product product = db.Products.FirstOrDefault(t => t.ProductID == productID);
            if(product == null)
            {
                return Json(new { success = false });
            }

            if (qty <= 0) qty = 1;
            if (product.WareHouse.Quantity < qty) qty = (int)product.WareHouse.Quantity;

            return Json(new { success = true, quantity = qty, total = qty * product.Price });
        }

        //Xóa đơn hàng ---------------
        [HttpPost]
        public JsonResult DeleteOrder(string orderID = "")
        {
            try
            {
                DatabaseSTechEntities db = new DatabaseSTechEntities();
                Order order = db.Orders.FirstOrDefault(t => t.OrderID == orderID);
                if(order == null)
                {
                    return Json(new { success = false, error = "Đã xẩy ra lỗi" });
                }

                List<OrderDetail> orderDetails = order.OrderDetails.ToList();
                //Cập nhật lại số lượng sản phẩm
                foreach (OrderDetail orderDetail in orderDetails)
                {
                    WareHouse wh = orderDetail.Product.WareHouse;
                    wh.Quantity += orderDetail.Quantity;
                }
                db.OrderDetails.RemoveRange(orderDetails);
                db.Orders.Remove(order);
                db.SaveChanges(); 

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = "Đã xẩy ra lỗi" });
            }       
        }
    }
}