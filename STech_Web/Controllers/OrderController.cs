using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using STech_Web.Models;
using STech_Web.Filters;
using Microsoft.AspNet.Identity;
using System.Web.Management;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using STech_Web.Identity;
using System.Globalization;
using System.Text;
using Stripe;
using Stripe.Checkout;
using System.Web.Http.Results;
using System.Net;
using System.Data.Entity.Migrations;
using PayPal.Api;
using System.Web.SessionState;

namespace STech_Web.Controllers
{
    [UserAuthorization]
    public class OrderController : Controller
    {
        //Dùng để chuyển sang định dạng số có dấu phân cách phần nghìn
        CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");

        // GET: Order
        public ActionResult OrderInfo()
        {
            var base64String = Request.Cookies["OrderTemp"]?.Value;
            OrderTemp orderTemp = new OrderTemp();

            if (!String.IsNullOrEmpty(base64String))
            {
                var bytesToDecode = Convert.FromBase64String(base64String);
                var orderTempJson = Encoding.UTF8.GetString(bytesToDecode);
                orderTemp = JsonConvert.DeserializeObject<OrderTemp>(orderTempJson);

                if (orderTemp == null)
                {
                    return Redirect("/cart");
                }
            }
            else
            {
                return Redirect("/cart");
            }

            //--------------------
            var appDbContext = new AppDBContext();
            var userStore = new AppUserStore(appDbContext);
            var userManager = new AppUserManager(userStore);
            var user = userManager.FindById(User.Identity.GetUserId());

            ViewBag.User = user;
            ViewBag.Order = orderTemp;

            //----------------------
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            string userID = User.Identity.GetUserId();
            List<Cart> cart = db.Carts.Where(t => t.UserID == userID).ToList();
            decimal totalPrice = (decimal)cart.Sum(t => t.Quantity * t.Product.Price);

            ViewBag.TotalPrice = totalPrice.ToString("##,###", cul);

            return View();
        }

        //Lấy domain hiện tại
        public string getCurrentDomain()
        {
            var url = HttpContext.Request.Url;

            if (url != null)
            {
                var scheme = url.Scheme;
                var host = url.Host;
                var port = url.Port;
                var domain = $"{scheme}://{host}:{port}";

                return domain;
            }

            return null;
        }

        //Thanh toán
        [HttpPost]
        public ActionResult CheckOut(string paymentMethod)
        {
            try
            {
                if (paymentMethod == "paypal") //Thanh toán bằng Paypal
                {
                    return Json(new { url = "/order/paymentwithpaypal" });
                }
                //--
                string userID = User.Identity.GetUserId();
                DatabaseSTechEntities db = new DatabaseSTechEntities();
                List<Cart> cart = db.Carts.Where(t => t.UserID == userID).ToList();

                if (cart.Count <= 0)
                {
                    return Json(new { url = "/cart" });
                }

                //Kiểm tra có sản phảm nào hết hàng không
                List<string> errors = new List<string>();
                int coutProductOutOfStock = 0;
                foreach (Cart c in cart)
                {
                    if (c.Product.WareHouse.Quantity <= 0)
                    {
                        coutProductOutOfStock += 1;
                        string err = "Sản phẩm " + c.Product.ProductName + " đã hết hàng.";
                        errors.Add(err);
                    }
                }

                if (coutProductOutOfStock > 0)
                {
                    return Json(new { success = false, error = errors });
                }

                //--Lấy thông tin đơn hàng đã lưu tạm vào cookie
                var base64String = Request.Cookies["OrderTemp"]?.Value;
                OrderTemp orderTemp = new OrderTemp();

                if (!String.IsNullOrEmpty(base64String))
                {
                    var bytesToDecode = Convert.FromBase64String(base64String);
                    var orderTempJson = Encoding.UTF8.GetString(bytesToDecode);
                    orderTemp = JsonConvert.DeserializeObject<OrderTemp>(orderTempJson);
                    if (orderTemp == null)
                    {
                        return Redirect("/cart");
                    }
                }

                //Tạo khách hàng mới nếu khách hàng này chưa tồn tại
                STech_Web.Models.Customer customer = db.Customers.FirstOrDefault(t => t.AccountID == userID);
                if (customer == null)
                {
                    addNewCustomer(db, userID);
                    db = new DatabaseSTechEntities();
                }

                //Tạo đơn hàng
                List<STech_Web.Models.Order> orders = db.Orders.OrderByDescending(t => t.OrderID).ToList();
                int orderNumber = 1;
                if (orders.Count > 0)
                {
                    orderNumber = int.Parse(orders[0].OrderID.Substring(2)) + 1;
                }

                string orderID = "DH" + orderNumber.ToString().PadLeft(5, '0');
                decimal totalPrice = (decimal)cart.Sum(t => t.Quantity * t.Product.Price);
                STech_Web.Models.Customer customer1 = db.Customers.FirstOrDefault(t => t.AccountID == userID);

                STech_Web.Models.Order order = new STech_Web.Models.Order();
                order.OrderID = orderID;
                order.CustomerID = customer1.CustomerID;
                order.OrderDate = DateTime.Now;
                order.Note = orderTemp.Note;
                order.ShipMethod = orderTemp.ShipMethod;
                order.DeliveryFee = 0;
                order.TotalPrice = totalPrice;
                order.TotalPaymentAmout = totalPrice + (decimal)order.DeliveryFee;
                order.PaymentStatus = "Chờ thanh toán";
                order.Status = "Chờ xác nhận";
                order.ShipAddress = customer1.Address;

                //Tạo chi tiết đơn hàng
                List<OrderDetail> orderDetails = new List<OrderDetail>();
                foreach (Cart c in cart)
                {
                    OrderDetail orderDetail = new OrderDetail();
                    orderDetail.OrderID = orderID;
                    orderDetail.ProductID = c.ProductID;
                    orderDetail.Quantity = c.Quantity;
                    orderDetail.Price = c.Product.Price;

                    orderDetails.Add(orderDetail);

                    //Trừ số lượng khỏi kho
                    WareHouse wh = c.Product.WareHouse;
                    wh.Quantity -= c.Quantity;
                }


                //--------------------------------------------------------------------------------
                if (paymentMethod == "COD") //Thanh toán khi nhận hàng
                {
                    //--
                    order.PaymentMethod = "Thanh toán khi nhận hàng";
                    db.Orders.Add(order);
                    db.OrderDetails.AddRange(orderDetails);
                    db.Carts.RemoveRange(cart);
                    db.SaveChanges();
                    //--
                    return Json(new { url = "/order/succeeded" });
                }
                else if (paymentMethod == "card") //Thanh toán bằng thẻ visa/mastercard
                {
                    //Lấy domain hiện tại
                    var domain = getCurrentDomain();

                    //Stripe api key
                    StripeConfiguration.ApiKey = "sk_test_51O7hGcASuMBoFWl8pQdaMvQaPYFY13MjLln9m2w2oQ41K5JuagkbAJLJmQ8pULQ48ebIgYx9RKCZeAT575F3qoVR00tx24Pnvt";

                    // Số tiền cần thanh toán => đổi sang USD (đơn vị là cents - 1 USD  = 100 cents)
                    var amount = ((long)Math.Round(totalPrice / 24000)) * 100;
                    //-------

                    var options = new SessionCreateOptions
                    {
                        PaymentMethodTypes = new List<string> { "card" },
                        LineItems = new List<SessionLineItemOptions>
                    {
                        new SessionLineItemOptions
                        {
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                Currency = "usd",
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = "Thanh toán đơn hàng " + orderID,
                                },
                                UnitAmount = amount,
                            },
                            Quantity = 1,
                        }
                    },
                        Metadata = new Dictionary<string, string> { { "order_id", orderID } },
                        Mode = "payment",
                        SuccessUrl = domain + "/order/updatepaymentstatus",
                        CancelUrl = domain + "/order/failed"

                    };

                    var service = new SessionService();
                    var session = service.Create(options);
                    TempData["Session"] = session.Id;

                    //Response.Headers.Add("Location", session.Url);

                    order.PaymentMethod = "Visa/Mastercard";
                    db.Orders.Add(order);
                    db.OrderDetails.AddRange(orderDetails);
                    db.Carts.RemoveRange(cart);
                    db.SaveChanges();

                    return Json(new { url = session.Url });


                }
                else
                {
                    return Redirect("/order/orderinfo");
                }
            }
            catch (Exception ex)
            {
                return Redirect("/error");
            }
        }

        //--Add new customer ---------------
        public string addNewCustomer(DatabaseSTechEntities db, string userID)
        {

            var appDbContext = new AppDBContext();
            var userStore = new AppUserStore(appDbContext);
            var userManager = new AppUserManager(userStore);
            var user = userManager.FindById(userID);

            STech_Web.Models.Customer customer = new STech_Web.Models.Customer();
           List<STech_Web.Models.Customer> customers = db.Customers.OrderByDescending(t => t.CustomerID).ToList();
            int customerNumber = 1;
            if(customers.Count > 0)
            {
                customerNumber = int.Parse(customers[0].CustomerID.Substring(2)) + 1;
            }

            string customerID = "KH" + customerNumber.ToString().PadLeft(4, '0');

            customer = new STech_Web.Models.Customer();
            customer.AccountID = userID;
            customer.CustomerID = customerID;
            customer.CustomerName = user.UserFullName;
            customer.Address = user.Address;
            customer.Phone = user.PhoneNumber;
            customer.Email = user.Email;
            customer.DoB = user.DOB;
            customer.Gender = user.Gender;

            db.Customers.Add(customer);
            db.SaveChanges();

            return customerID;

        }

        //Check order infomation 
        [HttpPost]
        public ActionResult CheckOrderInfo(string gender, string customerName, string customerPhone, string address, string shipMethod, string note)
        {
            //Không được để trống thông tin
            if (String.IsNullOrEmpty(gender) || String.IsNullOrEmpty(customerName) || String.IsNullOrEmpty(customerPhone) || String.IsNullOrEmpty(shipMethod))
            {
                return Json(new { success = false, error = "Vui lòng nhập đầy đủ thông tin." });
            }

            //Kiểm tra số điện thoại
            if (!(customerPhone.StartsWith("0")) || customerPhone.Length != 10 || !Regex.IsMatch(customerPhone, @"^[0-9]+$"))
            {
                string err = "Số điện thoại không hợp lệ.";
                return Json(new { success = false, error = err });
            }

            //Kiểm tra địa chỉ nhận hàng nếu chọn COD
            if (shipMethod == "COD")
            {
                if (String.IsNullOrEmpty(address))
                {
                    string err = "Vui lòng nhập địa chỉ nhận hàng.";
                    return Json(new { success = false, error = err });
                }
            }

            //Tạo đơn hàng nháp
            OrderTemp orderTemp = new OrderTemp(shipMethod, note);
            var orderTempJson = JsonConvert.SerializeObject(orderTemp);
            var bytesToEncode = Encoding.UTF8.GetBytes(orderTempJson);
            var base64String = Convert.ToBase64String(bytesToEncode);

            //--Save cart item list to cookie
            Response.Cookies["OrderTemp"].Value = base64String;
            //Cookie will expire in 15 minutes from the date the new product is added
            Response.Cookies["OrderTemp"].Expires = DateTime.Now.AddMinutes(15);

            return Json(new { success = true });
        }

        //Thanh toán bằng paypal
        public ActionResult PaymentWithPaypal(string Cancel = null)
        {
            //getting the apiContext  
            APIContext apiContext = PaypalConfiguration.GetAPIContext();
            //--
            string userID = User.Identity.GetUserId();
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            List<Cart> cart = db.Carts.Where(t => t.UserID == userID).ToList();

            //--Lấy thông tin đơn hàng đã lưu tạm vào cookie
            var base64String = Request.Cookies["OrderTemp"]?.Value;
            OrderTemp orderTemp = new OrderTemp();

            if (!String.IsNullOrEmpty(base64String))
            {
                var bytesToDecode = Convert.FromBase64String(base64String);
                var orderTempJson = Encoding.UTF8.GetString(bytesToDecode);
                orderTemp = JsonConvert.DeserializeObject<OrderTemp>(orderTempJson);
                if (orderTemp == null)
                {
                    return Redirect("/cart");
                }
            }

            //Tạo khách hàng mới nếu khách hàng này chưa tồn tại
            STech_Web.Models.Customer customer = db.Customers.FirstOrDefault(t => t.AccountID == userID);
            if (customer == null)
            {
                addNewCustomer(db, userID);
                db = new DatabaseSTechEntities();
            }

            //Tạo đơn hàng
            List<STech_Web.Models.Order> orders = db.Orders.OrderByDescending(t => t.OrderID).ToList();
            int orderNumber = 1;
            if (orders.Count > 0)
            {
                orderNumber = int.Parse(orders[0].OrderID.Substring(2)) + 1;
            }
            string orderID = "DH" + orderNumber.ToString().PadLeft(5, '0');

            decimal totalPrice = (decimal)cart.Sum(t => t.Quantity * t.Product.Price);
            STech_Web.Models.Customer customer1 = db.Customers.FirstOrDefault(t => t.AccountID == userID);

            STech_Web.Models.Order order = new STech_Web.Models.Order();
            order.OrderID = orderID;
            order.CustomerID = customer1.CustomerID;
            order.OrderDate = DateTime.Now;
            order.Note = orderTemp.Note;
            order.ShipMethod = orderTemp.ShipMethod;
            order.PaymentMethod = "Paypal";
            order.DeliveryFee = 0;
            order.TotalPrice = totalPrice;
            order.TotalPaymentAmout = totalPrice + (decimal)order.DeliveryFee;
            order.PaymentStatus = "Chờ thanh toán";
            order.Status = "Chờ xác nhận";
            order.ShipAddress = customer1.Address;

            //Tạo chi tiết đơn hàng
            List<OrderDetail> orderDetails = new List<OrderDetail>();
            foreach (Cart c in cart)
            {
                OrderDetail orderDetail = new OrderDetail();
                orderDetail.OrderID = orderID;
                orderDetail.ProductID = c.ProductID;
                orderDetail.Quantity = c.Quantity;
                orderDetail.Price = c.Product.Price;

                orderDetails.Add(orderDetail);

                //Trừ số lượng khỏi kho
                WareHouse wh = c.Product.WareHouse;
                wh.Quantity -= c.Quantity;
            }

            try
            {
                //A resource representing a Payer that funds a payment Payment Method as paypal  
                //Payer Id will be returned when payment proceeds or click to pay  
                string payerId = Request.Params["PayerID"];
                if (string.IsNullOrEmpty(payerId))
                {
                    //this section will be executed first because PayerID doesn't exist  
                    //it is returned by the create function call of the payment class  
                    // Creating a payment  
                    // baseURL is the url on which paypal sendsback the data.  
                    string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority + "/order/PaymentWithPayPal?";
                    //here we are generating guid for storing the paymentID received in session  
                    //which will be used in the payment execution  
                    var guid = Convert.ToString((new Random()).Next(100000));
                    //CreatePayment function gives us the payment approval url  
                    //on which payer is redirected for paypal account payment  
                    var createdPayment = CreatePayment(apiContext, baseURI + "guid=" + guid, orderID, Math.Round(totalPrice / 24000).ToString());
                    //get links returned from paypal in response to Create function call  
                    var links = createdPayment.links.GetEnumerator();
                    string paypalRedirectUrl = null;
                    while (links.MoveNext())
                    {
                        Links lnk = links.Current;
                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            //saving the payapalredirect URL to which user will be redirected for payment  
                            paypalRedirectUrl = lnk.href;
                        }
                    }
                    // saving the paymentID in the key guid  
                    Session.Add(guid, createdPayment.id);
                    return Redirect(paypalRedirectUrl);
                }
                else
                {
                    // This function exectues after receving all parameters for the payment  
                    var guid = Request.Params["guid"];
                    var executedPayment = ExecutePayment(apiContext, payerId, Session[guid] as string);
                    //If executed payment failed then we will show payment failure message to user  
                    if (executedPayment.state.ToLower() != "approved")
                    {
                        order.PaymentStatus = "Thanh toán thất bại";
                        db.Orders.Add(order);
                        db.OrderDetails.AddRange(orderDetails);
                        db.Carts.RemoveRange(cart);
                        db.SaveChanges();

                        return Redirect("/order/failed");
                    }
                }
            }
            catch (Exception ex)
            {
                order.PaymentStatus = "Thanh toán thất bại";
                db.Orders.Add(order);
                db.OrderDetails.AddRange(orderDetails);
                db.Carts.RemoveRange(cart);
                db.SaveChanges();

                return Redirect("/order/failed");
            }
            order.PaymentStatus = "Đã thanh toán";
            db.Orders.Add(order);
            db.OrderDetails.AddRange(orderDetails);
            db.Carts.RemoveRange(cart);
            db.SaveChanges();
            //on successful payment, show success page to user.  
            return Redirect("/order/succeeded");
        }
        private PayPal.Api.Payment payment;
        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution()
            {
                payer_id = payerId
            };
            this.payment = new Payment()
            {
                id = paymentId
            };
            return this.payment.Execute(apiContext, paymentExecution);
        }
        private Payment CreatePayment(APIContext apiContext, string redirectUrl, string orderID, string totalPrice)
        {
            //create itemlist and add item objects to it  
            var itemList = new ItemList()
            {
                items = new List<Item>()
            };
            //Adding Item Details like name, currency, price etc  
            itemList.items.Add(new Item()
            {
                name = "Pay for " + orderID,
                currency = "USD",
                price = totalPrice,
                quantity = "1",
                sku = orderID
            });
            var payer = new Payer()
            {
                payment_method = "paypal"
            };
            // Configure Redirect Urls here with RedirectUrls object  
            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl + "&Cancel=true",
                return_url = redirectUrl
            };
            // Adding Tax, shipping and Subtotal details  
            var details = new Details()
            {
                tax = "0",
                shipping = "0",
                subtotal = totalPrice
            };
            //Final amount with details  
            var amount = new Amount()
            {
                currency = "USD",
                total = totalPrice, // Total must be equal to sum of tax, shipping and subtotal.  
                details = details
            };
            var transactionList = new List<Transaction>();
            // Adding description about the transaction  
            var paypalOrderId = DateTime.Now.Ticks;
            transactionList.Add(new Transaction()
            {
                description = $"Invoice #{paypalOrderId}",
                invoice_number = paypalOrderId.ToString(), //Generate an Invoice No    
                amount = amount,
                item_list = itemList
            });
            this.payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };
            // Create a payment using a APIContext  
            return payment.Create(apiContext);
        }

        //Thanh toán thành công
        public ActionResult Succeeded()
        {
            return View();
        }

        //Thanh toán thất bại
        public ActionResult Failed()
        {

            return View();
        }

        //Cập nhật trạng thái giao dịch  - Stripe
        public ActionResult UpdatePaymentStatus()
        {
            var service = new SessionService();
            var session = service.Get(TempData["Session"].ToString());
            string orderID = session.Metadata["order_id"];
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            STech_Web.Models.Order order = db.Orders.FirstOrDefault(t => t.OrderID == orderID);

            if (session.PaymentStatus == "paid")
            {
                order.PaymentStatus = "Đã thanh toán";
                db.Orders.AddOrUpdate(order);
                db.SaveChanges();

                return RedirectToAction("Succeeded");
            }

            order.PaymentStatus = "Thanh toán thất bại";
            db.Orders.AddOrUpdate(order);
            db.SaveChanges();

            return RedirectToAction("Failed");
        }

        //Kiểm tra chi tiết đơn hàng
        public ActionResult Detail(string id)
        {
            try
            {
                string userID = User.Identity.GetUserId();
                DatabaseSTechEntities db = new DatabaseSTechEntities();
                STech_Web.Models.Order order = db.Orders.FirstOrDefault(t => t.OrderID == id && t.Customer.AccountID == userID);

                if (order == null)
                {
                    return Redirect("/account#orders");
                }

                //Tạo danh sách Breadcrumb
                List<Breadcrumb> breadcrumb = new List<Breadcrumb>();
                breadcrumb.Add(new Breadcrumb("Trang chủ", "/"));
                breadcrumb.Add(new Breadcrumb("Đơn hàng", "/account#orders"));
                breadcrumb.Add(new Breadcrumb("Chi tiết " + id, ""));

                //Dùng để chuyển sang định dạng số có dấu phân cách phần nghìn
                CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");

                //Lấy chi tiết đơn hàng
                List<OrderDetail> orderDetail = order.OrderDetails.ToList();

                //Lấy thông tin khách hàng
                STech_Web.Models.Customer customer = order.Customer;

                ViewBag.Breadcrumb = breadcrumb;
                ViewBag.Order = order;
                ViewBag.Customer = customer;
                ViewBag.cul = cul;
                return View(orderDetail);
            }
            catch (Exception ex)
            {

                return Redirect("/error/notfound");
            }

        }

        //In hóa đơn
        public ActionResult PrintOrder(string orderID)
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    string userID = User.Identity.GetUserId();
                    DatabaseSTechEntities db = new DatabaseSTechEntities();
                    STech_Web.Models.Customer customer = db.Customers.FirstOrDefault(t => t.AccountID == userID);
                    if (customer == null)
                    {
                        return Redirect("#");
                    }

                    STech_Web.Models.Order order = customer.Orders.FirstOrDefault(t => t.OrderID == orderID);
                    if (order == null)
                    {
                        return Redirect("#");
                    }

                    PrintInvoice printInvoice = new PrintInvoice(order);
                    byte[] file = printInvoice.Print();

                    return File(file, printInvoice.ContentType, printInvoice.FileName);
                }
                else
                {
                    return Redirect("#");
                }

            }
            catch (Exception ex)
            {
                return Redirect("#");
            }
        }

        //Xóa hóa đơn có trạng thái chờ thanh toán
        public ActionResult Delete(string orderID)
        {
            try
            {
                string userID = User.Identity.GetUserId();
                DatabaseSTechEntities db = new DatabaseSTechEntities();
                STech_Web.Models.Customer customer = db.Customers.FirstOrDefault(t => t.AccountID == userID);
                STech_Web.Models.Order order = customer.Orders.FirstOrDefault(t => t.OrderID == orderID);

                if (order != null && order.PaymentStatus == "Chờ thanh toán" && order.Status == "Chờ xác nhận")
                {
                    List<OrderDetail> orderDetails = order.OrderDetails.ToList();
                    //Cập nhật lại số lượng của sản phẩm
                    foreach(OrderDetail orderDetail in orderDetails)
                    {
                        WareHouse wh = orderDetail.Product.WareHouse;
                        wh.Quantity += orderDetail.Quantity;
                    }

                    db.OrderDetails.RemoveRange(orderDetails);
                    db.Orders.Remove(order);
                    db.SaveChanges();
                }
            }
            catch (Exception ex) { }
            return Redirect("/account#orders");
        }

        //Tìm hóa đơn
        [HttpPost]
        public ActionResult SearchOrder(string orderID)
        {
            try
            {
                string userID = User.Identity.GetUserId();
                List<OrderAPI> orderAPI = new List<OrderAPI>();

                DatabaseSTechEntities db = new DatabaseSTechEntities();
                STech_Web.Models.Customer customer = db.Customers.FirstOrDefault(t => t.AccountID == userID);
                List<STech_Web.Models.Order> orders = customer.Orders.Where(t => t.OrderID.Contains(orderID)).ToList();

                if(customer == null)
                {
                    return Json(new { orders = orderAPI });
                }

                foreach (var order in orders)
                {
                    orderAPI.Add(new OrderAPI(order.Customer.CustomerID, order.Customer.CustomerName, order.OrderID, (DateTime)order.OrderDate, order.TotalPrice, order.PaymentStatus, order.Status, order.Note, (Decimal)order.DeliveryFee, order.TotalPaymentAmout, order.ShipMethod, order.PaymentMethod, order.ShipAddress));
                }

                orderAPI = orderAPI.OrderByDescending(t => t.OrderDate).ToList();
                return Json(new { orders = orderAPI });
            }
            catch (Exception ex)
            {
                return Redirect("/account#orders");
            }
        }

        //Lấy danh sách hóa đơn theo trạng thái
        [HttpPost]
        public ActionResult GetOrder(string status)
        {
            try
            {
                string userID = User.Identity.GetUserId();
                DatabaseSTechEntities db = new DatabaseSTechEntities();
                Models.Customer customer = db.Customers.FirstOrDefault(t => t.AccountID == userID);
                List<Models.Order> orders = new List<Models.Order>();
                List<OrderAPI> orderAPI = new List<OrderAPI>();

                if(customer == null)
                {
                    return Json(new { orders = orderAPI });
                }

                switch (status)
                {
                    case "all":
                        orders = customer.Orders.ToList();
                        break;
                    case "new":
                        orders = customer.Orders.Where(t => t.OrderDate >= DateTime.Now.AddDays(-2)).ToList();
                        break;
                    case "wait-for-pay":
                        orders = customer.Orders.Where(t => t.PaymentStatus == "Chờ thanh toán").ToList();
                        break;
                    case "paid":
                        orders = customer.Orders.Where(t => t.PaymentStatus == "Đã thanh toán").ToList();
                        break;
                }

                foreach (var order in orders)
                {
                    orderAPI.Add(new OrderAPI(order.Customer.CustomerID, order.Customer.CustomerName, order.OrderID, (DateTime)order.OrderDate, order.TotalPrice, order.PaymentStatus, order.Status, order.Note, (Decimal)order.DeliveryFee, order.TotalPaymentAmout, order.ShipMethod, order.PaymentMethod, order.ShipAddress));
                }

                orderAPI = orderAPI.OrderByDescending(t => t.OrderDate).ToList();
                return Json(new { orders = orderAPI });
            }
            catch (Exception ex)
            {
                return Redirect("/account#orders");
            }
        }
    }
}