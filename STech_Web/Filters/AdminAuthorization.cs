using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace STech_Web.Filters
{
    public class AdminAuthorization : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.User.IsInRole("Admin") == false) 
            {
                filterContext.Result = new RedirectResult("/admin/login");
            }
        }
    }
}