using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.DynamicData;

namespace STech_Web.Models
{
    public class BrandAPI
    {
        public string BrandID { get; set; }
        public string BrandName { get; set; }
        public string Phone { get; set; }
        public string BrandAddress { get; set; }
        public string BrandImgSrc { get; set; }

        public BrandAPI() { }

        public BrandAPI(string BrandID, string BrandName, string Phone, string BrandAddress, string BrandImgSrc)
        {
            this.BrandID = BrandID;
            this.BrandName = BrandName;
            this.Phone = Phone;
            this.BrandAddress = BrandAddress;
            this.BrandImgSrc = BrandImgSrc;
        }
    }
}