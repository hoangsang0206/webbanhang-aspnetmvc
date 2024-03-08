using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Microsoft.Owin.Security;
using STech_Web.Filters;
using STech_Web.Models;

namespace STech_Web.Areas.Admin.Controllers
{
    [ManagerAuthorization]
    public class SaleController : Controller
    {
        // GET: Admin/Sale
        public ActionResult Index()
        {
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            List<Sale> sales = db.Sales.ToList();

            ViewBag.ActiveNav = "sale";
            return View(sales);
        }

        //Tạo mới khuyến mãi
        public ActionResult Create()
        {
            ViewBag.ActiveNav = "sale";
            return View();
        }

        [HttpPost]
        public JsonResult Create(Sale sale, string saleDetails)
        {
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            try
            {
                if (sale.StartTime >= sale.EndTime || sale.EndTime <= DateTime.Now)
                    return Json(new { success = false, error = "Thời gian khuyến mãi không hợp lệ." });

                sale.Status = "Chờ";
                db.Sales.Add(sale);
                db.SaveChanges();

                db = new DatabaseSTechEntities();

                Sale addedSale = db.Sales.FirstOrDefault(t => t.StartTime == sale.StartTime && t.EndTime == sale.EndTime && t.Status == "Chờ");

                List<string> strSale = saleDetails.Split(new string[] { ";;;;;;;;" }, StringSplitOptions.None).ToList();
                foreach (string str in strSale)
                {
                    if (!string.IsNullOrEmpty(str))
                    {
                        string[] parts = str.Split('+');
                        string productID = parts[0];
                        decimal sPrice = Convert.ToDecimal(parts[1]);

                        Product p = db.Products.FirstOrDefault(t => t.ProductID == productID);
                        if (p != null)
                        {
                            SaleDetail sDetail = new SaleDetail();
                            sDetail.SaleID = addedSale.SaleID;
                            sDetail.ProductID = productID;
                            sDetail.OriginalPrice = p.Price;
                            sDetail.SalePrice = sPrice;

                            db.SaleDetails.Add(sDetail);
                        }
                    }
                }

                db.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                if (sale.SaleDetails.Count > 0)
                {
                    db.SaleDetails.RemoveRange(sale.SaleDetails);
                }
                db.Sales.Remove(sale);
                db.SaveChanges();
                return Json(new { success = false, error = "Lỗi." });
            }
        }

        //--Bắt đầu khuyến mãi
        public ActionResult BeginSale(int saleId)
        {
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            Sale sale = db.Sales.FirstOrDefault(s => s.SaleID == saleId);
            if (sale != null && sale.Status == "Chờ" && sale.StartTime <= DateTime.Now)
            {
                List<SaleDetail> saleDetails = sale.SaleDetails.ToList();
                foreach (SaleDetail detail in saleDetails)
                {
                    Product product = detail.Product;
                    product.Price = detail.SalePrice;
                }
                sale.Status = "Bắt đầu";
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        //--Thay đổi giá của sản phẩm sale nếu kết thúc
        public ActionResult EndSale(int saleId)
        {
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            Sale sale = db.Sales.FirstOrDefault(s => s.SaleID == saleId);
            if (sale != null && sale.Status == "Bắt đầu")
            {
                List<SaleDetail> saleDetails = sale.SaleDetails.ToList();
                foreach (SaleDetail detail in saleDetails)
                {
                    Product product = detail.Product;
                    product.Price = detail.OriginalPrice;
                }
                sale.Status = "Kết thúc";
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        //Xóa một danh sách khuyến mãi
        public ActionResult Delete(int saleID)
        {
            try
            {
                DatabaseSTechEntities db = new DatabaseSTechEntities();
                Sale sale = db.Sales.FirstOrDefault(s => s.SaleID == saleID && s.Status == "Chờ");
                List<SaleDetail> saleDetails = sale.SaleDetails.ToList();

                if (saleDetails.Count > 0)
                {
                    db.SaleDetails.RemoveRange(saleDetails);
                }

                db.Sales.Remove(sale);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
        }

        //Xem chi tiết 1 danh sách khuyến mãi
        public ActionResult Detail(int saleID)
        {
            try
            {
                DatabaseSTechEntities db = new DatabaseSTechEntities();
                Sale sale = db.Sales.FirstOrDefault(s => s.SaleID == saleID);

                ViewBag.ActiveNav = "sale";
                return View(sale);
            }
            catch(Exception ex)
            {
                return RedirectToAction("Index");
            }
        }
    }
}