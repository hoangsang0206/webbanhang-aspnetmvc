using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using STech_Web.Filters;
using STech_Web.Models;

namespace STech_Web.Areas.Admin.Controllers
{
    [AdminAuthorization]
    public class MegamenuController : Controller
    {
        // GET: Admin/Megamenu
        public ActionResult Index()
        {
            DatabaseSTechEntities db = new DatabaseSTechEntities();
            List<SidebarMenuNav> sidebar = db.SidebarMenuNavs.ToList();

            return View(sidebar);
        }
    }
}