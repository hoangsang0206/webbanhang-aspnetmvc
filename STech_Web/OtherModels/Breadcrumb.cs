using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace STech_Web.Models
{
    public class Breadcrumb
    {
        string breadcrumbName;
        string breadcrumbLink;

        public string BreadcrumbName { get => breadcrumbName; set => breadcrumbName = value; }
        public string BreadcrumbLink { get => breadcrumbLink; set => breadcrumbLink = value; }

        public Breadcrumb() { }

        public Breadcrumb(string breadcrumbName, string breadcrumbLink)
        {
            this.breadcrumbName = breadcrumbName;
            this.breadcrumbLink = breadcrumbLink;
        }
    }
}