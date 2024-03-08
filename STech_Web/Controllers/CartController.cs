using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using STech_Web.Models;
using STech_Web.Filters;
using Microsoft.Owin;
using Newtonsoft.Json;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.DataHandler.Encoder;
using System.Text;
using System.Data.Entity.Migrations;
using STech_Web.Identity;

namespace STech_Web.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        public ActionResult Index()
        {
            try
            {
                DatabaseSTechEntities db = new DatabaseSTechEntities();
                List<CartItem> cartCookie = new List<CartItem>();

                //Dùng để chuyển sang định dạng số có dấu phân cách phần nghìn
                CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
                ViewBag.cul = cul;


                if (User.Identity.IsAuthenticated)
                {
                    string userID = User.Identity.GetUserId();

                    cartCookie = getCartFromCookie();
                    if (cartCookie.Count > 0)
                    {
                        foreach (CartItem cartItem in cartCookie)
                        {
                            Cart cartExist = db.Carts.FirstOrDefault(t => t.ProductID == cartItem.ProductID && t.UserID == userID);
                            if (cartExist != null)
                            {
                                if (cartExist.Product.WareHouse.Quantity <= 0 || cartExist.Product.WareHouse.Quantity == null)
                                {
                                    db.Carts.Remove(cartExist);
                                }
                                else
                                {
                                    cartExist.Quantity += cartItem.Quantity;
                                    if (cartExist.Quantity >= cartExist.Product.WareHouse.Quantity)
                                    {
                                        cartExist.Quantity = (int)cartExist.Product.WareHouse.Quantity;
                                    }
                                    db.Carts.AddOrUpdate(cartExist);
                                }
                            }
                            else
                            {
                                Cart cart = new Cart();
                                cart.ProductID = cartItem.ProductID;
                                cart.Quantity = cartItem.Quantity;
                                cart.UserID = userID;
                                db.Carts.Add(cart);
                            }
                        }
                        db.SaveChanges();
                    }

                    db = new DatabaseSTechEntities();
                    List<Cart> cartItems = db.Carts.Where(t => t.UserID == userID).ToList();
                    List<Cart> cartDelete = new List<Cart>();
                    foreach (Cart cart in cartItems)
                    {
                        if (cart.Product.WareHouse.Quantity <= 0 || cart.Product.WareHouse.Quantity == null)
                        {
                            cartDelete.Add(cart);
                        }
                    }

                    if (cartDelete.Count > 0)
                    {
                        db.Carts.RemoveRange(cartDelete);
                        db.SaveChanges();
                        db = new DatabaseSTechEntities();
                        cartItems = db.Carts.Where(t => t.UserID == userID).ToList();
                    }

                    var appDbContext = new AppDBContext();
                    var userStore = new AppUserStore(appDbContext);
                    var userManager = new AppUserManager(userStore);
                    var user = userManager.FindById(User.Identity.GetUserId());
                    ViewBag.User = user;

                    //Delete cookie
                    Response.Cookies["CartItems"].Expires = DateTime.Now.AddDays(-10);
                    ViewBag.CartCount = cartItems.Count;

                    return View(cartItems);
                }
                else
                {
                    cartCookie = getCartFromCookie();
                    List<CartTemp> cartTemp = new List<CartTemp>();
                    List<CartItem> cartCCDelete = new List<CartItem>();
                    if (cartCookie.Count > 0)
                    {
                        foreach (CartItem item in cartCookie)
                        {
                            Product product = new Product();
                            int quantity = item.Quantity;
                            product = db.Products.FirstOrDefault(t => t.ProductID == item.ProductID);

                            if (product.WareHouse.Quantity <= 0 || product.WareHouse.Quantity == null)
                            {
                                cartCCDelete.Add(item);
                                continue;
                            }
                            CartTemp cTemp = new CartTemp(product, quantity);
                            cartTemp.Add(cTemp);
                        }

                        if(cartCCDelete.Count > 0)
                        {
                            foreach(CartItem item in cartCCDelete)
                            {
                                cartCookie.Remove(item);
                            }

                            saveCartToCookie(cartCookie);
                        }
                    }

                    ViewBag.CartCount = cartTemp.Count;
                    return View(cartTemp);
                }
            }
            catch (Exception ex)
            {
                return Redirect("/");
            }
        }

        //Add product to cart
        [HttpPost]
        public ActionResult AddToCart(CartItem cart)
        {
            try
            {
                DatabaseSTechEntities db = new DatabaseSTechEntities();
                Product product = db.Products.FirstOrDefault(t => t.ProductID == cart.ProductID);
                if (product == null || product.WareHouse.Quantity <= 0 || product.WareHouse.Quantity == null)
                {
                    return Json(new { success = false });
                }

                //Add data from cart to database when user logged in
                if (User.Identity.IsAuthenticated)
                {
                   
                    string userID = User.Identity.GetUserId();
                    List<Cart> userCart = db.Carts.Where(t => t.UserID == userID).ToList();

                    Cart existCart = userCart.FirstOrDefault(t => t.ProductID == cart.ProductID);
                    if (existCart != null)
                    {
                        if(existCart.Product.WareHouse.Quantity <= 0 || existCart.Product.WareHouse.Quantity == null)
                        {
                            db.Carts.Remove(existCart);
                        }
                        else
                        {
                            existCart.Quantity += 1;
                            if (existCart.Quantity >= existCart.Product.WareHouse.Quantity)
                            {
                                existCart.Quantity = (int)existCart.Product.WareHouse.Quantity;
                            }

                            db.Carts.AddOrUpdate(existCart);
                        }
                        
                        db.SaveChanges();
                    }
                    else
                    {
                        Cart cartItem = new Cart();
                        cartItem.ProductID = cart.ProductID;
                        cartItem.Quantity = cart.Quantity;
                        cartItem.UserID = userID;
                        db.Carts.Add(cartItem);
                        db.SaveChanges();
                    }

                    return Json(new { success = true });
                }
                else //Add product to cart when user not logged in
                {    //data will save to cookie
                    List<CartItem> cartItems = getCartFromCookie();

                    //----------
                    CartItem cartItem = cartItems.FirstOrDefault(t => t.ProductID == cart.ProductID);

                    if (cartItem != null)
                    {
                        cartItem.Quantity += 1;
                        int inventory = (int)db.Products.FirstOrDefault(t => t.ProductID == cartItem.ProductID).WareHouse.Quantity;
                        if (cartItem.Quantity >= inventory) cartItem.Quantity = inventory;
                    }
                    else
                    {
                        cartItems.Add(new CartItem
                        {
                            ProductID = cart.ProductID,
                            Quantity = cart.Quantity
                        });

                    }

                    saveCartToCookie(cartItems);
                    
                    return Json(new { success = true });
                }
            }
            catch (Exception ex) {
                return Redirect("/");
            }
        }

        //--Delete item from cart
        public ActionResult DeleteCartItem(int line = 0)
        {
            try
            {
                if (line > 0)
                {
                    DatabaseSTechEntities db = new DatabaseSTechEntities();
                    List<Cart> cartList = new List<Cart>();
                    //Add to cart when user logged in
                    if (User.Identity.IsAuthenticated)
                    {
                        string userID = User.Identity.GetUserId();
                        cartList = db.Carts.Where(t => t.UserID == userID).ToList();
                        if (line > cartList.Count)
                        {
                            line = cartList.Count;
                        }
                        db.Carts.Remove(cartList[line - 1]);
                        db.SaveChanges();
                    }
                    else
                    {   //Add to cart when user not logged in
                        List<CartItem> cartCookie = getCartFromCookie();
                        if (line > cartCookie.Count)
                        {
                            line = cartCookie.Count;
                        }

                        cartCookie.RemoveAt(line - 1);

                        var cartJson = JsonConvert.SerializeObject(cartCookie);
                        var bytesToEncode = Encoding.UTF8.GetBytes(cartJson);
                        var base64String = Convert.ToBase64String(bytesToEncode);
                        string json = JsonConvert.SerializeObject(cartCookie);
                        Response.Cookies["CartItems"].Value = base64String;
                        //Cookie will expire in 30 days from the date the new product is added
                        Response.Cookies["CartItems"].Expires = DateTime.Now.AddDays(30);
                    }

                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return Redirect("/cart");
            }
        }

        //--Get cart items from Cookies
        private List<CartItem> getCartFromCookie()
        {
            List<CartItem> cartItems = new List<CartItem>();
            var base64String = Request.Cookies["CartItems"]?.Value;

            if (!String.IsNullOrEmpty(base64String))
            {
                var bytesToDecode = Convert.FromBase64String(base64String);
                var cartItemsJson = Encoding.UTF8.GetString(bytesToDecode);
                cartItems = JsonConvert.DeserializeObject<List<CartItem>>(cartItemsJson);
            }

            return cartItems;
        }

        //--Save cart to cookie
        private void saveCartToCookie(List<CartItem> cartItems)
        {
            var cartJson = JsonConvert.SerializeObject(cartItems);
            var bytesToEncode = Encoding.UTF8.GetBytes(cartJson);
            var base64String = Convert.ToBase64String(bytesToEncode);

            //--Save cart item list to cookie
            Response.Cookies["CartItems"].Value = base64String;
            //Cookie will expire in 30 days from the date the new product is added
            Response.Cookies["CartItems"].Expires = DateTime.Now.AddDays(30);
        }

        //Update cart item quantity
        public ActionResult UpdateQuantity(string productID, string updateType, int qtity = 0)
        {
            try
            {
                int quantity = 0;
                decimal totalPrice = 0;
                string updateError = "";
                //Update cart item quantity when user logged in
                if (User.Identity.IsAuthenticated)
                {
                    DatabaseSTechEntities db = new DatabaseSTechEntities();
                    string userID = User.Identity.GetUserId();
                    Cart cart = db.Carts.FirstOrDefault(t => t.UserID == userID && t.ProductID == productID);

                    if (cart != null)
                    {
                        int inventory = (int)db.Products.FirstOrDefault(t => t.ProductID == cart.ProductID).WareHouse.Quantity;
                        if (updateType == "increase")
                        {
                            cart.Quantity += 1;
                            if (cart.Quantity > inventory)
                            {
                                cart.Quantity = inventory;
                                updateError = "Sản phẩm này chỉ còn " + inventory + " sản phẩm trong kho.";
                            }
                        }
                        else if (updateType == "decrease")
                        {
                            cart.Quantity -= 1;
                            if (cart.Quantity <= 0) cart.Quantity = 1;
                        }
                        else
                        {
                            if (qtity <= 0) qtity = 1;
                            if (qtity >= inventory)
                            {
                                qtity = inventory;
                                updateError = "Sản phẩm này chỉ còn " + inventory + " sản phẩm trong kho.";
                            }
                           
                            cart.Quantity = qtity;
                        }
                        quantity = cart.Quantity;
                        db.Carts.AddOrUpdate(cart);
                        db.SaveChanges();

                        db = new DatabaseSTechEntities();
                        List<Cart> userCart = db.Carts.Where(t => t.UserID == userID).ToList();
                        totalPrice = (decimal)userCart.Sum(t => t.Product.Price * t.Quantity);
                    }
                }
                else
                {   //Update cart item quantity when user not logged in
                    List<CartItem> cartCookie = getCartFromCookie();
                    CartItem cartCCItem = cartCookie.FirstOrDefault(t => t.ProductID == productID);
                    if (cartCCItem != null)
                    {
                        DatabaseSTechEntities db = new DatabaseSTechEntities();
                        int inventory = (int)db.Products.FirstOrDefault(t => t.ProductID == cartCCItem.ProductID).WareHouse.Quantity;

                        if (updateType == "increase")
                        {
                            cartCCItem.Quantity += 1;
                            if (cartCCItem.Quantity > inventory)
                            {
                                cartCCItem.Quantity = inventory;
                                updateError = "Sản phẩm này chỉ còn " + inventory + " sản phẩm trong kho.";
                            }
                        }
                        else if (updateType == "decrease")
                        {
                            cartCCItem.Quantity -= 1;
                            if (cartCCItem.Quantity <= 0) cartCCItem.Quantity = 1;
                        }
                        else
                        {
                            if (qtity <= 0) qtity = 1;
                            if (qtity >= inventory)
                            {
                                qtity = inventory;
                                updateError = "Sản phẩm này chỉ còn " + inventory + " sản phẩm trong kho.";
                            }

                            cartCCItem.Quantity = qtity;
                        }
                        quantity = cartCCItem.Quantity;
                        cartCookie.Remove(cartCCItem);
                        cartCookie.Add(cartCCItem);

                        saveCartToCookie(cartCookie);

                        //Update total price in cart page
                        List<CartTemp> cartTemp = new List<CartTemp>();
                        if (cartCookie.Count > 0)
                        {
                            foreach (CartItem item in cartCookie)
                            {
                                Product product = new Product();
                                int qty = item.Quantity;
                                product = db.Products.FirstOrDefault(t => t.ProductID == item.ProductID);

                                CartTemp cTemp = new CartTemp(product, qty);
                                cartTemp.Add(cTemp);
                            }
                        }

                        totalPrice = (decimal)cartTemp.Sum(t => t.Product.Price * t.Quantity);
                    }
                }

                return Json(new { qty = quantity, total = totalPrice, error = updateError });
            }
            catch (Exception ex)
            {
                return Redirect("/cart");
            }
        }

        //Count item in cart
        [HttpPost]
        public JsonResult CartCount()
        {
            //Count item in cart when user logged in
            if(User.Identity.IsAuthenticated)
            {
                DatabaseSTechEntities db = new DatabaseSTechEntities();
                string userID = User.Identity.GetUserId();
                int cartCount = db.Carts.Where(t => t.UserID == userID).Count();

                return Json(new { count = cartCount });
            }
            else
            {   //Count item in cart when user not logged in
                List<CartItem> cartItems = getCartFromCookie();
                int cartCount = cartItems.Count();

                return Json(new { count = cartCount });
            }
        }

    }
}