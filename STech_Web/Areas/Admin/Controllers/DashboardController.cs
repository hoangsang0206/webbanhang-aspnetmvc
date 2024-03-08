using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using STech_Web.Filters;
using STech_Web.Identity;
using STech_Web.Models;

namespace STech_Web.Areas.Admin.Controllers
{
    [ManagerAuthorization]
    public class DashboardController : Controller
    {
        // GET: Admin/Home
        public ActionResult Index()
        {
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            List<Product> products = db.Products.ToList();
            List<Category> categories = db.Categories.ToList();
            List<Customer> customer = db.Customers.ToList();
            List<Order> orders = db.Orders.ToList();

            decimal total = orders.Where(t => t.PaymentStatus == "Đã thanh toán" && t.Status == "Đã xác nhận").Sum(t => t.TotalPaymentAmout);
            double millions = Convert.ToDouble(total) / 1000000;
            
            ViewBag.productCount = products.Count;
            ViewBag.categoryCount = categories.Count;
            ViewBag.customerCount = customer.Count;
            ViewBag.orderCount = orders.Count;
            ViewBag.total = millions + "tr";
            if(millions >= 1000)
            {
                ViewBag.total = millions + "tỉ";
            }
            ViewBag.CurrentDate = DateTime.Now.ToString("MM/yyyy");
            
            ViewBag.ActiveNav = "dashboard";
            return View();
        }

        //Lấy chi thiết doanh thu tháng hiện tại
        [HttpGet]
        public JsonResult GetCurrentMonthSumarry()
        {
            string userID = User.Identity.GetUserId();

            var appDbContext = new AppDBContext();
            var userStore = new AppUserStore(appDbContext);
            var userManager = new AppUserManager(userStore);
            var userList = userManager.Users.Where(t => t.DateCreate != null && t.DateCreate.Value.Month == DateTime.Now.Month && t.DateCreate.Value.Year == DateTime.Now.Year).ToList();

            DatabaseSTechEntities db = new DatabaseSTechEntities();
            List<Order> orders = db.Orders.Where(t => t.OrderDate.Value.Month == DateTime.Now.Month && t.OrderDate.Value.Year == DateTime.Now.Year).ToList();
            
            List<Customer> customers = new List<Customer>();

            if (userList.Count > 0)
            {
                foreach (var user in userList)
                {
                    Customer cus = db.Customers.FirstOrDefault(t => t.AccountID == user.Id);
                    if (cus != null)
                    {
                        customers.Add(cus);
                    }    
                }
            }

            decimal totalRevenue = orders.Where(t => t.PaymentStatus == "Đã thanh toán"  && t.Status == "Đã xác nhận").Sum(t => t.TotalPaymentAmout);
            int totalProductSold = 0;
            if(orders.Count > 0)
            {
                foreach(var order in orders)
                {
                    List<OrderDetail> orderDetails = order.OrderDetails.ToList();
                    if(orderDetails.Count > 0 && order.PaymentStatus == "Đã thanh toán" && order.Status == "Đã xác nhận")
                    {
                        foreach(var ordDetail in orderDetails)
                        {
                            totalProductSold += ordDetail.Quantity;
                        }
                    }
                }
            }

            return Json(new { orderCount = orders.Count, revenue = totalRevenue, customerCount = customers.Count, productSold = totalProductSold, currentTime = DateTime.Now }, JsonRequestBehavior.AllowGet);
        }

        //Lấy doanh thu của 4 tháng gần nhất
        [HttpGet]
        public ActionResult GetRevenue()
        {
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            List<Order> orders = db.Orders.ToList();
            List<Order> order1, order2, order3, order4, order5, order6;
            
            DateTime sixMonthAgo = DateTime.Now.AddMonths(-6);
            order1 = orders.Where(t => Convert.ToDateTime(t.OrderDate).ToString("MM/yyyy") == sixMonthAgo.ToString("MM/yyyy")).ToList();
            order2 = orders.Where(t => Convert.ToDateTime(t.OrderDate).ToString("MM/yyyy") == sixMonthAgo.AddMonths(1).ToString("MM/yyyy")).ToList();
            order3 = orders.Where(t => Convert.ToDateTime(t.OrderDate).ToString("MM/yyyy") == sixMonthAgo.AddMonths(2).ToString("MM/yyyy")).ToList();
            order4 = orders.Where(t => Convert.ToDateTime(t.OrderDate).ToString("MM/yyyy") == sixMonthAgo.AddMonths(3).ToString("MM/yyyy")).ToList();
            order5 = orders.Where(t => Convert.ToDateTime(t.OrderDate).ToString("MM/yyyy") == sixMonthAgo.AddMonths(4).ToString("MM/yyyy")).ToList();
            order6 = orders.Where(t => Convert.ToDateTime(t.OrderDate).ToString("MM/yyyy") == sixMonthAgo.AddMonths(5).ToString("MM/yyyy")).ToList();

            decimal revenue1 = 0;
            decimal revenue2 = 0;
            decimal revenue3 = 0;
            decimal revenue4 = 0;
            decimal revenue5 = 0;
            decimal revenue6 = 0;

            if(order1.Count > 0)
            {
                revenue1 = order1.Sum(t => t.TotalPaymentAmout);
            }
            if (order2.Count > 0)
            {
                revenue2 = order2.Sum(t => t.TotalPaymentAmout);
            }
            if (order3.Count > 0)
            {
                revenue3 = order3.Sum(t => t.TotalPaymentAmout);
            }
            if (order4.Count > 0)
            {
                revenue4 = order4.Sum(t => t.TotalPaymentAmout);
            }
            if (order5.Count > 0)
            {
                revenue5 = order5.Sum(t => t.TotalPaymentAmout);
            }
            if (order6.Count > 0)
            {
                revenue6 = order6.Sum(t => t.TotalPaymentAmout);
            }

            var revenueData = new List<object>
            {
                new { month = sixMonthAgo.Date.ToString("MM/yyyy"), revenue = revenue1 },
                new { month = sixMonthAgo.AddMonths(1).Date.ToString("MM/yyyy"), revenue =  revenue2 },
                new { month = sixMonthAgo.Date.AddMonths(2).ToString("MM/yyyy"), revenue =  revenue3 },
                new { month = sixMonthAgo.Date.AddMonths(3).ToString("MM/yyyy"), revenue =  revenue4 },
                new { month = sixMonthAgo.Date.AddMonths(4).ToString("MM/yyyy"), revenue =  revenue5 },
                new { month = sixMonthAgo.Date.AddMonths(5).ToString("MM/yyyy"), revenue =  revenue6 }
            };

            return Json(new {success = true, data = revenueData }, JsonRequestBehavior.AllowGet);
        }
    }
}