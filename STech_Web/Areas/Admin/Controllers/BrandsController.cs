using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using STech_Web.Filters;
using STech_Web.Models;

namespace STech_Web.Areas.Admin.Controllers
{
    [ManagerAuthorization]  
    public class BrandsController : Controller
    {
        // GET: Admin/Brands
        //Thêm hãng  -------------------------------------
        [HttpPost]
        public JsonResult AddBrand(Brand brand)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    DatabaseSTechEntities db = new DatabaseSTechEntities();
                    List<Category> categories = db.Categories.ToList();

                    //Kiểm tra category đã tồn tại chưa
                    Brand _brand = db.Brands.FirstOrDefault(t => t.BrandID == brand.BrandID);

                    if (_brand != null)
                    {
                        return Json(new { success = false, error = "Hãng này đã tồn tại." });
                    }

                    db.Brands.Add(brand);
                    db.SaveChanges();

                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, error = "Dữ liệu không hợp lệ." });
                }
            }
            catch (Exception ex) { }
            {
                return Json(new { success = false, error = "Đã xảy ra lỗi." });
            }   
        }

        //Sửa hãng --------------------------------------
        [HttpPost]
        public JsonResult UpdateBrand(Brand brand)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    DatabaseSTechEntities db = new DatabaseSTechEntities();
                    Brand _brand = db.Brands.FirstOrDefault(t => t.BrandID == brand.BrandID);

                    //Kiểm tra xem danh mục có tồn tại không
                    if (_brand == null)
                    {
                        return Json(new { success = false, error = "Hãng này không tồn tại." });
                    }

                    _brand.BrandName = brand.BrandName;
                    _brand.BrandAddress = brand.BrandAddress;
                    _brand.Phone = brand.Phone;
                    _brand.BrandImgSrc = brand.BrandImgSrc;
                    db.SaveChanges();

                    return Json(new { success = true });
                }

                return Json(new { success = false, error = "Dữ liệu không hợp lệ." });
            }
            catch (Exception ex) 
            {
                return Json(new { success = false, error = "Đã xảy ra lỗi." });
            }
        }


        //Xóa hãng ----------------------
        [HttpPost, AdminAuthorization]
        public JsonResult DeleteBrand(string brandID)
        {
            DatabaseSTechEntities db = new DatabaseSTechEntities();

            //Kiểm tra xem danh mục này có sản phẩm không 
            List<Product> products = db.Products.Where(t => t.BrandID == brandID).ToList();
            if (products.Count > 0)
            {
                string error = "Hãng này đã có " + products.Count + " sản phẩm, không thể xóa";
                return Json(new { success = false, err = error });

            }

            //Kiểm tra xem danh mục có tồn tại không
            Brand brand = db.Brands.FirstOrDefault(t => t.BrandID == brandID);
            if (brand == null)
            {
                return Json(new { success = false, error = "Hãng này không tồn tại." });
            }

            db.Brands.Remove(brand);
            db.SaveChanges();

            return Json(new { success = true });
        }
    }
}