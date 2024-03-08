using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using STech_Web.Models;
using STech_Web.Filters;
using System.Web.Management;
using iText.IO.Codec;

namespace STech_Web.Areas.Admin.Controllers
{
    [ManagerAuthorization]
    public class ProductsController : Controller
    {
        // GET: Admin/Products
        public ActionResult Index()
        {
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            List<Category> categories = db.Categories.OrderBy(t => t.Sort).ToList();
            List<Brand> brands = db.Brands.ToList();

            ViewBag.ActiveNav = "products";
            ViewBag.Categories = categories;
            ViewBag.Brands = brands;
            return View();
        }

        //Product detail page
        public ActionResult Detail(string id)
        {
            try
            {
                DatabaseSTechEntities db = new DatabaseSTechEntities();
                Product product = db.Products.FirstOrDefault(t => t.ProductID == id);
                if (product == null)
                {
                    return Redirect("/admin/products");
                }

                List<Category> categories = db.Categories.OrderBy(t => t.Sort).ToList();
                List<Brand> brands = db.Brands.ToList();
                List<ProductGift> gifts = db.ProductGifts.Where(t => t.ProductID == id).ToList();
                List<ProductSpecification> specs = product.ProductSpecifications.ToList();
                List<ProductContent> contents = product.ProductContents.ToList();

                ViewBag.ActiveNav = "products";
                ViewBag.Categories = categories;
                ViewBag.Brands = brands;
                ViewBag.Gifts = gifts;
                ViewBag.Specs = specs;
                ViewBag.Contents = contents;
                return View(product);
            }
            catch (Exception ex)
            {
                return Redirect("/admin/products");
            }
        }

        //Check valid value
        string checkValidValue(Product product, int quantity)
        {
            //Kiểm tra dữ liệu không để trống
            if (product.ProductID == null || product.ProductName == null || product.ImgSrc == null || product.Cost == null)
            {
                return "Vui lòng nhập đầy đủ thông tin.";
            }

            if (product.ProductID.Contains(" "))
            {
                return "Mã sản phẩm không được chứa khoảng trắng.";
            }

            //Số lượng và thời gian bảo hành >= 0
            if (quantity < 0)
            {
                return "Số lượng phải lớn hơn 0.";
            }

            if (product.Warranty < 0)
            {
                return "Thời gian bảo hành không được nhỏ hơn 0.";
            }

            //Giá không được bé hơn 0 
            if (product.Cost < 0)
            {
                return "Giá của sản phẩm phải lớn hơn 0.";
            }

            //Giá khuyến mãi < giá gốc
            if (product.Price >= product.Cost)
            {
                return "Giá khuyến mãi phải thấp hơn giá gốc.";
            }

            return null;
        }


        //Add new product --------
        [HttpPost]
        public JsonResult AddProduct(Product product, int quantity)
        {
            if (ModelState.IsValid)
            {
                string checkValue = checkValidValue(product, quantity);
                if (checkValue != null)
                {
                    return Json(new { success = false, error = checkValue });
                }

                //--------------
                DatabaseSTechEntities db = new DatabaseSTechEntities();
                List<Product> products = db.Products.ToList();
                //Kiểm tra xem sản phẩm đã tồn tại chưa
                Product pro = products.FirstOrDefault(t => t.ProductID == product.ProductID);
                if (pro != null)
                {
                    return Json(new { success = false, error = "Sản phẩm này đã tồn tại." });
                }

                //---
                if (product.Price == null)
                {
                    product.Price = product.Cost;
                }

                //---
                if (product.CateID == null)
                {
                    product.CateID = "khac";
                }
                if (product.BrandID == null)
                {
                    product.BrandID = "khac";
                }

                //---
                db.Products.Add(product);

                //---
                WareHouse wh = new WareHouse();
                wh.ProductID = product.ProductID;
                wh.Quantity = quantity;
                db.WareHouses.Add(wh);
                db.SaveChanges();
                //---
                return Json(new { success = true });
            }

            return Json(new { success = false, error = "Dữ liệu không hợp lệ." });
        }

        //Update product
        [HttpPost]
        public JsonResult UpdateProduct(Product product, int quantity)
        {
            if (ModelState.IsValid)
            {
                string checkValue = checkValidValue(product, quantity);
                if (checkValue != null)
                {
                    return Json(new { success = false, error = checkValue });
                }

                //--------------
                DatabaseSTechEntities db = new DatabaseSTechEntities();
                List<Product> products = db.Products.ToList();
                //Kiểm tra xem sản phẩm có tồn tại không
                Product pro = products.FirstOrDefault(t => t.ProductID == product.ProductID);
                if (pro == null)
                {
                    return Json(new { success = false, error = "Sản phẩm này không tồn tại." });
                }

                //------------
                if (product.Price == null)
                {
                    product.Price = product.Cost;
                }
                pro.ProductName = product.ProductName;
                pro.ImgSrc = product.ImgSrc;
                pro.ImgSrc1 = product.ImgSrc1;
                pro.ImgSrc2 = product.ImgSrc2;
                pro.ImgSrc3 = product.ImgSrc3;
                pro.ImgSrc4 = product.ImgSrc4;
                pro.ImgSrc5 = product.ImgSrc5;
                pro.ImgSrc6 = product.ImgSrc6;
                pro.ImgSrc7 = product.ImgSrc7;
                pro.Cost = product.Cost;
                pro.Price = product.Price;
                pro.CateID = product.CateID;
                pro.BrandID = product.BrandID;
                pro.Warranty = product.Warranty;
                pro.ManufacturingDate = product.ManufacturingDate;
                pro.Type = product.Type;
                pro.Description = product.Description;

                //--------------
                WareHouse wh = pro.WareHouse;
                wh.Quantity = quantity;
                db.SaveChanges();
                return Json(new { success = true });
            }

            return Json(new { success = false, error = "Dữ liệu không hợp lệ." });
        }

        //--Add product gift
        [HttpPost]
        public JsonResult AddGifts(string productID, string strGifts)
        {
            try
            {
                DatabaseSTechEntities db = new DatabaseSTechEntities();
                Product product = db.Products.FirstOrDefault(t => t.ProductID == productID);
                if (product != null)
                {
                    //Xóa dữ liệu cũ trước khi thêm dữ liệu mới vào
                    List<ProductGift> proGifts = db.ProductGifts.Where(t => t.ProductID == product.ProductID).ToList();
                    if (proGifts.Count > 0) db.ProductGifts.RemoveRange(proGifts);

                    List<string> gifts = strGifts.Split(new string[] { ";;;;;;;;" }, StringSplitOptions.None).ToList();
                    foreach (string g in gifts)
                    {
                        if (!string.IsNullOrEmpty(g))
                        {
                            ProductGift gift = new ProductGift();
                            gift.ProductID = product.ProductID;
                            gift.GiftID = g;
                            db.ProductGifts.Add(gift);
                        }
                    }
                    db.SaveChanges();
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false });
            }
        }

        //--Add product specification
        [HttpPost]
        public JsonResult AddSpecification(string productID, string specifications)
        {
            try
            {
                DatabaseSTechEntities db = new DatabaseSTechEntities();
                Product product = db.Products.FirstOrDefault(t => t.ProductID == productID);
                if (product != null)
                {
                    //Xóa dữ liệu cũ trước khi thêm dữ liệu mới vào
                    List<ProductSpecification> specs = product.ProductSpecifications.ToList();
                    if(specs.Count > 0) db.ProductSpecifications.RemoveRange(specs);

                    List<string> strContent = specifications.Split(new string[] { ";;;;;;;;" }, StringSplitOptions.None).ToList();
                    foreach (string str in strContent)
                    {
                        if (!string.IsNullOrEmpty(str))
                        {
                            string[] parts = str.Split(new string[] { "++++++++" }, StringSplitOptions.None);
                            ProductSpecification spec = new ProductSpecification();
                            spec.ProductID = product.ProductID;
                            spec.SpecName = parts[0];
                            spec.SpecContent = parts[1];
                            db.ProductSpecifications.Add(spec);
                        }
                    }
                    db.SaveChanges();
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false });
            }
        }

        //--Add product content
        [HttpPost]
        public JsonResult AddProductContent(string productID, string contents)
        {
            try 
            {
                DatabaseSTechEntities db = new DatabaseSTechEntities();
                Product product = db.Products.FirstOrDefault(t => t.ProductID == productID);
                if (product != null)
                {
                    //Xóa dữ liệu cũ trước khi thêm dữ liệu mới vào
                    List<ProductContent> pContens = product.ProductContents.ToList();
                    if (pContens.Count > 0) db.ProductContents.RemoveRange(pContens);

                    List<string> strContent = contents.Split(new string[] { ";;;;;;;;" }, StringSplitOptions.None).ToList();
                    foreach(string str in strContent)
                    {
                        if(!string.IsNullOrEmpty(str))
                        {
                            string[] parts = str.Split(new string[] { "++++++++" }, StringSplitOptions.None);
                            ProductContent content = new ProductContent();
                            content.ProductID = product.ProductID;
                            content.Title = parts[0];
                            content.Content = parts[1];
                            content.ContentImg = parts.Length > 2 ? parts[2] : "";
                            content.ContentVideo = parts.Length > 3 ? HttpUtility.UrlDecode(parts[3]) : "";
                            db.ProductContents.Add(content);
                        }
                    }
                    db.SaveChanges();
                }

                return Json(new { success = true });
            }
            catch(Exception ex)
            {
                return Json(new { success = false });
            }
        }
    }
}