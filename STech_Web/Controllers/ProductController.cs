using STech_Web.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace STech_Web.Controllers
{
    public class ProductController : Controller
    {
        // GET: Product
        public ActionResult Detail(string id="")
        {
            try
            {
                if (id.Length > 0)
                {
                    DatabaseSTechEntities db = new DatabaseSTechEntities();
                    Product product = db.Products.Where(t => t.ProductID == id)
                        .FirstOrDefault();

                    Category cate = db.Categories.Where(t => t.CateID == product.CateID)
                        .FirstOrDefault();

                    List<Product> products = db.Products.Where(t => t.CateID == product.CateID && t.CateID != "khac").OrderBy(t => Guid.NewGuid()).Take(15).ToList();

                    List<ProductGift> pGitfs = db.ProductGifts.Where(t => t.ProductID == product.ProductID).ToList();

                    List<Breadcrumb> breadcrumb = new List<Breadcrumb>();
                    breadcrumb.Add(new Breadcrumb("Trang chủ", "/"));
                    breadcrumb.Add(new Breadcrumb(cate.CateName, "/collections/" + cate.CateID + ""));
                    breadcrumb.Add(new Breadcrumb(product.ProductName, ""));

                    ViewBag.Title = product.ProductName;
                    ViewBag.Breadcrumb = breadcrumb;
                    ViewBag.Products = products;
                    ViewBag.ProductGifts = pGitfs;

                    //Dùng để chuyển sang định dạng số có dấu phân cách phần nghìn
                    CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
                    ViewBag.cul = cul;

                    return View(product);
                }

                return Redirect("/error/notfound");
            }
            catch (Exception ex)
            {
                return Redirect("/error/notfound");
            }
        }
    }
}