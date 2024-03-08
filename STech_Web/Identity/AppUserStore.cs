using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Microsoft.AspNet.Identity.EntityFramework;

namespace STech_Web.Identity
{
    public class AppUserStore : UserStore<AppUser>
    {
        public AppUserStore(AppDBContext dBContext) : base(dBContext) { }
    }
}