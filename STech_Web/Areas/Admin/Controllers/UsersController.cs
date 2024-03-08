using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using STech_Web.Filters;
using STech_Web.Identity;
using STech_Web.Models;
using STech_Web.ViewModel;

namespace STech_Web.Areas.Admin.Controllers
{
    [AdminAuthorization]
    public class UsersController : Controller
    {
        // GET: Admin/Users
        public ActionResult Index()
        {
            var appDbContext = new AppDBContext();
            var userStore = new AppUserStore(appDbContext);
            var userManager = new AppUserManager(userStore);
            List<AppUser> users = userManager.Users.ToList();

            ViewBag.ActiveNav = "users";
            return View(users);
        }

        //Kiểm tra SDT đã tồn tại trong Identity User
        private bool CheckUserPhoneExist(string phone, string username)
        {
            var appDbContext = new AppDBContext();
            var userStore = new AppUserStore(appDbContext);
            var userManager = new AppUserManager(userStore);
            var allUsers = userManager.Users.ToList();

            if (allUsers.Any(t => t.UserName != username && t.PhoneNumber == phone))
            {
                return true;
            }

            return false;
        }
        //Kiểm tra Email đã tồn tại trong Identity User
        private bool CheckUserEmailExist(string email, string username)
        {
            var appDbContext = new AppDBContext();
            var userStore = new AppUserStore(appDbContext);
            var userManager = new AppUserManager(userStore);
            var allUsers = userManager.Users.ToList();

            if (allUsers.Any(t => t.UserName != username && t.Email == email))
            {
                return true;
            }

            return false;
        }

        //Kiểm tra SDT đã tồn tại trong bảng Customer
        private bool checkCustomerPhoneExist(string phone)
        {
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            List<Customer> customers = db.Customers.ToList();

            if(customers.Count > 0 && customers.Any(c => c.Phone == phone))
            {
                return true;
            }

            return false;
        }

        //Kiểm tra Email đã tồn tại trong bảng Customer
        private bool checkCustomerEmailExist(string email) 
        {
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            List<Customer> customers = db.Customers.ToList();

            if (customers.Count > 0 && customers.Any(c => c.Email == email))
            {
                return true;
            }

            return false;
        }

        //Tạo tài khoản

        //Tạo khách hàng
        [HttpPost]
        public JsonResult CreateCustomer(Customer cus)
        {
            try
            {
                if(checkCustomerPhoneExist(cus.Phone))
                {
                    return Json(new { success = false, error = "Số điện thoại này đã tồn tại. " });
                }
                if(checkCustomerEmailExist(cus.Email))
                {
                    return Json(new { success = false, error = "Email này đã tồn tại. " });
                }

                DatabaseSTechEntities db = new DatabaseSTechEntities();
                Customer customer = new Customer();
                List<Customer> customers = db.Customers.OrderByDescending(t => t.CustomerID).ToList();

                int customerNumber = 1;
                if (customers.Count > 0)
                {
                    customerNumber = int.Parse(customers[0].CustomerID.Substring(2)) + 1;
                }

                string customerID = "KH" + customerNumber.ToString().PadLeft(4, '0');

                customer = new Customer();
                customer.CustomerID = customerID;
                customer.CustomerName = cus.CustomerName;
                customer.Address = cus.Address;
                customer.Phone = cus.Phone;
                customer.Email = cus.Email;
                customer.DoB = cus.DoB;
                customer.Gender = cus.Gender;

                db.Customers.Add(customer);
                db.SaveChanges();
                return Json(new { success = true }); 
            }
            catch(Exception ex)
            {
                return Json(new { success = false });
            }         
        }

        //Cập nhật thông tin tài khoản
        [HttpPost]
        public ActionResult Update(UpdateUserVM update, string userName)
        {
            var appDbContext = new AppDBContext();
            var userStore = new AppUserStore(appDbContext);
            var userManager = new AppUserManager(userStore);
            var user = userManager.FindByName(userName);

            if (user != null)
            {
                DateTime oldDate = DateTime.Parse("1930/01/01");
                if (update.DOB > DateTime.Now || update.DOB <= oldDate || CheckUserEmailExist(update.Email, userName) || CheckUserPhoneExist(update.PhoneNumber, userName))
                {
                    return Redirect("/admin/users");
                }

                //Kiểm tra số điện thoại
                if (update.PhoneNumber == null || !(update.PhoneNumber.StartsWith("0")) || update.PhoneNumber.Length != 10 || !Regex.IsMatch(update.PhoneNumber, @"^[0-9]+$"))
                {
                    return Redirect("/admin/users");
                }

                //------------------------
                if (user.DOB != null && update.DOB == null)
                {
                    update.DOB = user.DOB;
                }

                if (user.Email.Length > 0 && update.Email == null)
                {
                    update.Email = user.Email;
                }

                //-----------------------
                user.UserFullName = update.UserFullName;
                user.Gender = update.Gender;
                user.PhoneNumber = update.PhoneNumber;
                user.Email = update.Email;
                user.DOB = update.DOB;
                user.Address = update.Address;

                var updateCheck = userManager.Update(user);
                if (updateCheck.Succeeded)
                {
                    DatabaseSTechEntities db = new DatabaseSTechEntities();
                    Customer customer = db.Customers.FirstOrDefault(c => c.AccountID == user.Id);
                    if (customer != null)
                    {
                        customer.CustomerName = user.UserFullName;
                        customer.Gender = user.Gender;
                        customer.Phone = user.PhoneNumber;
                        customer.Email = user.Email;
                        customer.DoB = user.DOB;
                        customer.Address = user.Address;

                        db.SaveChanges();
                    }
                    return Redirect("/admin/users");
                }
            }
            return Redirect("/admin/users");
        }

        //Tạo tài khoản ------------------------------
        [HttpPost]
        public JsonResult CreateAccount(RegisterVM register)
        {
            if (ModelState.IsValid)
            {
                var appDbContext = new AppDBContext();
                var userStore = new AppUserStore(appDbContext);
                var userManager = new AppUserManager(userStore);
                var passwordHash = Crypto.HashPassword(register.ResPassword);
                var user = new AppUser()
                {
                    Email = register.Email,
                    UserName = register.ResUsername,
                    PasswordHash = passwordHash,
                    DateCreate = DateTime.Now
                };

                var existingUser = userManager.FindByName(register.ResUsername);
                var existingUserEmail = userManager.FindByEmail(register.Email);

                //Kiểm tra xem user đã tồn tại chưa
                if (existingUser != null)
                {
                    return Json(new { success = false, error = "Tài khoản nãy đã tồn tại." });
                }

                //Kiểm tra xem email đã tồn tại chưa
                if (existingUserEmail != null)
                {
                    return Json(new { success = false, error = "Email này đã tồn tại." });
                }

                IdentityResult identityResult = userManager.Create(user);

                if (identityResult.Succeeded)
                {
                    userManager.AddToRole(user.Id, "Customer");
                }
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false, error = "Lỗi" });
            }
        }
    }
}