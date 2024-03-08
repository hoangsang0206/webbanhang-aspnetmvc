using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace STech_Web.Models
{
    public class CategoryAPI
    {
        public int Sort { get; set; }
        public string CateID { get; set; }
        public string CateName { get; set; }
        public string ImgSrc { get; set; }
        public int ProductCount { get; set; }

        public CategoryAPI() { }

        public CategoryAPI(int Sort, string CateID, string CateName, string ImgSrc, int ProductCount) 
        {
            this.Sort = Sort;
            this.CateID = CateID;
            this.CateName = CateName;
            this.ImgSrc = ImgSrc;
            this.ProductCount = ProductCount;
        }
    }
}