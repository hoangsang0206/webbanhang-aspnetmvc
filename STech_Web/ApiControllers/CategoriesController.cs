using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.DynamicData;
using System.Web.Http;
using STech_Web.Models;
using STech_Web.ApiModels;

namespace STech_Web.ApiControllers
{
    public class CategoriesController : ApiController
    {
        public IHttpActionResult Get()
        {
            bool _isAdmin = User.IsInRole("Admin") ? true : false;

            DatabaseSTechEntities db = new DatabaseSTechEntities();
            List<Category> categories = db.Categories.ToList();
            List<CategoryAPI> categoriesAPI = new List<CategoryAPI>();

            foreach (Category cate in categories)
            {
                CategoryAPI category = new CategoryAPI((int)cate.Sort, cate.CateID, cate.CateName, cate.ImgSrc, cate.Products.Count);

                categoriesAPI.Add(category);
            }
            categoriesAPI = categoriesAPI.OrderBy(t => t.Sort).ToList();

            return Ok(new { categories = categoriesAPI, isAdmin = _isAdmin });
        }

        public CategoryAPI GetOne(string CateID)
        {
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            Category category = db.Categories.FirstOrDefault(t => t.CateID == CateID);
            CategoryAPI categoryApi = new CategoryAPI();
            if (category != null)
            {
                categoryApi = new CategoryAPI(category.Sort, category.CateID, category.CateName, category.ImgSrc, category.Products.Count);
            }
            
            return categoryApi;
        }
    }
}
